using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Amazon;
using Amazon.Textract;
using Amazon.Textract.Model;
using Newtonsoft.Json;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;
using ZappyMessages.Helpers;

namespace Zappy.ZappyActions.OCR
{
    public abstract class AwsTextExtractServices : TemplateAction
    {
        public AwsTextExtractServices()
        {
            RegionName = "us-east-2";
                    }

        [Category("Input")]
        [Description("Specify AWS Access Key ID")]
        public DynamicProperty<string> AwsAccessKeyId { get; set; }

        [Category("Input")]
        [Description("Specify AWS Secret Access Key")]
        public DynamicProperty<string> AwsSecretAccessKey { get; set; }

        [Category("Optional")]
        [Description("Specify AWS Region")]
        public DynamicProperty<string> RegionName { get; set; }

        [Category("Optional")]
        [Description("Analyze document for key value pairs")]
        public DynamicProperty<bool> AnalyzeDocument { get; set; }

        [Category("Output")]
        [Description("AWS Extraction JSON")]
        public string JSONData { get; set; }

        [Category("Output")][XmlIgnore, JsonIgnore]
        [Description("AWS Extraction JSON")]
        public List<Block> BlockData { get; set; }

        private IAmazonTextract textract;

        public string GetAwsDocumentText(string InputFilePath)
        {
            textract = new AmazonTextractClient(AwsAccessKeyId, AwsSecretAccessKey, RegionEndpoint.GetBySystemName(RegionName));
            dynamic localTask;
            if (AnalyzeDocument)
            {
                Task<AnalyzeDocumentResponse> localTaskDoc = AnalyseDocumentLocal(InputFilePath);
                localTaskDoc.Wait();
                localTask = localTaskDoc;
            }
            else
            {
                Task<DetectDocumentTextResponse> localTaskAnalysis = DetectTextLocal(InputFilePath);
                localTaskAnalysis.Wait();
                localTask = localTaskAnalysis;
            }
            List<Block> blocks = localTask.Result.Blocks;
            BlockData = blocks;
            JSONData = ZappySerializer.SerializeObject(blocks);
            StringBuilder sb = new StringBuilder();
            blocks.ForEach(x =>
            {
                if (x.BlockType.Equals("LINE"))
                {
                    sb.Append(x.Text);
                    sb.Append(Environment.NewLine);
                }
            });
            return sb.ToString();
        }

        public async Task<DetectDocumentTextResponse> DetectTextLocal(string localPath)
        {
            var result = new DetectDocumentTextResponse();

            if (File.Exists(localPath))
            {
                var request = new DetectDocumentTextRequest();
                request.Document = new Document
                {
                    Bytes = new MemoryStream(File.ReadAllBytes(localPath))
                };
                return await this.textract.DetectDocumentTextAsync(request);
            }
            return result;
        }

        public async Task<AnalyzeDocumentResponse> AnalyseDocumentLocal(string localPath)
        {
            var result = new AnalyzeDocumentResponse();
            if (File.Exists(localPath))
            {
                var request = new AnalyzeDocumentRequest();
                request.FeatureTypes = new List<string>();
                request.FeatureTypes.Add("TABLES");
                request.FeatureTypes.Add("FORMS");
                request.Document = new Document
                {
                    Bytes = new MemoryStream(File.ReadAllBytes(localPath))
                };
                return await this.textract.AnalyzeDocumentAsync(request);
            }
            return result;
        }
    }
}