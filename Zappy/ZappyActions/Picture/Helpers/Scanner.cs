using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using WIA;
using Zappy.Decode.LogManager;

namespace Zappy.ZappyActions.Picture.Helpers
{
    [Description("Scanners for scan the Document ")]
    public class Scanner
    {
        private readonly DeviceInfo _deviceInfo;
        private int resolution = 150;
        private int width_pixel = 1250;
        private int height_pixel = 1700;
        private int color_mode = 1;

        public Scanner(DeviceInfo deviceInfo)
        {
            this._deviceInfo = deviceInfo;
        }

                                        public ImageFile ScanPNG()
        {
                                    var device = this._deviceInfo.Connect();

                        CommonDialogClass dlg = new CommonDialogClass();

            var item = device.Items[1];

            try
            {
                AdjustScannerSettings(item, resolution, 0, 0, width_pixel, height_pixel, 0, 0, color_mode);

                object scanResult = dlg.ShowTransfer(item, WIA.FormatID.wiaFormatPNG, true);

                if (scanResult != null)
                {
                    var imageFile = (ImageFile)scanResult;

                                        return imageFile;
                }
            }
            catch (COMException e)
            {
                                Console.WriteLine(e.ToString());

                uint errorCode = (uint)e.ErrorCode;

                                if (errorCode == 0x80210006)
                {
                    MessageBox.Show("The scanner is busy or isn't ready");
                }
                else if (errorCode == 0x80210064)
                {
                    MessageBox.Show("The scanning process has been cancelled.");
                }
                else
                {
                    MessageBox.Show("A non catched error occurred, check the console", "Error", MessageBoxButtons.OK);
                }
            }

            return new ImageFile();
        }

                                        public ImageFile ScanJPEG()
        {
                                    var device = this._deviceInfo.Connect();

                        CommonDialogClass dlg = new CommonDialogClass();

            var item = device.Items[1];

            try
            {
                AdjustScannerSettings(item, resolution, 0, 0, width_pixel, height_pixel, 0, 0, color_mode);

                object scanResult = dlg.ShowTransfer(item, WIA.FormatID.wiaFormatJPEG, true);

                if (scanResult != null)
                {
                    var imageFile = (ImageFile)scanResult;

                                        return imageFile;
                }
            }
            catch (COMException e)
            {
                                Console.WriteLine(e.ToString());

                uint errorCode = (uint)e.ErrorCode;

                                if (errorCode == 0x80210006)
                {
                    MessageBox.Show("The scanner is busy or isn't ready");
                }
                else if (errorCode == 0x80210064)
                {
                    MessageBox.Show("The scanning process has been cancelled.");
                }
                else
                {
                    MessageBox.Show("A non catched error occurred, check the console", "Error", MessageBoxButtons.OK);
                }
            }

            return new ImageFile();
        }

                                        public ImageFile ScanTIFF()
        {
                                    var device = this._deviceInfo.Connect();

                        CommonDialogClass dlg = new CommonDialogClass();

            var item = device.Items[1];

            try
            {
                AdjustScannerSettings(item, resolution, 0, 0, width_pixel, height_pixel, 0, 0, color_mode);

                object scanResult = dlg.ShowTransfer(item, WIA.FormatID.wiaFormatTIFF, true);

                if (scanResult != null)
                {
                    var imageFile = (ImageFile)scanResult;

                                        return imageFile;
                }
            }
            catch (COMException e)
            {
                                Console.WriteLine(e.ToString());

                uint errorCode = (uint)e.ErrorCode;

                                if (errorCode == 0x80210006)
                {
                    MessageBox.Show("The scanner is busy or isn't ready");
                }
                else if (errorCode == 0x80210064)
                {
                    MessageBox.Show("The scanning process has been cancelled.");
                }
                else
                {
                    MessageBox.Show("A non catched error occurred, check the console", "Error", MessageBoxButtons.OK);
                }
            }

            return new ImageFile();
        }

                                                                                                        private static void AdjustScannerSettings(IItem scannnerItem, int scanResolutionDPI, int scanStartLeftPixel, int scanStartTopPixel, int scanWidthPixels, int scanHeightPixels, int brightnessPercents, int contrastPercents, int colorMode)
        {
            const string WIA_SCAN_COLOR_MODE = "6146";
            const string WIA_HORIZONTAL_SCAN_RESOLUTION_DPI = "6147";
            const string WIA_VERTICAL_SCAN_RESOLUTION_DPI = "6148";
            const string WIA_HORIZONTAL_SCAN_START_PIXEL = "6149";
            const string WIA_VERTICAL_SCAN_START_PIXEL = "6150";
            const string WIA_HORIZONTAL_SCAN_SIZE_PIXELS = "6151";
            const string WIA_VERTICAL_SCAN_SIZE_PIXELS = "6152";
            const string WIA_SCAN_BRIGHTNESS_PERCENTS = "6154";
            const string WIA_SCAN_CONTRAST_PERCENTS = "6155";
            SetWIAProperty(scannnerItem.Properties, WIA_HORIZONTAL_SCAN_RESOLUTION_DPI, scanResolutionDPI);
            SetWIAProperty(scannnerItem.Properties, WIA_VERTICAL_SCAN_RESOLUTION_DPI, scanResolutionDPI);
            SetWIAProperty(scannnerItem.Properties, WIA_HORIZONTAL_SCAN_START_PIXEL, scanStartLeftPixel);
            SetWIAProperty(scannnerItem.Properties, WIA_VERTICAL_SCAN_START_PIXEL, scanStartTopPixel);
            SetWIAProperty(scannnerItem.Properties, WIA_HORIZONTAL_SCAN_SIZE_PIXELS, scanWidthPixels);
            SetWIAProperty(scannnerItem.Properties, WIA_VERTICAL_SCAN_SIZE_PIXELS, scanHeightPixels);
            SetWIAProperty(scannnerItem.Properties, WIA_SCAN_BRIGHTNESS_PERCENTS, brightnessPercents);
            SetWIAProperty(scannnerItem.Properties, WIA_SCAN_CONTRAST_PERCENTS, contrastPercents);
            SetWIAProperty(scannnerItem.Properties, WIA_SCAN_COLOR_MODE, colorMode);
        }

                                                        private static void SetWIAProperty(IProperties properties, object propName, object propValue)
        {
            try
            {
                Property prop = properties.get_Item(ref propName);
                prop.set_Value(ref propValue);
            }
            catch (Exception ex)
            {
                CrapyLogger.log.Error(ex);
            }
        }

                                        public override string ToString()
        {
            return (string)this._deviceInfo.Properties["Name"].get_Value();
        }

    }
}