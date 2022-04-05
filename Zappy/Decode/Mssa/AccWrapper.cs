using Accessibility;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Zappy.Decode.Helper;
using Zappy.Decode.LogManager;
using Zappy.ExecuteTask.Helpers.Interface;
using Zappy.Properties;

namespace Zappy.Decode.Mssa
{
    internal class AccWrapper
    {
        private IAccessible accessibleObject;
        private int childId;
        private object[] children;
        private int childrenCount;
        private int currentChildIndex;
        private AccWrapper parent;
        private AccessibleRole? role;
        private IntPtr windowHandle;

        internal AccWrapper(IAccessible accessibleObject, int childId)
        {
            this.accessibleObject = accessibleObject;
            this.childId = childId;
        }

        internal AccWrapper(IntPtr windowHandle, IAccessible accessibleObject, int childId) : this(accessibleObject, childId)
        {
            WindowHandle = windowHandle;
        }

        internal void DoDefaultAction()
        {
            try
            {
                AccessibleObject.accDoDefaultAction(ChildId);
            }
            catch (SystemException exception)
            {
                MsaaUtility.MapAndThrowException(exception, true);
                throw;
            }
        }

        public override bool Equals(object obj)
        {
            if (this != obj)
            {
                AccWrapper wrapper = obj as AccWrapper;
                if (wrapper == null)
                {
                    return false;
                }
                if (ChildId != wrapper.ChildId)
                {
                    return false;
                }
                if (AccessibleObject == wrapper.AccessibleObject)
                {
                    return true;
                }
                if (RoleInt != wrapper.RoleInt)
                {
                    return false;
                }
                if (windowHandle != IntPtr.Zero && wrapper.windowHandle != IntPtr.Zero && RoleInt != AccessibleRole.MenuItem)
                {
                    if (windowHandle != wrapper.windowHandle)
                    {
                        return false;
                    }
                    if (!NativeMethods.IsWindow(windowHandle))
                    {
                        return true;
                    }
                }
                int childCount = -1;
                int num2 = -1;
                try
                {
                    childCount = ChildCount;
                }
                catch (Exception)
                {
                }
                try
                {
                    num2 = wrapper.ChildCount;
                }
                catch (Exception)
                {
                }
                if (childCount != num2)
                {
                    return false;
                }
                if (childCount != -1)
                {
                    int num3;
                    int num4;
                    int num5;
                    int num6;
                    string name = Name;
                    string b = wrapper.Name;
                    if (!string.Equals(name, b, StringComparison.Ordinal))
                    {
                        return false;
                    }
                    Rectangle objA = new Rectangle();
                    Rectangle objB = new Rectangle();
                    try
                    {
                        AccessibleObject.accLocation(out num3, out num4, out num5, out num6, ChildId);
                        objA = new Rectangle(num3, num4, num5, num6);
                    }
                    catch (SystemException)
                    {
                    }
                    try
                    {
                        wrapper.AccessibleObject.accLocation(out num3, out num4, out num5, out num6, wrapper.ChildId);
                        objB = new Rectangle(num3, num4, num5, num6);
                    }
                    catch (SystemException)
                    {
                    }
                    if (!Equals(objA, objB))
                    {
                        return false;
                    }
                }
            }
            return true;
        }


        internal static AccWrapper GetAccessibleFromObject(IAccessible parent, object childObject)
        {
            int childId = 0;
            IAccessible accessibleObject = childObject as IAccessible;
            if (accessibleObject == null && childObject is int)
            {
                object obj2 = null;
                try
                {
                    if ((int)childObject != 0)
                        obj2 = parent.get_accChild((int)childObject);
                }
                catch (Exception ex)
                {
                    CrapyLogger.log.Warn(ex);
                }
                if (obj2 is IAccessible)
                {
                    accessibleObject = (IAccessible)obj2;
                }
                else
                {
                    accessibleObject = parent;
                    childId = (int)childObject;
                }
            }
            return new AccWrapper(accessibleObject, childId);
        }

