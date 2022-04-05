using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using Zappy.Decode.Helper;
using Zappy.Helpers;
using Zappy.SharedInterface.Helper;
using Point = System.Drawing.Point;

namespace Zappy.ZappyActions.Picture
{
    
    public class ImageClick : ImageClickHelper
    {

        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            /*if (!context.ContextData.TryGetValue(CrapyConstants.SikuliSessionString, out SikuliSession))
            {
                context.ContextData[CrapyConstants.SikuliSessionString] = CreateSikuliSession();
            }
            ISikuliSession session = SikuliSession as ISikuliSession;*/

                        
                        
            
                                                                                                                                                                                                                                                                                    
                                                                                                
                                                                                                
            Point point = GetPointOnImageToClick();

            NativeMethods.SetCursorPos(point.X, point.Y);
            NativeMethods.mouse_event(NativeMethods.MOUSEEVENTF_LEFTDOWN | NativeMethods.MOUSEEVENTF_LEFTUP, Convert.ToUInt32(point.X), Convert.ToUInt32(point.Y), 0, UIntPtr.Zero);
                    }

        public override string AuditInfo()
        {
            return base.AuditInfo();
        }
    }
}