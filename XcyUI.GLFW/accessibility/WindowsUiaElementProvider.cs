using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using XcyUI.models;

namespace XcyUI.GLFW.accessibility
{
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    internal class WindowsUiaElementProvider :
        IRawElementProviderSimple,
        IRawElementProviderFragment,
        IInvokeProvider,
        IValueProvider,
        IToggleProvider,
        IExpandCollapseProvider,
        ISelectionItemProvider
    {
        private readonly WindowsUiaBridge bridge;
        private readonly int? runtimeId;

        protected WindowsUiaBridge Bridge => bridge;

        public WindowsUiaElementProvider(WindowsUiaBridge bridge, int? runtimeId)
        {
            this.bridge = bridge;
            this.runtimeId = runtimeId;
        }

        public UiaProviderOptions ProviderOptions => UiaProviderOptions.ServerSideProvider;

        public IRawElementProviderSimple HostRawElementProvider
        {
            get
            {
                return runtimeId.HasValue ? null : bridge.HostProvider;
            }
        }

        public UiaRect BoundingRectangle
        {
            get
            {
                var node = CurrentNode;
                return node == null ? new UiaRect() : bridge.ToScreenRect(node.Node.Bounds);
            }
        }

        public IRawElementProviderFragmentRoot FragmentRoot => bridge.RootProvider;

        public string Value
        {
            get
            {
                var node = CurrentNode;
                return node?.Node.Value ?? "";
            }
        }

        public bool IsReadOnly
        {
            get
            {
                var node = CurrentNode;
                return node?.Node.ReadOnly == true;
            }
        }

        public UiaToggleState ToggleState
        {
            get
            {
                var node = CurrentNode;
                if (node?.Node.Checked == true)
                {
                    return UiaToggleState.On;
                }
                return UiaToggleState.Off;
            }
        }

        public UiaExpandCollapseState ExpandCollapseState
        {
            get
            {
                var node = CurrentNode;
                if (node == null || !node.Node.Expanded.HasValue)
                {
                    return UiaExpandCollapseState.LeafNode;
                }
                return node.Node.Expanded == true ? UiaExpandCollapseState.Expanded : UiaExpandCollapseState.Collapsed;
            }
        }

        public bool IsSelected
        {
            get
            {
                var node = CurrentNode;
                return node?.Node.Selected == true || node?.Node.Checked == true;
            }
        }

        public IRawElementProviderSimple SelectionContainer
        {
            get
            {
                var parent = CurrentNode?.Parent;
                while (parent != null)
                {
                    if (parent.Node.Role == XAccessibilityRole.ListBox
                        || parent.Node.Role == XAccessibilityRole.ComboBox
                        || parent.Node.Role == XAccessibilityRole.Group)
                    {
                        return bridge.CreateProvider(parent);
                    }
                    parent = parent.Parent;
                }
                return null;
            }
        }

        private WindowsUiaSnapshotNode CurrentNode => bridge.Resolve(runtimeId);

        public object GetPatternProvider(int patternId)
        {
            var node = CurrentNode;
            if (node == null)
            {
                return null;
            }

            switch (patternId)
            {
                case UiaIds.InvokePatternId:
                    return IsInvokable(node) ? (IInvokeProvider)this : null;
                case UiaIds.ValuePatternId:
                    return IsValueProvider(node) ? (IValueProvider)this : null;
                case UiaIds.TogglePatternId:
                    return IsToggleProvider(node) ? (IToggleProvider)this : null;
                case UiaIds.ExpandCollapsePatternId:
                    return node.Node.Expanded.HasValue ? (IExpandCollapseProvider)this : null;
                case UiaIds.SelectionItemPatternId:
                    return IsSelectionItemProvider(node) ? (ISelectionItemProvider)this : null;
                default:
                    return null;
            }
        }

        public object GetPropertyValue(int propertyId)
        {
            var node = CurrentNode;
            if (node == null)
            {
                return null;
            }

            switch (propertyId)
            {
                case UiaIds.RuntimeIdPropertyId:
                    return GetRuntimeId();
                case UiaIds.BoundingRectanglePropertyId:
                    return BoundingRectangle;
                case UiaIds.ProcessIdPropertyId:
                    return Process.GetCurrentProcess().Id;
                case UiaIds.ControlTypePropertyId:
                    return WindowsUiaBridge.GetControlType(node.Node.Role);
                case UiaIds.LocalizedControlTypePropertyId:
                    return WindowsUiaBridge.GetLocalizedControlType(node.Node.Role);
                case UiaIds.NamePropertyId:
                    return node.Node.Name ?? "";
                case UiaIds.HasKeyboardFocusPropertyId:
                    return node.Node.Focused;
                case UiaIds.IsKeyboardFocusablePropertyId:
                    return node.Node.Focusable;
                case UiaIds.IsEnabledPropertyId:
                    return node.Node.Enabled;
                case UiaIds.AutomationIdPropertyId:
                    return "xcyui-" + node.RuntimeId;
                case UiaIds.ClassNamePropertyId:
                    return "XcyUI." + node.Node.Role;
                case UiaIds.HelpTextPropertyId:
                    return node.Node.Description ?? node.Node.Hint ?? "";
                case UiaIds.IsControlElementPropertyId:
                    return true;
                case UiaIds.IsContentElementPropertyId:
                    return !runtimeId.HasValue || node.Node.Role != XAccessibilityRole.Group;
                case UiaIds.IsPasswordPropertyId:
                    return node.Node.Password == true;
                case UiaIds.NativeWindowHandlePropertyId:
                    return runtimeId.HasValue ? 0 : unchecked((int)bridge.Hwnd.ToInt64());
                case UiaIds.IsOffscreenPropertyId:
                    return node.Node.Bounds.Width <= 0 || node.Node.Bounds.Height <= 0;
                case UiaIds.FrameworkIdPropertyId:
                    return "XcyUI";
                case UiaIds.IsRequiredForFormPropertyId:
                    return node.Node.Required == true;
                case UiaIds.ValueValuePropertyId:
                    return Value;
                case UiaIds.ValueIsReadOnlyPropertyId:
                    return IsReadOnly;
                case UiaIds.ExpandCollapseExpandCollapseStatePropertyId:
                    return ExpandCollapseState;
                case UiaIds.SelectionItemIsSelectedPropertyId:
                    return IsSelected;
                case UiaIds.ToggleToggleStatePropertyId:
                    return ToggleState;
                default:
                    return null;
            }
        }

        public IRawElementProviderFragment Navigate(UiaNavigateDirection direction)
        {
            var node = CurrentNode;
            if (node == null)
            {
                return null;
            }

            switch (direction)
            {
                case UiaNavigateDirection.Parent:
                    return node.Parent == null ? null : bridge.CreateProvider(node.Parent);
                case UiaNavigateDirection.FirstChild:
                    return node.Children.Count == 0 ? null : bridge.CreateProvider(node.Children[0]);
                case UiaNavigateDirection.LastChild:
                    return node.Children.Count == 0 ? null : bridge.CreateProvider(node.Children[node.Children.Count - 1]);
                case UiaNavigateDirection.NextSibling:
                    return GetSibling(node, 1);
                case UiaNavigateDirection.PreviousSibling:
                    return GetSibling(node, -1);
                default:
                    return null;
            }
        }

        public int[] GetRuntimeId()
        {
            var node = CurrentNode;
            if (node == null)
            {
                return null;
            }

            if (!runtimeId.HasValue)
            {
                return null;
            }

            return new[] { UiaIds.UiaAppendRuntimeId, node.RuntimeId };
        }

        public IRawElementProviderSimple[] GetEmbeddedFragmentRoots()
        {
            return null;
        }

        public void SetFocus()
        {
            bridge.SetFocus(CurrentNode);
        }

        public void Invoke()
        {
            bridge.Invoke(CurrentNode);
        }

        public void SetValue(string value)
        {
            bridge.SetValue(CurrentNode, value);
        }

        public void Toggle()
        {
            bridge.Invoke(CurrentNode);
        }

        public void Expand()
        {
            var node = CurrentNode;
            if (node?.Node.Expanded == false)
            {
                bridge.Invoke(node);
            }
        }

        public void Collapse()
        {
            var node = CurrentNode;
            if (node?.Node.Expanded == true)
            {
                bridge.Invoke(node);
            }
        }

        public void Select()
        {
            var node = CurrentNode;
            if (node?.Node.Selected != true && node?.Node.Checked != true)
            {
                bridge.Invoke(node);
            }
        }

        public void AddToSelection()
        {
            Select();
        }

        public void RemoveFromSelection()
        {
            var node = CurrentNode;
            if (node?.Node.Selected == true || node?.Node.Checked == true)
            {
                bridge.Invoke(node);
            }
        }

        private IRawElementProviderFragment GetSibling(WindowsUiaSnapshotNode node, int offset)
        {
            if (node.Parent == null)
            {
                return null;
            }

            var index = node.Parent.Children.IndexOf(node);
            var siblingIndex = index + offset;
            if (siblingIndex < 0 || siblingIndex >= node.Parent.Children.Count)
            {
                return null;
            }

            return bridge.CreateProvider(node.Parent.Children[siblingIndex]);
        }

        private static bool IsInvokable(WindowsUiaSnapshotNode node)
        {
            var role = node.Node.Role;
            return role == XAccessibilityRole.Button
                || role == XAccessibilityRole.Link
                || role == XAccessibilityRole.ComboBox
                || role == XAccessibilityRole.DatePicker
                || role == XAccessibilityRole.ListItem
                || role == XAccessibilityRole.MenuItem
                || role == XAccessibilityRole.Tab;
        }

        private static bool IsValueProvider(WindowsUiaSnapshotNode node)
        {
            return node.Node.Role == XAccessibilityRole.TextBox
                || node.Node.Role == XAccessibilityRole.TextArea;
        }

        private static bool IsToggleProvider(WindowsUiaSnapshotNode node)
        {
            return node.Node.Role == XAccessibilityRole.CheckBox
                || node.Node.Role == XAccessibilityRole.Switch;
        }

        private static bool IsSelectionItemProvider(WindowsUiaSnapshotNode node)
        {
            return node.Node.Role == XAccessibilityRole.RadioButton
                || node.Node.Role == XAccessibilityRole.ListItem
                || node.Node.Role == XAccessibilityRole.Tab;
        }
    }

    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    internal sealed class WindowsUiaRootElementProvider : WindowsUiaElementProvider, IRawElementProviderFragmentRoot
    {
        public WindowsUiaRootElementProvider(WindowsUiaBridge bridge)
            : base(bridge, null)
        {
        }

        public IRawElementProviderFragment ElementProviderFromPoint(double x, double y)
        {
            return Bridge.CreateProvider(Bridge.ElementFromPoint(x, y));
        }

        public IRawElementProviderFragment GetFocus()
        {
            return Bridge.CreateProvider(Bridge.FocusedElement());
        }
    }
}
