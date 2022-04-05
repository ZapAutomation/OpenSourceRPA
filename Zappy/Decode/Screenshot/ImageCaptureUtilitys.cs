using System.Drawing;

namespace Zappy.Decode.Screenshot
{
    internal class ImageCaptureUtilitys
    {
        internal static Image CaptureScreenShotAndDrawBounds(Rectangle bounds, int borderWidth, bool isActualControlBounds, bool resizeImage)
        {
            return ImageCaptureUtility.CaptureScreenShotAndDrawBounds(bounds.X, bounds.Y, bounds.Width, 
                bounds.Height, borderWidth, isActualControlBounds, resizeImage);
        }

        internal static object GetDesktopImage() =>
            ImageCaptureUtility.GetDesktopImage();

        internal static bool IsScreenLockedOrRemoteSessionMinimized() =>
            ImageCaptureUtility.IsScreenLockedOrRemoteSessionMinimized();

        internal static void ResetScreenShot()
        {
            ImageCaptureUtility.ResetScreenShot();
        }

                    }
}