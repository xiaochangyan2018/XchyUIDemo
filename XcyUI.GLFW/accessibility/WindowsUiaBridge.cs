using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using XcyUI.events;
using XcyUI.models;
using XcyUI.utils;
using XcyUI.views;
using static XcyUI.models.XFunctions;

namespace XcyUI.GLFW.accessibility
{
    internal sealed class WindowsUiaBridge : IDisposable
    {
        private const int GWLP_WNDPROC = -4;
        private const uint WM_GETOBJECT = 0x003D;

        private readonly IntPtr hwnd;
        private readonly Func<XAccessibilityNode> getAccessibilityTree;
        private readonly WindowsUiaRootElementProvider rootProvider;
        private readonly WndProc wndProc;
        private readonly Action<XFunction> dispatchToUiThread;
        private IntPtr previousWndProc;
        private IRawElementProviderSimple hostProvider;
        private bool disposed;

        internal delegate IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", EntryPoint = "GetWindowLongPtrW")]
        private static extern IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtrW")]
        private static extern IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll", EntryPoint = "CallWindowProcW")]
        private static extern IntPtr CallWindowProc(IntPtr lpPrevWndFunc, IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern bool ClientToScreen(IntPtr hWnd, ref POINT lpPoint);

        [DllImport("UIAutomationCore.dll")]
        private static extern IntPtr UiaReturnRawElementProvider(
            IntPtr hwnd,
            IntPtr wParam,
            IntPtr lParam,
            [MarshalAs(UnmanagedType.Interface)] IRawElementProviderSimple provider);

        [DllImport("UIAutomationCore.dll")]
        private static extern int UiaHostProviderFromHwnd(IntPtr hwnd, out IntPtr provider);

        [DllImport("UIAutomationCore.dll")]
        private static extern int UiaRaiseAutomationEvent(
            [MarshalAs(UnmanagedType.Interface)] IRawElementProviderSimple provider,
            int eventId);

        [DllImport("UIAutomationCore.dll")]
        private static extern int UiaRaiseStructureChangedEvent(
            [MarshalAs(UnmanagedType.Interface)] IRawElementProviderSimple provider,
            UiaStructureChangeType structureChangeType,
            IntPtr runtimeId,
            int runtimeIdLength);


        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int X;
            public int Y;
        }

        public WindowsUiaBridge(IntPtr hwnd, Func<XAccessibilityNode> getAccessibilityTree, Action<XFunction> dispatchToUiThread)
        {
            this.hwnd = hwnd;
            this.getAccessibilityTree = getAccessibilityTree;
            this.dispatchToUiThread = dispatchToUiThread;
            rootProvider = new WindowsUiaRootElementProvider(this);
            wndProc = WindowProc;
            previousWndProc = GetWindowLongPtr(hwnd, GWLP_WNDPROC);
            SetWindowLongPtr(hwnd, GWLP_WNDPROC, Marshal.GetFunctionPointerForDelegate(wndProc));
            XEvent.AccessibilityFocusChanged += OnAccessibilityFocusChanged;
            XAccessibility.StructureChanged += OnAccessibilityStructureChanged;
        }

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            try
            {
                // Release UIA's provider-event references before the HWND is destroyed.
                UiaReturnRawElementProvider(hwnd, IntPtr.Zero, IntPtr.Zero, null);
            }
            catch
            {
                // Cleanup must not prevent normal window teardown.
            }

            var current = GetWindowLongPtr(hwnd, GWLP_WNDPROC);
            var mine = Marshal.GetFunctionPointerForDelegate(wndProc);
            if (current == mine && previousWndProc != IntPtr.Zero)
            {
                SetWindowLongPtr(hwnd, GWLP_WNDPROC, previousWndProc);
            }

            XEvent.AccessibilityFocusChanged -= OnAccessibilityFocusChanged;
            XAccessibility.StructureChanged -= OnAccessibilityStructureChanged;
            disposed = true;
        }

        private IntPtr WindowProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
        {
            if (msg == WM_GETOBJECT && lParam.ToInt64() == UiaIds.UiaRootObjectId)
            {
                return UiaReturnRawElementProvider(hWnd, wParam, lParam, rootProvider);
            }

            return CallWindowProc(previousWndProc, hWnd, msg, wParam, lParam);
        }

