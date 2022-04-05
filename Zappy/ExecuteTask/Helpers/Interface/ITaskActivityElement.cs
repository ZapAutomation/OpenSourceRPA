using System;
using System.Windows.Forms;
using Zappy.ActionMap.Enums;
using Zappy.ActionMap.HelperClasses;
using Zappy.ActionMap.Query;

namespace Zappy.ExecuteTask.Helpers.Interface
{
#if COMENABLED
    [ComImport, Guid("E235A067-FA97-4C38-9FA6-453D10B018A1"), ComVisible(true), InterfaceType(ComInterfaceType.InterfaceIsIUnknown), TypeLibType(TypeLibTypeFlags.FNonExtensible)]
#endif
    public interface ITaskActivityElement
    {
        int ChildIndex { get; }
        bool IsLeafNode { get; }
        object GetNativeControlType(NativeControlTypeKind nativeControlTypeKind);

        void GetBoundingRectangle(out int left, out int top, out int width, out int height);
        string ClassName { get; }

        void GetClickablePoint(out int pointX, out int pointY);
        bool Equals(ITaskActivityElement element);
        string ControlTypeName { get; }
        string Framework { get; }
        bool IsPassword { get; }
        string Name { get; }
        IntPtr WindowHandle { get; }

        IQueryElement QueryId { get; }
        AccessibleStates GetRequestedState(AccessibleStates requestedState);
        string Value { get; set; }
        object NativeElement { get; }
        void InvokeProgrammaticAction(ProgrammaticActionOption programmaticActionOption);

        void EnsureVisibleByScrolling(int pointX, int pointY, ref int outpointX, ref int outpointY);
        object GetPropertyValue(string propertyName);
        void SetFocus();
        void WaitForReady();
        string FriendlyName { get; }
        object GetOption(UITechnologyElementOption technologyElementOption);
        void SetOption(UITechnologyElementOption technologyElementOption, object optionValue);
        ITaskActivityElement SwitchingElement { get; set; }
        bool GetRightToLeftProperty(RightToLeftKind rightToLeftKind);
        bool InitializeProgrammaticScroll();
        void ScrollProgrammatically(ScrollDirection scrollDirection, ScrollAmount scrollAmount);
        int GetScrolledPercentage(ScrollDirection scrollDirection, ITaskActivityElement scrollElement);
        string GetQueryIdForRelatedElement(ZappyTaskElementKind relatedElement, object additionalInfo, out int maxDepth);
        bool IsTreeSwitchingRequired { get; }
    }
}
