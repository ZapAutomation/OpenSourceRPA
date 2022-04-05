using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using Zappy.Decode.LogManager;
using Zappy.Decode.Mssa;

namespace Zappy.Decode.Helper
{
    internal static class WindowHandleGetter
    {
        private static IntPtr GetWindowFromAccessible(AccWrapper accWrapper) =>
            NativeMethods.WindowFromAccessibleObject(accWrapper.AccessibleObject);

        public static IntPtr GetWindowHandle(AccWrapper accWrapper)
        {
            int traversedLevel = -1;
            IntPtr zero = IntPtr.Zero;
            bool flag = true;
            AccWrapper parent = accWrapper;
            if (accWrapper.ChildId != 0)
            {
                parent = new AccWrapper(accWrapper.AccessibleObject, 0);
            }
            while (parent != null && traversedLevel++ < 100)
            {
                if (Thread.CurrentThread.GetApartmentState() != ApartmentState.STA)
                {
                    object[] objArray1 = { parent };
                    object obj2 = MsaaZappyPlugin.Instance.InvokeDelegateOnStaThread(new GetWindowFromAccessibleHandler(GetWindowFromAccessible), objArray1);
                    if (obj2 is IntPtr)
                    {
                        zero = (IntPtr)obj2;
                        flag = true;
                    }
                }
                if (!NativeMethods.IsWindow(zero))
                {
                    zero = GetWindowFromAccessible(parent);
                    flag = false;
                }
                if (NativeMethods.IsWindow(zero))
                {
                    break;
                }
                parent = parent.Parent;
            }
            if (!NativeMethods.IsWindow(zero) || IsNotValidWindowHandle(zero, traversedLevel))
            {
                object[] objArray2 = { zero };
                
                zero = GetWindowHandleUsingPoint(accWrapper);
                if (!NativeMethods.IsWindow(zero))
                {
                    object[] objArray3 = { zero };
                    CrapyLogger.log.WarnFormat("Msaa:WindowHandleGetter: Got invalid window handle [{0}] even after using WindowFromPoint", objArray3);
                    return IntPtr.Zero;
                }
                object[] objArray4 = { zero };
                
                return zero;
            }
            object[] args = { flag ? "STA" : "MTA", zero };
            
            return zero;
        }

        public static IntPtr GetWindowHandleUsingPoint(AccWrapper accWrapper)
        {
            Rectangle invalidRectangle = MsaaUtility.InvalidRectangle;
            try
            {
                invalidRectangle = accWrapper.GetBoundingRectangle();
            }
            catch (ZappyTaskException)
            {
                return IntPtr.Zero;
            }
            NativeMethods.POINT[] pointArray = new NativeMethods.POINT[3];
            pointArray[0].x = invalidRectangle.X + 2;
            pointArray[0].y = invalidRectangle.Y + 2;
            pointArray[1].x = invalidRectangle.X + invalidRectangle.Width - 2;
            pointArray[1].y = invalidRectangle.Y + invalidRectangle.Height - 2;
            pointArray[2].x = invalidRectangle.X + invalidRectangle.Width / 2;
            pointArray[2].y = invalidRectangle.Y + invalidRectangle.Height / 2;
            for (int i = 0; i < pointArray.Length; i++)
            {
                if (IsElementAcceptableForWindowFromPoint(accWrapper) || IsElementFromPoint(accWrapper, pointArray[i]))
                {
                    object[] args = { i };
                    
                    return GetWindowHandleUsingPoint(pointArray[i]);
                }
            }
            return IntPtr.Zero;
        }

        private static IntPtr GetWindowHandleUsingPoint(NativeMethods.POINT pt)
        {
            object[] args = { pt.x, pt.y };
            
            IntPtr zero = IntPtr.Zero;
            IntPtr hwnd = NativeMethods.WindowFromPoint(pt);
            IntPtr ancestor = NativeMethods.GetAncestor(hwnd, NativeMethods.GetAncestorFlag.GA_PARENT);
            NativeMethods.RECT lpRect = new NativeMethods.RECT();
            if (ancestor != IntPtr.Zero && NativeMethods.GetWindowRect(ancestor, out lpRect))
            {
                pt.x -= lpRect.left;
                pt.y -= lpRect.top;
                zero = MsaaNativeMethods.ChildWindowFromPointEx(ancestor, pt, MsaaNativeMethods.ChildWindowFromPointParameter.CWP_ALL | MsaaNativeMethods.ChildWindowFromPointParameter.CWP_SKIPDISABLED | MsaaNativeMethods.ChildWindowFromPointParameter.CWP_SKIPINVISIBLE | MsaaNativeMethods.ChildWindowFromPointParameter.CWP_SKIPTRANSPARENT);
                if (zero != IntPtr.Zero && NativeMethods.GetWindowRect(zero, out lpRect) && (pt.y < lpRect.top || pt.y > lpRect.bottom || pt.x < lpRect.left || pt.x > lpRect.right))
                {
                    zero = IntPtr.Zero;
                }
                if (zero == ancestor)
                {
                    zero = IntPtr.Zero;
                }
            }
            if (zero == IntPtr.Zero)
            {
                zero = hwnd;
            }
            return zero;
        }

        private static bool IsElementAcceptableForWindowFromPoint(AccWrapper accWrapper)
        {
            try
            {
                return accWrapper.RoleInt == AccessibleRole.MenuPopup || accWrapper.RoleInt == AccessibleRole.List;
            }
            catch (ZappyTaskException)
            {
                return false;
            }
        }

        private static bool IsElementFromPoint(AccWrapper accWrapper, NativeMethods.POINT point)
        {
            AccWrapper accWrapperFromPoint = null;
            try
            {
                accWrapperFromPoint = MsaaUtility.FastAccessibleObjectFromPoint(point);
                if (accWrapperFromPoint == null)
                {
                    CrapyLogger.log.ErrorFormat("Msaa.IsElementFromPoint: FastAccessibleObjectFromPoint : Failed");
                    accWrapperFromPoint = AccWrapper.GetAccWrapperFromPoint(point);
                }
            }
            catch (ZappyTaskException)
            {
                return false;
            }
            if (accWrapperFromPoint != null && accWrapperFromPoint.ChildId != 0)
            {
                accWrapperFromPoint = new AccWrapper(accWrapperFromPoint.AccessibleObject, 0);
            }
            return accWrapper.Equals(accWrapperFromPoint);
        }

        private static bool IsNotValidWindowHandle(IntPtr windowHandle, int traversedLevel) =>
            traversedLevel > 0 && windowHandle == MsaaUtility.DesktopWindowHandle;

        private delegate IntPtr GetWindowFromAccessibleHandler(AccWrapper accWrapper);
    }
}