using System.ComponentModel;
using System.Drawing;

namespace Zappy.ZappyActions.OCR.Snipping.SnippingTool
{
    [Description("Device Information Of Snipping Tool")]
    public class DeviceInfo
    {
        [Description("Tools device name")]
        public string DeviceName { get; set; }

        [Description("Vertical Resolution")]
        public int VerticalResolution { get; set; }

        [Description("HorizontalResolution")]
        public int HorizontalResolution { get; set; }

        [Description("MonitorArea with four locations")]
        public Rectangle MonitorArea { get; set; }
    }
}