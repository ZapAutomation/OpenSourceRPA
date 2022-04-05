using System;
using System.ComponentModel;
using System.Drawing;
using System.Xml.Serialization;
using Zappy.Decode.Helper;
using Zappy.Helpers;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.OCR
{
    [Serializable]
    [Description("OCR To Clipboard")]
    public class OcrToClipboardAction : TemplateAction
    {
        [Category("Input")]
        [Description("Handle to the window")]
        public DynamicProperty<int> Hwnd_Int { get; set; }
        [XmlIgnore, Newtonsoft.Json.JsonIgnore]
        [Description(" hexadecimal value")]
        [Browsable(false)]
        public IntPtr Hwnd { get; set; }

        [Category("Input")]
        [Description("Language here default language is English")]
        [DefaultValue("eng")] public string Language { get; set; }

        public OcrToClipboardAction()
        {
            Id = ActionIDRegister.GetUniqueId();
            SelfGuid = Guid.NewGuid();
            Timestamp = WallClock.UtcNow;
            Language = "eng";
        }

        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            if (Hwnd_Int.ValueSpecified)                Hwnd = new IntPtr(Hwnd_Int.Value);
            string _CaptureData = Capture(this.Hwnd);
            if (!CommonProgram.SetTextInClipboard(_CaptureData))
                throw new Exception("Unable to \"Set\" text to clipboard!");
        }


        public string Capture(IntPtr handle)
        {
            uint _Flags = 1;
            if (handle == IntPtr.Zero)
            {
                handle = NativeMethods.GetDesktopWindow();
                _Flags = 0;
            }

            NativeMethods.RECT rect = new NativeMethods.RECT();
            NativeMethods.GetWindowRect(handle, out rect);
            using (Bitmap Bmp = new Bitmap(rect.right - rect.left, rect.bottom - rect.top))
            {
                using (Graphics memoryGraphics = Graphics.FromImage(Bmp))
                {
                    IntPtr dc = memoryGraphics.GetHdc();
                    try
                    {
                        bool success = NativeMethods.PrintWindow(handle, dc, _Flags);
                    }
                    finally
                    {
                        memoryGraphics.ReleaseHdc(dc);
                    }
                }
                                                                                                                                ProcessImageUsingTessaract processImageUsingTessaract = new ProcessImageUsingTessaract();

                return processImageUsingTessaract.ProcessBitmapImage(Bmp, Language);

                            }
        }

                        

        public override string AuditInfo()
        {
            return base.AuditInfo() + " Language:" + this.Language;
        }
    }
}
