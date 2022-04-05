using Emgu.CV;
using Emgu.CV.Structure;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml.Serialization;
using Zappy.Decode.Helper;
using Zappy.Decode.LogManager;
using Zappy.Helpers;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;
using Zappy.ZappyActions.Picture.Helpers;
using Point = System.Drawing.Point;

namespace Zappy.ZappyActions.Picture
{
    public abstract class ImageClickHelper : TemplateAction
    {
        public ImageClickHelper()
        {
            Threshold = 80;
            ImageSelector = new ImageObject();
            ImageSelector.ClickLocation = "Center";
            PauseTimeAfterAction = 1000;
        }
        [Editor(typeof(ClipPickerEditor), typeof(UITypeEditor))]
                [Category("Input")]
        [Description("Select the image to perform action on")]
        [XmlIgnore, JsonIgnore]
        public ImageObject ImageSelector { get; set; }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public string ClickLocation
        {
            get
            {
                if (ImageSelector == null) return null;
                return ImageSelector.ClickLocation;
            }
            set
            {
                if (ImageSelector == null)
                    ImageSelector = new ImageObject();
                ImageSelector.ClickLocation = value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        [XmlElement("PatternFile")]
        public byte[] LargeIconSerialized
        {
            get
            {                 if (ImageSelector == null || ImageSelector.PatternFile == null) return null;
                using (MemoryStream ms = new MemoryStream())
                {
                    var i2 = new Bitmap(ImageSelector.PatternFile);
                    i2.Save(ms, ImageFormat.Bmp);
                    return ms.ToArray();
                }
            }
            set
            {                 if (value == null)
                {
                    ImageSelector = null;
                }
                else
                {
                    using (MemoryStream ms = new MemoryStream(value))
                    {
                        if (ImageSelector == null)
                            ImageSelector = new ImageObject();
                        ImageSelector.PatternFile = new Bitmap(ms);

                    }

                }
            }
        }

        [Category("Input")]
        public DynamicProperty<string> TopLevelWindowName { get; set; }

        [Category("Optional")]
        [Description("Set this property if Top Level Window Name changes every time")]
        public DynamicProperty<string> ExeFileName { get; set; }

        [Category("Optional")]
        [Description("Set this property you want to enumerate all top level windows")]
        public DynamicProperty<bool> EnumerateAllTopLevelWindows { get; set; }

        [Category("Optional")]
        public DynamicProperty<int> Threshold { get; set; }

                                                        public delegate bool EnumDelegate(IntPtr hWnd, int lParam);

                                                                [DllImport("user32.dll", EntryPoint = "EnumDesktopWindows",
        ExactSpelling = false, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool EnumDesktopWindows(IntPtr hDesktop, EnumDelegate lpEnumCallbackFunction, IntPtr lParam);

        public List<IntPtr> GetTopLevelWindowHandles()
        {
            List<IntPtr> collection = new List<IntPtr>();
            EnumDelegate filter = delegate (IntPtr hWnd, int lParam)
            {
                StringBuilder strbTitle = new StringBuilder(255);
                                                if (NativeMethods.IsWindowVisible(hWnd))
                {
                    if (NativeMethods.GetWindowRect(hWnd, out NativeMethods.RECT rct))
                    {
                        if ((rct.right - rct.left) != 0 &&
                            (rct.bottom - rct.top) != 0)
                        { collection.Add(hWnd); }
                    }
                }
                return true;
            };
            EnumDesktopWindows(IntPtr.Zero, filter, IntPtr.Zero);
            return collection;
        }

        public List<IntPtr> GetExeWindowHandles(string Exe)
        {
            IntPtr handle = IntPtr.Zero;
            Process[] processlist;
            if (string.IsNullOrWhiteSpace(Exe))
                processlist = Process.GetProcesses();
            else
                processlist = Process.GetProcessesByName(Exe);
            List<IntPtr> windowHandles = new List<IntPtr>();

            foreach (Process process in processlist)
            {
                                if (process.MainWindowHandle == IntPtr.Zero)
                {
                    continue;
                }
                if (NativeMethods.GetWindowRect(process.MainWindowHandle, out NativeMethods.RECT rct))
                {
                    if ((rct.right - rct.left) == 0 ||
                        (rct.bottom - rct.top) == 0)
                        continue;
                }
                windowHandles.Add(process.MainWindowHandle);
            }
            return windowHandles;
        }

        private Rectangle GetLocationOfPattern(Bitmap bitmap1, Bitmap bitmap2, float scalingFactor)
        {
            Rectangle location = Rectangle.Empty;
            location = FindImageUsingEmguCV(bitmap2, bitmap1, scalingFactor, ((double)Threshold / 100));
            return location;
        }
        private Bitmap CaptureWindow(IntPtr handle)
        {
            NativeMethods.RECT rect = new NativeMethods.RECT();
            float scalingFactor = GetScalingFactor(handle);
            NativeMethods.GetWindowRect(handle, out rect);
            int width = rect.right - rect.left;
            int height = rect.bottom - rect.top;

            int newWidth = (int)(width * scalingFactor);
            int newHeight = (int)(height * scalingFactor);

            Bitmap bmp = new Bitmap(newWidth, newHeight, PixelFormat.Format32bppArgb);
            Graphics g = Graphics.FromImage(bmp);
            g.CopyFromScreen(rect.left,
                                        rect.top,
                                        0,
                                        0,
                                        new System.Drawing.Size(newWidth, newHeight),
                                        CopyPixelOperation.SourceCopy);
            g.Dispose();
            return bmp;
                                                                                    
                                                                                                                                                                                                
            
        }

        public Point GetPointOfImage(IntPtr handle, Rectangle location)
        {
            float scalingFactor = GetScalingFactor(handle);
            if (string.IsNullOrEmpty(ImageSelector.ClickLocation))
            {
                throw new Exception("Click Location is not specified");
            }
            Point point = new Point();
            if (ImageSelector.ClickLocation == "Center")
            {
                point.X = location.X + location.Width / 2;
                point.Y = location.Y + location.Height / 2;
            }
            else if (ImageSelector.ClickLocation == "TopLeft")
            {
                point.X = location.X;
                point.Y = location.Y;

            }
            else if (ImageSelector.ClickLocation == "TopRight")
            {
                point.X = location.X + location.Width;
                point.Y = location.Y;
            }
            else if (ImageSelector.ClickLocation == "BottomLeft")
            {
                point.X = location.X;
                point.Y = location.Y + location.Height;
            }
            else if (ImageSelector.ClickLocation == "BottomRight")
            {
                point.X = location.X + location.Width;
                point.Y = location.Y + location.Height;
            }
            else
            {
                point.X = Convert.ToInt32(System.Windows.Point.Parse(ImageSelector.ClickLocation).X);
                point.Y = Convert.ToInt32(System.Windows.Point.Parse(ImageSelector.ClickLocation).Y);
                point.X = (int)(point.X * scalingFactor) + location.X;
                point.Y = (int)(point.Y * scalingFactor) + location.Y;                
            }
            NativeMethods.RECT rect = new NativeMethods.RECT();
            NativeMethods.GetWindowRect(handle, out rect);
            
                        point.X = (int)(point.X/scalingFactor) + rect.left;
            point.Y = (int)(point.Y/ scalingFactor) + rect.top;
            return point;
        }
        public Rectangle ProcessImage(IntPtr handle)
        {
            Rectangle location = Rectangle.Empty;
            float scalingFactor = NativeMethods.GetDpiForWindow(handle) / (float)96;


                        WindowHelper.FocusWindow(handle);
            
            Bitmap WindowImage = CaptureWindow(handle);

            int width = ImageSelector.PatternFile.Width;
            int height = ImageSelector.PatternFile.Height;

            int newWidth = (int)(width * scalingFactor);
            int newHeight = (int)(height * scalingFactor);
            Bitmap bitmap1 = new Bitmap(ImageSelector.PatternFile, newWidth, newHeight);

            
            if (WindowImage.Height > bitmap1.Height)
                location = GetLocationOfPattern(bitmap1, WindowImage, scalingFactor);
            
                        string imagePath = Path.Combine(CrapyConstants.imageDirectory, SelfGuid.ToString() + ".bmp");
            WindowImage.Save(imagePath, ImageFormat.Bmp);

            WindowImage.Dispose();
            return location;
        }

                        
        
        
        
        
                                                
                        
        
                
        internal Rectangle FindImageUsingEmguCV(Bitmap filepathB, Bitmap filepathA, float scalingFactor,double threshold)
        {
            Image<Gray, Byte> source = new Image<Gray, Byte>(filepathB);             Image<Gray, Byte> template = new Image<Gray, Byte>(filepathA);                         Rectangle match = Rectangle.Empty;

            using (Image<Gray, float> result = source.MatchTemplate(template, Emgu.CV.CvEnum.TemplateMatchingType.CcoeffNormed))
            {
                double[] minValues, maxValues;
                Point[] minLocations, maxLocations;
                result.MinMax(out minValues, out maxValues, out minLocations, out maxLocations);
                if (maxValues[0] > threshold)
                {
                    match = new Rectangle(maxLocations[0], template.Size);
                }
            }
                                    return match;

        }


        internal Point GetPointOnImageToClick()
        {
            IntPtr handle = IntPtr.Zero;
            Rectangle location = Rectangle.Empty;

            
            if (!string.IsNullOrEmpty(TopLevelWindowName))
            {
                handle = NativeMethods.FindWindow(null, TopLevelWindowName);
                if (handle == IntPtr.Zero)
                    throw new Exception("Window is NOT found");
                location = ProcessImage(handle);
            }
                                                                                                                        else
            {
                                                                List<IntPtr> windowHandles;
                if(EnumerateAllTopLevelWindows)
                    windowHandles = GetTopLevelWindowHandles();
                else
                    windowHandles = GetExeWindowHandles(ExeFileName);

                foreach (IntPtr windowHandle in windowHandles)
                {
                                        try
                    {
                        location = ProcessImage(windowHandle);
                    }
                    catch(Exception Ex)
                    {
                        CrapyLogger.log.Error(Ex);
                    }
                                        if (!location.IsEmpty)
                    {
                        handle = windowHandle;
                        break;
                    }
                }
                windowHandles.Clear();

            }
            if (location.IsEmpty)
            {
                throw new Exception("Pattern is not found");
            }
            WindowHelper.FocusWindow(handle);

                        return GetPointOfImage(handle, location);
        }
        internal float GetScalingFactor(IntPtr handle)
        {
            float scalingFactor = NativeMethods.GetDpiForWindow(handle) / (float)96;
            return scalingFactor;
        }
    }
}