        internal static AccWrapper GetAccWrapperFromEvent(IntPtr windowHandle, int idObject, int idChild)
        {
            IAccessible accessibleObject = null;
            object childObject = null;
            if (NativeMethods.IsWindow(windowHandle) && NativeMethods.AccessibleObjectFromEvent(windowHandle, idObject, idChild, ref accessibleObject, ref childObject) == 0 && accessibleObject != null)
            {
                AccWrapper accessibleFromObject = GetAccessibleFromObject(accessibleObject, childObject);
                accessibleFromObject.WindowHandle = windowHandle;
                return accessibleFromObject;
            }
            return null;
        }

        internal static AccWrapper GetAccWrapperFromPoint(NativeMethods.POINT pt)
        {
            object[] args = { pt.x, pt.y };
            
            AccWrapper wrapper = null;
            try
            {
                object childId = 0;
                IAccessible pAcc = null;
                object[] objArray = { pt, pAcc, childId };
                int num = -1;
                object obj3 = MsaaZappyPlugin.Instance.InvokeDelegateOnStaThread(new GetAccessibleFromPointHandler(MsaaNativeMethods.AccessibleObjectFromPoint), objArray);
                if (obj3 is int)
                {
                    num = (int)obj3;
                }
                if (num == -2147417842 && Thread.CurrentThread.GetApartmentState() == ApartmentState.MTA)
                {
                    
                    num = MsaaNativeMethods.AccessibleObjectFromPoint(pt, ref pAcc, ref childId);
                }
                if (num == 0)
                {
                    pAcc = objArray[1] as IAccessible;
                    childId = objArray[2];
                }
                else
                {
                                        CrapyLogger.log.WarnFormat("Msaa.GetAccWrapperFromPoint: Failed: {0}", num);
                }
                if (pAcc == null)
                {
                    throw new ZappyTaskControlNotAvailableException(Resources.AccessDenied);
                }
                wrapper = GetElementFromChildObject(pAcc, childId, false);
            }
            catch (SystemException exception)
            {
                MsaaUtility.MapAndThrowException(exception, true);
                throw;
            }
            return wrapper;
        }

        internal static AccWrapper GetAccWrapperFromWindow(IntPtr windowHandle)
        {
                        
            AccWrapper wrapper = null;
            try
            {
                if (windowHandle != IntPtr.Zero && NativeMethods.IsWindow(windowHandle))
                {
                    object[] objArray2 = { windowHandle };
                    IAccessible accessibleObject = MsaaZappyPlugin.Instance.InvokeDelegateOnStaThread(new GetAccessibleFromWindowHandler(NativeMethods.AccessibleObjectFromWindow), objArray2) as IAccessible;
                    if (accessibleObject != null)
                    {
                        wrapper = new AccWrapper(windowHandle, accessibleObject, 0);
                    }
                    else
                    {
                        CrapyLogger.log.WarnFormat("Msaa.AccWrapperFromWindow: AccessibleObjectFromWindow returned null object");
                    }
                }
            }
            catch (SystemException exception)
            {
                MsaaUtility.MapAndThrowException(exception, true);
                throw;
            }
            if (wrapper == null)
            {
                throw new ZappyTaskControlNotAvailableException();
            }
            return wrapper;
        }

        internal Rectangle GetBoundingRectangle()
        {
            Rectangle rectangle;
            try
            {
                int pxLeft = -1;
                int pyTop = -1;
                int pcxWidth = -1;
                int pcyHeight = -1;
                AccessibleObject.accLocation(out pxLeft, out pyTop, out pcxWidth, out pcyHeight, ChildId);
                rectangle = new Rectangle(pxLeft, pyTop, pcxWidth, pcyHeight);
            }
            catch (SystemException)
            {
                                throw;
            }
            return rectangle;
        }

        internal static AccWrapper GetElementFromChildObject(IAccessible parentAcc, object childObj, bool ignoreInvisibleChild)
        {
            AccWrapper accessibleFromObject = GetAccessibleFromObject(parentAcc, childObj);
            if ((accessibleFromObject.ChildId != 0 || accessibleFromObject.windowHandle == IntPtr.Zero || NativeMethods.IsWindowResponding(accessibleFromObject.windowHandle)) && (!ignoreInvisibleChild || IsVisibleAndAvailableToUser(accessibleFromObject)))
            {
                return accessibleFromObject;
            }
            return null;
        }

