using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.GoogleSpreadSheet
{
    public class GetSpreadSheetValue : TemplateAction
    {

        static readonly string[] Scopes = { SheetsService.Scope.Spreadsheets };

        [Category("Input")]
        [Description("Id of created Google SpreadSheet")]
        public DynamicProperty<string> SpreadsheetId { get; set; }

        [Category("Input")]
        [Description("Google SpreadSheet name")]
        public DynamicProperty<string> SheetName { get; set; }

        [Category("Input")]
        [Description("Range with row and column as ")]
        public DynamicProperty<string> CellAddress { get; set; }

        [Category("Input")]
        [Description("Json file path of the created spredsheet")]
        public DynamicProperty<string> JsonFilePathofSheet { get; set; }

        [Category("Output")]
        [Description("Gets the value of set CellAddress ")]
        public string Value { get; set; }

        public static GoogleCredential HttpClientInitializer;
        private static SheetsService service;
        public string ApplicationName;

                public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            GoogleCredential credential;

            using (var stream = new System.IO.FileStream(JsonFilePathofSheet, FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream)
                    .CreateScoped(Scopes);
            }

                        service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            GetRange();
        }

        void GetRange()
        {
            
            String range = CellAddress;
            var valueRange = new ValueRange();

            SpreadsheetsResource.ValuesResource.GetRequest getRequest =
                       service.Spreadsheets.Values.Get(SpreadsheetId, range);
            getRequest.ValueRenderOption = SpreadsheetsResource.ValuesResource.GetRequest.ValueRenderOptionEnum.FORMATTEDVALUE;
            ValueRange getResponse = getRequest.Execute();
            Value = (string)getResponse.Values.First().FirstOrDefault();
        }
    }
}

