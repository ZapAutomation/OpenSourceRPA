using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading;

namespace Zappy.Decode.Helper
{
    internal class BitmapFileSaver
    {
        private static BitmapFileSaver instance;
        private static readonly object locker = new object();
        public event EventHandler<BitmapSaveEventArgs> BitmapSaved;


        private BitmapFileSaver()
        {
        }

        [DllImport("GDI32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);
        internal void SaveImageAsync(IntPtr hBitmap, string filename, ImageFormat imageFormat)
        {
            ThreadPool.QueueUserWorkItem(delegate (object param)
            {
                try
                {
                    using (Bitmap bitmap = Image.FromHbitmap(hBitmap))
                    {
                        object[] args = new object[] { bitmap.Width, bitmap.Height, filename };
                                                if (imageFormat == ImageFormat.Jpeg)
                        {
                            bitmap.SaveCompressedJpeg(filename, 60L);
                        }
                        else
                        {
                            bitmap.SafeSave(filename, imageFormat);
                        }
                        if (this.BitmapSaved != null)
                        {
                            this.BitmapSaved(this, new BitmapSaveEventArgs(filename));
                        }
                        else
                        {
                            object[] objArray2 = new object[] { filename };
                                                    }
                    }
                }
                catch (SystemException exception)
                {
                    object[] objArray3 = new object[] { exception, filename };
                                    }
                finally
                {
                    DeleteObject(hBitmap);
                }
            });
        }

        internal static BitmapFileSaver Instance
        {
            get
            {
                if (instance == null)
                {
                    object locker = BitmapFileSaver.locker;
                    lock (locker)
                    {
                        if (instance == null)
                        {
                            instance = new BitmapFileSaver();
                        }
                    }
                }
                return instance;
            }
        }
    }
}