        public override int GetHashCode() =>
            AccessibleObject.GetHashCode();

        internal AccWrapper GetNextChild(bool ignoreInvisibleChild)
        {
            AccWrapper wrapper = null;
            while (currentChildIndex < childrenCount)
            {
                try
                {
                    object obj2;
                    if (children != null && children[currentChildIndex] != null)
                    {
                        obj2 = children[currentChildIndex];
                    }
                    else
                    {
                        obj2 = currentChildIndex + 1;
                    }
                    wrapper = GetElementFromChildObject(AccessibleObject, obj2, ignoreInvisibleChild);
                }
                catch (COMException)
                {
                }
                catch (ZappyTaskControlNotAvailableException)
                {
                }
                currentChildIndex++;
                if (wrapper != null)
                {
                    wrapper.ContainerElement = ContainerElement;
                    return wrapper;
                }
            }
            return wrapper;
        }

        private static bool IsVisibleAndAvailableToUser(AccWrapper element)
        {
            AccessibleStates state = element.State;
            return (!MsaaUtility.HasState(state, AccessibleStates.Unavailable) || NativeMethods.IsWindow(element.WindowHandle)) && (!MsaaUtility.HasState(state, AccessibleStates.Invisible) || MsaaUtility.HasState(state, AccessibleStates.Offscreen));
        }


        internal AccWrapper Navigate(AccessibleNavigation navDir)
        {
            AccWrapper wrapper = null;
            try
            {
                object obj2 = AccessibleObject.accNavigate((int)navDir, ChildId);
                if (obj2 == null)
                {
                    return wrapper;
                }
                if (obj2 is int)
                {
                    return new AccWrapper(AccessibleObject, (int)obj2) { ContainerElement = ContainerElement };
                }
                IAccessible accessibleObject = obj2 as IAccessible;
                if (accessibleObject != null)
                {
                    wrapper = new AccWrapper(accessibleObject, 0)
                    {
                        ContainerElement = ContainerElement
                    };
                }
            }
            catch (SystemException exception)
            {
                object[] args = { exception.Message };
                
            }
            return wrapper;
        }

        internal void Select(int selectOption)
        {
            try
            {
                AccessibleObject.accSelect(selectOption, ChildId);
            }
            catch (SystemException exception)
            {
                MsaaUtility.MapAndThrowException(exception, true);
                throw;
            }
        }

        internal void UpdateChildren()
        {
            int pcObtained = 0;
            currentChildIndex = 0;
            if (ChildId != 0)
            {
                childrenCount = 0;
            }
            else
            {
                childrenCount = accessibleObject.accChildCount;
                children = new object[childrenCount];
                if (MsaaNativeMethods.AccessibleChildren(AccessibleObject, 0, childrenCount, children, out pcObtained) != 0 && childrenCount != pcObtained)
                {
                    children = null;
                }
            }
        }

        internal string AccessibleDescription
        {
            get
            {
                string str = string.Empty;
                try
                {
                    str = AccessibleObject.get_accDescription(ChildId);
                }
                catch (SystemException exception)
                {
                    MsaaUtility.MapAndThrowException(exception, true);
                    throw;
                }
                return str;
            }
        }

        internal IAccessible AccessibleObject =>
            accessibleObject;

        internal int ChildCount
        {
            get
            {
                int accChildCount = 0;
                try
                {
                    accChildCount = AccessibleObject.accChildCount;
                }
                catch (SystemException exception)
                {
                    MsaaUtility.MapAndThrowException(exception, true);
                    throw;
                }
                return accChildCount;
            }
        }

        internal int ChildId =>
            childId;

        internal ITaskActivityElement ContainerElement { get; set; }


        internal string HelpText
        {
            get
            {
                string str = string.Empty;
                try
                {
                    str = AccessibleObject.get_accHelp(ChildId);
                }
                catch (SystemException)
                {
                }
                return str;
            }
        }

