using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Zappy.Decode.Helper;

namespace Zappy.Decode.Screenshot
{
    internal static class ImageCaptureUtility
    {     
        private static Image failureScreenShot;
        public static ImageCodecInfo myImageCodecInfo;
        public static EncoderParameters myEncoderParameters;

        public static float ScalingFactor;
        public static Rectangle ScaledUpVIrtualScreen;

        static ImageCaptureUtility()
        {
            Encoder myEncoder;
            EncoderParameter myEncoderParameter;

            myImageCodecInfo = GetEncoderInfo("image/jpeg");

            myEncoder = Encoder.Quality;

            myEncoderParameters = new EncoderParameters(1);

                        myEncoderParameter = new EncoderParameter(myEncoder, 10L);
            myEncoderParameters.Param[0] = myEncoderParameter;

            ScalingFactor = GetScalingFactor();
            Rectangle virtualScreen = SystemInformation.VirtualScreen;
            Rectangle rectangle2 = new Rectangle(new Point(0, 0), virtualScreen.Size);
            int newWidth = (int)(rectangle2.Width * ScalingFactor);
            int newHeight = (int)(rectangle2.Height * ScalingFactor);
            ScaledUpVIrtualScreen = new Rectangle(virtualScreen.X, virtualScreen.Y, newWidth, newHeight);
        }

        internal static Image CaptureScreenShotAndDrawBounds(int x, int y, int width, int height, int borderWidth, 
            bool isActualControlBounds, bool resizeImage)
        {
            Rectangle rect = new Rectangle(x, y, width, height);
            rect.Intersect(SystemInformation.VirtualScreen);
            if (!rect.IsEmpty)
            {
                failureScreenShot = (Image)GetDesktopImage(resizeImage);
                Color color = isActualControlBounds ? Color.Red : Color.Green;
                using (Graphics graphics = Graphics.FromImage(failureScreenShot))
                {
                    using (Pen pen = new Pen(color, (float)borderWidth))
                    {
                        graphics.DrawRectangle(pen, rect);
                    }
                }
            }
            else
            {
                failureScreenShot = null;
            }
            return failureScreenShot;        }


        private static ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            int j;
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            }
            return null;
        }

        internal static object GetDesktopImage(bool resizeImage = true)
        {
            
            Bitmap image = new Bitmap(ScaledUpVIrtualScreen.Width, ScaledUpVIrtualScreen.Height);
            using (Graphics graphics = Graphics.FromImage(image))
            {
                graphics.CopyFromScreen(ScaledUpVIrtualScreen.Location, ScaledUpVIrtualScreen.Location, new System.Drawing.Size
                    (ScaledUpVIrtualScreen.Width, ScaledUpVIrtualScreen.Height));
            }
            if(ScalingFactor>1 && resizeImage)
            {
                                image = ResizeImage(image, SystemInformation.VirtualScreen.Width, SystemInformation.VirtualScreen.Height);
            }
            return image;
        }

        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        internal static bool IsScreenLockedOrRemoteSessionMinimized()
        {
            for (int i = 0; i < 5; i++)
            {
                if (IntPtr.Zero != NativeMethods.GetForegroundWindow())
                {
                    return false;
                }
                Thread.Sleep(100);
            }
            try
            {
                GetDesktopImage();
                return false;
            }
            catch (Exception)
            {
                return true;
            }
        }

                public static string ConvertImageToBase64String(Image image)
        {
            using (MemoryStream mstream = new MemoryStream())
            {
                image.Save(mstream, myImageCodecInfo, myEncoderParameters);
                byte[] buffer = mstream.ToArray();
                return Convert.ToBase64String(buffer);
            }
        }

        internal static void ResetScreenShot()
        {
            failureScreenShot = null;
        }

        internal static object LastFailureScreenShot =>
            failureScreenShot;

        internal static float GetScalingFactor()
        {
            Graphics g = Graphics.FromHwnd(IntPtr.Zero);
            IntPtr desktop = g.GetHdc();
            int LogicalScreenHeight = NativeMethods.GetDeviceCaps(desktop, 10);
            int PhysicalScreenHeight = NativeMethods.GetDeviceCaps(desktop, 117);

            float ScreenScalingFactor = (float)PhysicalScreenHeight / (float)LogicalScreenHeight;

            return ScreenScalingFactor;
        }

        public static Bitmap Base642Bitmap(string base64)
        {
            if (string.IsNullOrEmpty(base64)) return null;
            using (var ms = new System.IO.MemoryStream(Convert.FromBase64String(base64)))
            using (var image = System.Drawing.Image.FromStream(ms, false, true))
                return new System.Drawing.Bitmap(image);
        }
    }
}