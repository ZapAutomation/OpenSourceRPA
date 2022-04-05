using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using Zappy.Decode.Helper;
using Zappy.Helpers;
using Zappy.SharedInterface.Helper;
using Point = System.Drawing.Point;
using System.ComponentModel;
using Zappy.Decode.Hooks.Keyboard;
using Zappy.InputData;


namespace Zappy.ZappyActions.Picture
{
    [Description("This first clicks on the image and then types the required text")]
    public class ImageClickAndType : ImageClickHelper
    {
        public ImageClickAndType()
        {
            PauseTimeAfterAction = 0;
        }
        [Category("Input")]
        [Description("Text to send on focused window")]
        public DynamicTextProperty Text { get; set; }

        [Category("Input")]
        [Description("Select one of the modifier key")]
        public ModifierKeys ModifierKeys { get; set; }
        [Category("Optional")] public int DelayBetweenKeyboardCharacters { get; set; }
        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            Point point = GetPointOnImageToClick();

            NativeMethods.SetCursorPos(point.X, point.Y);
            NativeMethods.mouse_event(NativeMethods.MOUSEEVENTF_LEFTDOWN | NativeMethods.MOUSEEVENTF_LEFTUP, Convert.ToUInt32(point.X), Convert.ToUInt32(point.Y), 0, UIntPtr.Zero);
            Thread.Sleep(1000);
            int result = ExecuteTask.KeyboardWrapper.KeyboardWrapper.sendKeys(Text, Convert.ToInt32(this.ModifierKeys), null, 0, DelayBetweenKeyboardCharacters);
        }

        public override string AuditInfo()
        {
            return base.AuditInfo();
        }
    }
}