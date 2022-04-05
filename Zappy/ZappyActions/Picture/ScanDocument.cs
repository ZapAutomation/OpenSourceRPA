using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;
using WIA;
using Zappy.Helpers;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;
using Zappy.ZappyActions.Picture.Helpers;

namespace Zappy.ZappyActions.Picture
{
    [Description("Scan Document And Save with .jpeg, .png ,.tiff extension")]
    public class ScanDocument : TemplateAction
    {
        
        [Category("Input")]
        [XmlIgnore, JsonIgnore]
        [Description("Scanners to scan the document")]
        public List<Scanner> Scanners { get; set; }

        [Category("Input")]
        [Description("Type of Scan Document Save with .jpeg, .png ,.tiff extension")]
        public ImageType Type { get; set; }

        [Category("Output")]
        [Description("File path of scan document")]
        public DynamicProperty<string> Path { get; set; }

        public ScanDocument()
        {
            Path = System.IO.Path.Combine(CrapyConstants.imageDirectory, Guid.NewGuid().ToString());

                        var deviceManager = new DeviceManager();
            Scanners = new List<Scanner>();
                        for (int i = 1; i <= deviceManager.DeviceInfos.Count; i++)
            {
                                if (deviceManager.DeviceInfos[i].Type != WiaDeviceType.ScannerDeviceType)
                {
                    continue;
                }

                                                Scanners.Add(new Scanner(deviceManager.DeviceInfos[i]));
            }
        }



                                                public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            Scanner device = Scanners[0];

                        if (device == null)
            {
                throw new Exception("No scanners found");
            }

            ImageFile image = new ImageFile();
            string imageExtension = "";

                                                switch (Type)
            {
                case ImageType.PNG:
                    image = device.ScanPNG();
                    imageExtension = ".png";
                    break;
                case ImageType.JPEG:
                    image = device.ScanJPEG();
                    imageExtension = ".jpeg";
                    break;
                case ImageType.TIFF:
                    image = device.ScanTIFF();
                    imageExtension = ".tiff";
                    break;
            }

                        var path = Path + imageExtension;
            image.SaveFile(path);
        }

        public override string AuditInfo()
        {
            return base.AuditInfo() + "Input path" + this.Path;
        }
    }
}