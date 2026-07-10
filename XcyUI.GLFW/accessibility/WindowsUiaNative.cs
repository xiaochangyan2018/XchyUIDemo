using System;
using System.Runtime.InteropServices;

namespace XcyUI.GLFW.accessibility
{
    [Flags]
    public enum UiaProviderOptions
    {
        ClientSideProvider = 1,
        ServerSideProvider = 2,
        NonClientAreaProvider = 4,
        OverrideProvider = 8,
        ProviderOwnsSetFocus = 16,
        UseComThreading = 32
    }

    public enum UiaNavigateDirection
    {
        Parent = 0,
        NextSibling = 1,
        PreviousSibling = 2,
        FirstChild = 3,
        LastChild = 4
    }

    public enum UiaToggleState
    {
        Off = 0,
        On = 1,
        Indeterminate = 2
    }

    public enum UiaExpandCollapseState
    {
        Collapsed = 0,
        Expanded = 1,
        PartiallyExpanded = 2,
        LeafNode = 3
    }

    public enum UiaStructureChangeType
    {
        ChildAdded = 0,
        ChildRemoved = 1,
        ChildrenInvalidated = 2,
        ChildrenBulkAdded = 3,
        ChildrenBulkRemoved = 4,
        ChildrenReordered = 5
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct UiaRect
    {
        public double Left;
        public double Top;
        public double Width;
        public double Height;

        public UiaRect(double left, double top, double width, double height)
        {
            Left = left;
            Top = top;
            Width = width;
            Height = height;
        }
    }

    [ComVisible(true)]
    [Guid("D6DD68D1-86FD-4332-8666-9ABEDEA2D24C")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IRawElementProviderSimple
    {
        UiaProviderOptions ProviderOptions { get; }
        [return: MarshalAs(UnmanagedType.IUnknown)]
        object GetPatternProvider(int patternId);

        object GetPropertyValue(int propertyId);
        IRawElementProviderSimple HostRawElementProvider { get; }
    }

    [ComVisible(true)]
    [Guid("F7063DA8-8359-439C-9297-BBC5299A7D87")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IRawElementProviderFragment : IRawElementProviderSimple
    {
        IRawElementProviderFragment Navigate(UiaNavigateDirection direction);

        int[] GetRuntimeId();

        UiaRect BoundingRectangle { get; }

        IRawElementProviderSimple[] GetEmbeddedFragmentRoots();

        void SetFocus();
        IRawElementProviderFragmentRoot FragmentRoot { get; }
    }

    [ComVisible(true)]
    [Guid("620CE2A5-AB8F-40A9-86CB-DE3C75599B58")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IRawElementProviderFragmentRoot : IRawElementProviderFragment
    {
        IRawElementProviderFragment ElementProviderFromPoint(double x, double y);
        IRawElementProviderFragment GetFocus();
    }

    [ComVisible(true)]
    [Guid("54FCB24B-E18E-47A2-B4D3-ECCBE77599A2")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IInvokeProvider
    {
        void Invoke();
    }

    [ComVisible(true)]
    [Guid("C7935180-6FB3-4201-B174-7DF73ADBF64A")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IValueProvider
    {
        void SetValue(string value);
        string Value { get; }
        bool IsReadOnly { get; }
    }

    [ComVisible(true)]
    [Guid("56D00BD0-C4F4-433C-A836-1A52A57E0892")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IToggleProvider
    {
        void Toggle();
        UiaToggleState ToggleState { get; }
    }

    [ComVisible(true)]
    [Guid("D847D3A5-CAB0-4A98-8C32-ECB45C59AD24")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IExpandCollapseProvider
    {
        void Expand();
        void Collapse();
        UiaExpandCollapseState ExpandCollapseState { get; }
    }

    [ComVisible(true)]
    [Guid("2ACAD808-B2D4-452D-A407-91FF1AD167B2")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ISelectionItemProvider
    {
        void Select();
        void AddToSelection();
        void RemoveFromSelection();
        bool IsSelected { get; }
        IRawElementProviderSimple SelectionContainer { get; }
    }

    internal static class UiaIds
    {
        public const int UiaRootObjectId = -25;
        public const int UiaAppendRuntimeId = 3;

        public const int StructureChangedEventId = 20002;
        public const int AutomationFocusChangedEventId = 20005;

        public const int InvokePatternId = 10000;
        public const int ValuePatternId = 10002;
        public const int ExpandCollapsePatternId = 10005;
        public const int SelectionItemPatternId = 10010;
        public const int TogglePatternId = 10015;

        public const int RuntimeIdPropertyId = 30000;
        public const int BoundingRectanglePropertyId = 30001;
        public const int ProcessIdPropertyId = 30002;
        public const int ControlTypePropertyId = 30003;
        public const int LocalizedControlTypePropertyId = 30004;
        public const int NamePropertyId = 30005;
        public const int HasKeyboardFocusPropertyId = 30008;
        public const int IsKeyboardFocusablePropertyId = 30009;
        public const int IsEnabledPropertyId = 30010;
        public const int AutomationIdPropertyId = 30011;
        public const int ClassNamePropertyId = 30012;
        public const int HelpTextPropertyId = 30013;
        public const int IsControlElementPropertyId = 30016;
        public const int IsContentElementPropertyId = 30017;
        public const int IsPasswordPropertyId = 30019;
        public const int NativeWindowHandlePropertyId = 30020;
        public const int IsOffscreenPropertyId = 30022;
        public const int FrameworkIdPropertyId = 30024;
        public const int IsRequiredForFormPropertyId = 30025;
        public const int ValueValuePropertyId = 30045;
        public const int ValueIsReadOnlyPropertyId = 30046;
        public const int ExpandCollapseExpandCollapseStatePropertyId = 30070;
        public const int SelectionItemIsSelectedPropertyId = 30079;
        public const int ToggleToggleStatePropertyId = 30086;

        public const int ButtonControlTypeId = 50000;
        public const int CalendarControlTypeId = 50001;
        public const int CheckBoxControlTypeId = 50002;
        public const int ComboBoxControlTypeId = 50003;
        public const int EditControlTypeId = 50004;
        public const int HyperlinkControlTypeId = 50005;
        public const int ImageControlTypeId = 50006;
        public const int ListItemControlTypeId = 50007;
        public const int ListControlTypeId = 50008;
        public const int MenuControlTypeId = 50009;
        public const int MenuItemControlTypeId = 50011;
        public const int ProgressBarControlTypeId = 50012;
        public const int RadioButtonControlTypeId = 50013;
        public const int ScrollBarControlTypeId = 50014;
        public const int SliderControlTypeId = 50015;
        public const int StatusBarControlTypeId = 50017;
        public const int TabControlTypeId = 50018;
        public const int TabItemControlTypeId = 50019;
        public const int TextControlTypeId = 50020;
        public const int ToolTipControlTypeId = 50022;
        public const int CustomControlTypeId = 50025;
        public const int GroupControlTypeId = 50026;
        public const int WindowControlTypeId = 50032;
        public const int PaneControlTypeId = 50033;
        public const int SeparatorControlTypeId = 50038;
    }
}