        internal string KeyboardShortcut
        {
            get
            {
                string str;
                try
                {
                    str = AccessibleObject.get_accKeyboardShortcut(ChildId);
                }
                catch (SystemException exception)
                {
                    MsaaUtility.MapAndThrowException(exception, true);
                    throw;
                }
                return str;
            }
        }


        internal string Name
        {
            get
            {
                string str = string.Empty;
                try
                {
                    str = AccessibleObject.get_accName(ChildId);
                    if (str != null)
                    {
                        int index = str.IndexOf('\0');
                        if (index != -1)
                        {
                            str = str.Substring(0, index);
                        }
                        return str.Replace("\r", string.Empty).Replace("\n", string.Empty);
                    }
                    str = string.Empty;
                }
                catch (SystemException)
                {
                }
                return str;
            }
        }


        internal AccWrapper Parent
        {
            get
            {
                if (parent == null)
                {
                    IAccessible accessibleObject = null;
                    if (ChildId == 0)
                    {
                        try
                        {
                            accessibleObject = (IAccessible)AccessibleObject.accParent;
                        }
                        catch (SystemException)
                        {
                        }
                    }
                    else
                    {
                        accessibleObject = AccessibleObject;
                    }
                    if (accessibleObject != null)
                    {
                        parent = new AccWrapper(accessibleObject, 0);
                    }
                }
                if (parent != null)
                {
                    parent.ContainerElement = ContainerElement;
                }
                return parent;
            }
        }


        internal AccessibleRole RoleInt
        {
            get
            {
                if (!this.role.HasValue)
                {
                    this.role = 0;
                    object obj2 = null;
                    try
                    {
                        if (!ReferenceEquals(AccessibleObject, null))
                            obj2 = AccessibleObject.get_accRole(ChildId);
                    }
                    catch (SystemException exception)
                    {
                        CrapyLogger.log.Warn(exception);
                    }
                    if (obj2 is int)
                    {
                        role = (AccessibleRole)obj2;
                    }
                    else
                    {
                        string role = obj2 as string;
                        if (role != null)
                        {
                            this.role = MsaaUtility.ConvertRoleToInteger(role);
                        }
                    }
                }
                return this.role.Value;
            }
        }


        internal AccWrapper Selection
        {
            get
            {
                object childId = null;
                try
                {
                    childId = AccessibleObject.accSelection;
                }
                catch (SystemException)
                {
                }
                if (childId is int)
                {
                    if ((int)childId == 0)
                    {
                        return this;
                    }
                    childId = MsaaUtility.GetChildIAccessible(AccessibleObject, childId);
                    if (childId is int)
                    {
                        return new AccWrapper(AccessibleObject, (int)childId);
                    }
                }
                IAccessible accessibleObject = null;
                try
                {
                    accessibleObject = childId as IAccessible;
                }
                catch (SystemException)
                {
                }
                if (accessibleObject != null)
                {
                    return new AccWrapper(accessibleObject, 0);
                }
                return this;
            }
        }

        internal AccessibleStates State
        {
            get
            {
                AccessibleStates states;
                try
                {
                    states = (AccessibleStates)AccessibleObject.get_accState(ChildId);
                }
                catch (SystemException exception)
                {
                    MsaaUtility.MapAndThrowException(exception, true);
                    return AccessibleStates.None;
                                                        }
                return states;
            }
        }

        internal string Value
        {
            get
            {
                string str = null;
                try
                {
                    str = AccessibleObject.get_accValue(ChildId);
                }
                catch (UnauthorizedAccessException)
                {
                    return null;
                }
                catch (SystemException exception)
                {
                    MsaaUtility.MapAndThrowException(exception, true);
                    throw;
                }
                return str;
            }
            set
            {
                try
                {
                    AccessibleObject.set_accValue(ChildId, value);
                }
                catch (SystemException exception)
                {
                    MsaaUtility.MapAndThrowException(exception, true);
                    throw;
                }
            }
        }

        internal IntPtr WindowHandle
        {
            get
            {
                if (windowHandle == IntPtr.Zero)
                {
                    WindowHandle = WindowHandleGetter.GetWindowHandle(this);
                }
                return windowHandle;
            }
            set
            {
                windowHandle = value;
            }
        }
    }
}