        internal WindowsUiaSnapshotNode RootSnapshot
        {
            get
            {
                var tree = getAccessibilityTree?.Invoke();
                return tree == null ? null : WindowsUiaSnapshotNode.Create(tree, null);
            }
        }

        internal IntPtr Hwnd => hwnd;
        internal WindowsUiaRootElementProvider RootProvider => rootProvider;

        private void OnAccessibilityFocusChanged(XView view)
        {
            if (disposed || view == null)
            {
                return;
            }

            try
            {
                var node = RootSnapshot?.Flatten().FirstOrDefault(n => ReferenceEquals(n.Node.View, view));
                var provider = CreateProvider(node);
                if (provider != null)
                {
                    UiaRaiseAutomationEvent(provider, UiaIds.AutomationFocusChangedEventId);
                }
            }
            catch
            {
                // Accessibility notifications must not interrupt the UI event loop.
            }
        }

        private void OnAccessibilityStructureChanged(XView container)
        {
            if (disposed || container == null)
            {
                return;
            }

            try
            {
                var node = RootSnapshot?.Flatten().FirstOrDefault(n => ReferenceEquals(n.Node.View, container));
                var provider = CreateProvider(node);
                if (provider != null)
                {
                    UiaRaiseStructureChangedEvent(provider, UiaStructureChangeType.ChildrenInvalidated, IntPtr.Zero, 0);
                }
            }
            catch
            {
                // Accessibility notifications must not interrupt the UI event loop.
            }
        }

        internal IRawElementProviderSimple HostProvider
        {
            get
            {
                if (hostProvider != null)
                {
                    return hostProvider;
                }

                IntPtr provider;
                var hr = UiaHostProviderFromHwnd(hwnd, out provider);
                if (hr < 0 || provider == IntPtr.Zero)
                {
                    return null;
                }

                try
                {
                    hostProvider = Marshal.GetObjectForIUnknown(provider) as IRawElementProviderSimple;
                    return hostProvider;
                }
                finally
                {
                    Marshal.Release(provider);
                }
            }
        }

        internal WindowsUiaSnapshotNode Resolve(int? runtimeId)
        {
            var root = RootSnapshot;
            if (root == null)
            {
                return null;
            }

            if (!runtimeId.HasValue)
            {
                return root;
            }

            return root.Flatten().FirstOrDefault(n => n.RuntimeId == runtimeId.Value);
        }

        internal WindowsUiaElementProvider CreateProvider(WindowsUiaSnapshotNode node)
        {
            if (node == null)
            {
                return null;
            }

            return node.Parent == null ? rootProvider : new WindowsUiaElementProvider(this, node.RuntimeId);
        }

        internal UiaRect ToScreenRect(XRect rect)
        {
            var point = new POINT() { X = rect.X, Y = rect.Y };
            ClientToScreen(hwnd, ref point);
            return new UiaRect(point.X, point.Y, rect.Width, rect.Height);
        }

        internal WindowsUiaSnapshotNode ElementFromPoint(double screenX, double screenY)
        {
            var root = RootSnapshot;
            if (root == null)
            {
                return null;
            }

            WindowsUiaSnapshotNode result = null;
            foreach (var node in root.Flatten())
            {
                var rect = ToScreenRect(node.Node.Bounds);
                if (screenX >= rect.Left
                    && screenX <= rect.Left + rect.Width
                    && screenY >= rect.Top
                    && screenY <= rect.Top + rect.Height)
                {
                    result = node;
                }
            }
            return result ?? root;
        }

        internal WindowsUiaSnapshotNode FocusedElement()
        {
            var root = RootSnapshot;
            return root?.Flatten().FirstOrDefault(n => n.Node.Focused);
        }

        internal void SetFocus(WindowsUiaSnapshotNode node)
        {
            if (node?.Node.View == null)
            {
                return;
            }

            DispatchToUiThread(() => XEvent.SetFocus(node.Node.View));
        }

        internal void Invoke(WindowsUiaSnapshotNode node)
        {
            if (node?.Node.View == null)
            {
                return;
            }

            DispatchToUiThread(() => XEvent.Activate(node.Node.View));
        }

        internal void SetValue(WindowsUiaSnapshotNode node, string value)
        {
            var input = node?.Node.View as XInput;
            if (input == null || node.Node.ReadOnly == true)
            {
                return;
            }

            DispatchToUiThread(() =>
            {
                input.Text = value ?? "";
                input.StartLayout();
                input.Invalidate();
            });
        }

