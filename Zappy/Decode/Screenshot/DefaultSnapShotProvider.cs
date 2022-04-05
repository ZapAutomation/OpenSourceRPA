using System;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using Zappy.ActionMap.ZappyTaskUtil;
using Zappy.Decode.Helper;
using Zappy.Helpers;

namespace Zappy.Decode.Screenshot
{
    internal class DefaultSnapShotProvider : ISnapShotProvider
    {
                private static ImageFormat ImageFileFormat = ImageFormat.Png;
                                
        private static string GetFilePath() =>
            Path.Combine(CrapyConstants.imageDirectory, Path.ChangeExtension(Guid.NewGuid().ToString(), ImageFileFormat.GetExtension()));

                                                                                                                                                                                                                                
        ZappyTaskImageInfo ISnapShotProvider.TakeAndSaveSnapShotAsync(Screen activeScreen)
        {
            IntPtr hBitmap = TakeScreenShotOfActiveMonitor(activeScreen);
            long ticks = DateTime.UtcNow.Ticks;
            if (hBitmap != IntPtr.Zero)
            {
                string filePath = GetFilePath();
                BitmapFileSaver.Instance.SaveImageAsync(hBitmap, filePath, ImageFileFormat);
                return new ZappyTaskImageInfo(filePath, ticks);
            }
            return null;
        }

        private static IntPtr TakeScreenShotOfActiveMonitor(Screen activeScreen)
        {
                        {
                IntPtr zero = IntPtr.Zero;
                IntPtr hdc = IntPtr.Zero;
                IntPtr hgdiobj = IntPtr.Zero;
                try
                {
                    
                    if (((activeScreen == null) || activeScreen.Bounds.IsEmpty) || activeScreen.Bounds.Size.IsEmpty)
                    {
                        
                        activeScreen = Screen.PrimaryScreen;
                    }
                    if (((activeScreen != null) && !activeScreen.Bounds.IsEmpty) && !activeScreen.Bounds.Size.IsEmpty)
                    {
                        int width = activeScreen.Bounds.Width;
                        int height = activeScreen.Bounds.Height;
                        zero = GDI32.CreateDC("DISPLAY", activeScreen.DeviceName, null, IntPtr.Zero);
                        hdc = GDI32.CreateCompatibleDC(zero);
                        hgdiobj = GDI32.CreateCompatibleBitmap(zero, width, height);
                                                
                        IntPtr ptr4 = GDI32.SelectObject(hdc, hgdiobj);
                        GDI32.BitBlt(hdc, 0, 0, width, height, zero, 0, 0, GDI32.TernaryRasterOperations.CAPTUREBLT | GDI32.TernaryRasterOperations.SRCCOPY);
                        
                        GDI32.SelectObject(hdc, ptr4);
                    }
                    else
                    {
                        
                    }
                }
                finally
                {
                    if (zero != IntPtr.Zero)
                    {
                        GDI32.DeleteDC(zero);
                    }
                    if (hdc != IntPtr.Zero)
                    {
                        GDI32.DeleteDC(hdc);
                    }
                }
                
                return hgdiobj;
            }
        }

                                                                                                                                public void CleanupImageDirectory()
        {
                    }

                                    }
}