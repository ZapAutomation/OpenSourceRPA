using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace Zappy.Decode.Helper
{
    internal static class ImageExtensions
    {
        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] imageEncoders = ImageCodecInfo.GetImageEncoders();
            if (imageEncoders == null)
            {
                return null;
            }
            return imageEncoders.FirstOrDefault<ImageCodecInfo>(q => (q.FormatID == format.Guid));
        }

        internal static string GetExtension(this ImageFormat imageFormat) =>
            imageFormat.ToString();

        internal static string GetSearchPattern(this ImageFormat imageFormat) =>
            ("*." + imageFormat.ToString());

        internal static void SafeSave(this Image image, string fileName, ImageFormat imageFormat)
        {
            try
            {
                image.Save(fileName, imageFormat);
            }
            catch (SystemException)
            {
                string directoryName = Path.GetDirectoryName(fileName);
                if (Directory.Exists(directoryName))
                {
                    throw;
                }
                Directory.CreateDirectory(directoryName);
                image.Save(fileName, imageFormat);
            }
        }

        internal static void SaveCompressedJpeg(this Image image, string fileName, long quality)
        {
            ImageCodecInfo encoder = GetEncoder(ImageFormat.Jpeg);
            EncoderParameters encoderParams = new EncoderParameters(1);
            encoderParams.Param[0] = new EncoderParameter(Encoder.Quality, quality);
            try
            {
                image.Save(fileName, encoder, encoderParams);
            }
            catch (SystemException)
            {
                string directoryName = Path.GetDirectoryName(fileName);
                if (Directory.Exists(directoryName))
                {
                    throw;
                }
                Directory.CreateDirectory(directoryName);
                image.Save(fileName, encoder, encoderParams);
            }
        }
    }
}