        private void DispatchToUiThread(XFunction action)
        {
            if (disposed || action == null)
            {
                return;
            }

            if (dispatchToUiThread != null)
            {
                dispatchToUiThread(action);
            }
            else
            {
                action.Invoke();
            }
        }

        internal static int GetControlType(XAccessibilityRole role)
        {
            switch (role)
            {
                case XAccessibilityRole.Button:
                    return UiaIds.ButtonControlTypeId;
                case XAccessibilityRole.Link:
                    return UiaIds.HyperlinkControlTypeId;
                case XAccessibilityRole.Image:
                    return UiaIds.ImageControlTypeId;
                case XAccessibilityRole.TextBox:
                case XAccessibilityRole.TextArea:
                    return UiaIds.EditControlTypeId;
                case XAccessibilityRole.CheckBox:
                    return UiaIds.CheckBoxControlTypeId;
                case XAccessibilityRole.RadioButton:
                    return UiaIds.RadioButtonControlTypeId;
                case XAccessibilityRole.Switch:
                    return UiaIds.ButtonControlTypeId;
                case XAccessibilityRole.ComboBox:
                case XAccessibilityRole.DatePicker:
                    return UiaIds.ComboBoxControlTypeId;
                case XAccessibilityRole.ListBox:
                    return UiaIds.ListControlTypeId;
                case XAccessibilityRole.ListItem:
                    return UiaIds.ListItemControlTypeId;
                case XAccessibilityRole.Menu:
                    return UiaIds.MenuControlTypeId;
                case XAccessibilityRole.MenuItem:
                    return UiaIds.MenuItemControlTypeId;
                case XAccessibilityRole.Dialog:
                case XAccessibilityRole.Window:
                    return UiaIds.WindowControlTypeId;
                case XAccessibilityRole.Tooltip:
                    return UiaIds.ToolTipControlTypeId;
                case XAccessibilityRole.ProgressBar:
                    return UiaIds.ProgressBarControlTypeId;
                case XAccessibilityRole.Slider:
                    return UiaIds.SliderControlTypeId;
                case XAccessibilityRole.ScrollBar:
                    return UiaIds.ScrollBarControlTypeId;
                case XAccessibilityRole.Separator:
                    return UiaIds.SeparatorControlTypeId;
                case XAccessibilityRole.Tab:
                    return UiaIds.TabItemControlTypeId;
                case XAccessibilityRole.TabPanel:
                    return UiaIds.TabControlTypeId;
                case XAccessibilityRole.Status:
                    return UiaIds.StatusBarControlTypeId;
                case XAccessibilityRole.Group:
                case XAccessibilityRole.Application:
                    return UiaIds.GroupControlTypeId;
                case XAccessibilityRole.Text:
                case XAccessibilityRole.Heading:
                default:
                    return UiaIds.TextControlTypeId;
            }
        }

        internal static string GetLocalizedControlType(XAccessibilityRole role)
        {
            return role.ToString();
        }
    }

    internal sealed class WindowsUiaSnapshotNode
    {
        public XAccessibilityNode Node { get; private set; }
        public WindowsUiaSnapshotNode Parent { get; private set; }
        public List<WindowsUiaSnapshotNode> Children { get; private set; }
        public int RuntimeId { get; private set; }

        private WindowsUiaSnapshotNode(XAccessibilityNode node, WindowsUiaSnapshotNode parent)
        {
            Node = node;
            Parent = parent;
            Children = new List<WindowsUiaSnapshotNode>();
            RuntimeId = node.View == null ? 1 : node.View.GetHashCode();
        }

        public static WindowsUiaSnapshotNode Create(XAccessibilityNode node, WindowsUiaSnapshotNode parent)
        {
            var snapshot = new WindowsUiaSnapshotNode(node, parent);
            foreach (var child in node.Children)
            {
                snapshot.Children.Add(Create(child, snapshot));
            }
            return snapshot;
        }

        public IEnumerable<WindowsUiaSnapshotNode> Flatten()
        {
            yield return this;
            foreach (var child in Children)
            {
                foreach (var item in child.Flatten())
                {
                    yield return item;
                }
            }
        }
    }
}
