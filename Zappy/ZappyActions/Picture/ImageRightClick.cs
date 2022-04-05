using System;
using System.Drawing;
using System.IO;
using System.Threading;
using Zappy.Decode.Helper;
using Zappy.Helpers;
using Zappy.SharedInterface.Helper;
using Point = System.Drawing.Point;

namespace Zappy.ZappyActions.Picture
{
    public class ImageRightClick : ImageClickHelper
    {
        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
                                                                                                                        
                                                
                                                                                                                                                                                                                                                                        
                                                                                                
            
                                                                                    
            Point point = GetPointOnImageToClick();

            NativeMethods.SetCursorPos(point.X, point.Y);
            NativeMethods.mouse_event(NativeMethods.MOUSEEVENTF_RIGHTDOWN | NativeMethods.MOUSEEVENTF_RIGHTUP, Convert.ToUInt32(point.X), Convert.ToUInt32(point.Y), 0, UIntPtr.Zero);
                    }
        public override string AuditInfo()
        {
            return base.AuditInfo();

        }

    }
}
