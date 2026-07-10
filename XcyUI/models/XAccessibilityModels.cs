using System;
using System.Collections.Generic;
using System.Linq;
using XcyUI.events;
using XcyUI.expansions;
using XcyUI.views;

namespace XcyUI.models
{
    public enum XAccessibilityRole
    {
        None,
        Application,
        Window,
        Group,
        Text,
        Heading,
        Button,
        Link,
        Image,
        TextBox,
        TextArea,
        CheckBox,
        RadioButton,
        Switch,
        ComboBox,
        ListBox,
        ListItem,
        Menu,
        MenuItem,
        Dialog,
        Tooltip,
        ProgressBar,
        Slider,
        ScrollBar,
        Separator,
        Tab,
        TabPanel,
        DatePicker,
        Status,
        Alert
    }

    public enum XAccessibilityLiveRegion
    {
        Off,
        Polite,
        Assertive
    }

    public enum XAccessibilityIssueSeverity
    {
        Info,
        Warning,
        Error
    }

    public class XAccessibilityParams
    {
        public XAccessibilityRole Role { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Value { get; set; }
        public string Hint { get; set; }
        public XView LabelledBy { get; set; }
        public XView DescribedBy { get; set; }
        public bool IsHidden { get; set; }
        public bool MergeDescendants { get; set; }
        public bool? IsEnabled { get; set; }
        public bool? IsChecked { get; set; }
        public bool? IsSelected { get; set; }
        public bool? IsExpanded { get; set; }
        public bool? IsPressed { get; set; }
        public bool? IsReadOnly { get; set; }
        public bool? IsRequired { get; set; }
        public bool? IsInvalid { get; set; }
        public bool? IsMultiline { get; set; }
        public bool? IsPassword { get; set; }
        public int HeadingLevel { get; set; }
        public int PositionInSet { get; set; }
        public int SetSize { get; set; }
        public int TabIndex { get; set; }
        public XAccessibilityLiveRegion LiveRegion { get; set; }

        public XAccessibilityParams()
        {
            Reset();
        }

        public void Reset()
        {
            Role = XAccessibilityRole.None;
            Name = null;
            Description = null;
            Value = null;
            Hint = null;
            LabelledBy = null;
            DescribedBy = null;
            IsHidden = false;
            MergeDescendants = false;
            IsEnabled = null;
            IsChecked = null;
            IsSelected = null;
            IsExpanded = null;
            IsPressed = null;
            IsReadOnly = null;
            IsRequired = null;
            IsInvalid = null;
            IsMultiline = null;
            IsPassword = null;
            HeadingLevel = 0;
            PositionInSet = 0;
            SetSize = 0;
            TabIndex = 0;
            LiveRegion = XAccessibilityLiveRegion.Off;
        }
    }

    public class XAccessibilityNode
    {
        public XView View { get; set; }
        public int Key { get; set; }
        public XAccessibilityRole Role { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Value { get; set; }
        public string Hint { get; set; }
        public XRect Bounds { get; set; }
        public bool Enabled { get; set; }
        public bool Focusable { get; set; }
        public bool Focused { get; set; }
        public bool Hidden { get; set; }
        public bool? Checked { get; set; }
        public bool? Selected { get; set; }
        public bool? Expanded { get; set; }
        public bool? Pressed { get; set; }
        public bool? ReadOnly { get; set; }
        public bool? Required { get; set; }
        public bool? Invalid { get; set; }
        public bool? Multiline { get; set; }
        public bool? Password { get; set; }
        public int HeadingLevel { get; set; }
        public int PositionInSet { get; set; }
        public int SetSize { get; set; }
        public XAccessibilityLiveRegion LiveRegion { get; set; }
        public List<XAccessibilityNode> Children { get; private set; }

        public XAccessibilityNode()
        {
            Children = new List<XAccessibilityNode>();
        }
    }

    public class XAccessibilityIssue
    {
        public XView View { get; set; }
        public XAccessibilityIssueSeverity Severity { get; set; }
        public string Message { get; set; }
    }

    public static class XAccessibility
    {
        public static event Action<XView> StructureChanged;

        public static void NotifyStructureChanged(XView container)
        {
            StructureChanged?.Invoke(container);
        }

        public static XAccessibilityNode BuildTree(XView root)
        {
            if (root == null)
            {
                return null;
            }

            var nodes = BuildNodes(root, true);
            if (nodes.Count == 0)
            {
                return null;
            }

            if (nodes.Count == 1)
            {
                return nodes[0];
            }

            var node = CreateNode(root, nodes);
            node.Role = XAccessibilityRole.Group;
            return node;
        }

        public static List<XView> GetFocusableViews(XView root)
        {
            var views = new List<XView>();
            AddFocusableViews(root, views);
            var positiveTabIndex = views.Where(n => n.Accessibility.TabIndex > 0)
                .OrderBy(n => n.Accessibility.TabIndex)
                .ToList();
            positiveTabIndex.AddRange(views.Where(n => n.Accessibility.TabIndex == 0));
            return positiveTabIndex;
        }

        public static bool IsKeyboardFocusable(XView view)
        {
            if (view == null)
            {
                return false;
            }

            return IsVisibleToAccessibility(view)
                && IsEnabled(view)
                && view.EventParams.Focusable
                && view.Accessibility.TabIndex >= 0;
        }

        public static bool IsKeyboardActivatable(XView view)
        {
            if (view == null || !IsEnabled(view) || !IsVisibleToAccessibility(view))
            {
                return false;
            }

            var role = GetRole(view);
            if (role == XAccessibilityRole.TextBox || role == XAccessibilityRole.TextArea)
            {
                return false;
            }

            return view.EventParams.Contains(XEventType.Click);
        }

        public static bool IsEnabled(XView view)
        {
            if (view == null)
            {
                return false;
            }

            return view.Accessibility.IsEnabled ?? view.EventParams.Enable;
        }

        public static bool IsVisibleToAccessibility(XView view)
        {
            return view != null
                && !view.Accessibility.IsHidden
                && view.LayoutParams.Visible == XVisibleType.Visible;
        }

        public static bool IsInteractive(XView view)
        {
            if (view == null)
            {
                return false;
            }

            var role = GetRole(view);
            return view.EventParams.Focusable
                || view.EventParams.Contains(XEventType.Click)
                || role == XAccessibilityRole.Button
                || role == XAccessibilityRole.Link
                || role == XAccessibilityRole.CheckBox
                || role == XAccessibilityRole.RadioButton
                || role == XAccessibilityRole.Switch
                || role == XAccessibilityRole.ComboBox
                || role == XAccessibilityRole.TextBox
                || role == XAccessibilityRole.TextArea
                || role == XAccessibilityRole.DatePicker
                || role == XAccessibilityRole.Slider;
        }

        public static XAccessibilityRole GetRole(XView view)
        {
            if (view == null)
            {
                return XAccessibilityRole.None;
            }

            if (view.Accessibility.Role != XAccessibilityRole.None)
            {
                return view.Accessibility.Role;
            }

            var input = view as XInput;
            if (input != null)
            {
                return input.Lines == 1 ? XAccessibilityRole.TextBox : XAccessibilityRole.TextArea;
            }

            if (view.EventParams.Contains(XEventType.Click))
            {
                return XAccessibilityRole.Button;
            }

            if (view is XText)
            {
                return XAccessibilityRole.Text;
            }

            if (view is XGroup)
            {
                return XAccessibilityRole.Group;
            }

            return XAccessibilityRole.None;
        }

        public static string GetAccessibleName(XView view)
        {
            if (view == null)
            {
                return "";
            }

            var accessibility = view.Accessibility;
            if (!string.IsNullOrWhiteSpace(accessibility.Name))
            {
                return accessibility.Name;
            }

            if (accessibility.LabelledBy != null)
            {
                return GetText(accessibility.LabelledBy);
            }

            var input = view as XInput;
            if (input != null && !string.IsNullOrWhiteSpace(input.Hint))
            {
                return input.Hint;
            }

            var text = view as XText;
            if (text != null && !string.IsNullOrWhiteSpace(text.Text))
            {
                return text.Text;
            }

            if (accessibility.MergeDescendants || IsInteractive(view))
            {
                return GetDescendantText(view);
            }

            return "";
        }

        public static string GetAccessibleDescription(XView view)
        {
            if (view == null)
            {
                return "";
            }

            if (!string.IsNullOrWhiteSpace(view.Accessibility.Description))
            {
                return view.Accessibility.Description;
            }

            if (view.Accessibility.DescribedBy != null)
            {
                return GetText(view.Accessibility.DescribedBy);
            }

            return "";
        }

        public static string GetAccessibleValue(XView view)
        {
            if (view == null)
            {
                return "";
            }

            if (!string.IsNullOrWhiteSpace(view.Accessibility.Value))
            {
                return view.Accessibility.Value;
            }

            var input = view as XInput;
            if (input != null)
            {
                return input.Text;
            }

            return "";
        }

        public static List<XAccessibilityIssue> Audit(XView root)
        {
            var issues = new List<XAccessibilityIssue>();
            Audit(root, issues);
            return issues;
        }

        private static void Audit(XView view, List<XAccessibilityIssue> issues)
        {
            if (view == null)
            {
                return;
            }

            if (IsVisibleToAccessibility(view))
            {
                var role = GetRole(view);
                var isInteractive = IsInteractive(view);
                var name = GetAccessibleName(view);
                if (isInteractive && string.IsNullOrWhiteSpace(name))
                {
                    issues.Add(new XAccessibilityIssue()
                    {
                        View = view,
                        Severity = XAccessibilityIssueSeverity.Warning,
                        Message = "Interactive views must have an accessible name."
                    });
                }

                if (view.EventParams.Focusable && !IsEnabled(view))
                {
                    issues.Add(new XAccessibilityIssue()
                    {
                        View = view,
                        Severity = XAccessibilityIssueSeverity.Warning,
                        Message = "Disabled views should not remain keyboard focusable."
                    });
                }

                if (role == XAccessibilityRole.Heading && view.Accessibility.HeadingLevel <= 0)
                {
                    issues.Add(new XAccessibilityIssue()
                    {
                        View = view,
                        Severity = XAccessibilityIssueSeverity.Info,
                        Message = "Heading views should include a heading level."
                    });
                }
            }

            for (var i = 0; i < view.ChildCount(); i++)
            {
                Audit(view.ChildElemnt(i), issues);
            }
        }

        private static List<XAccessibilityNode> BuildNodes(XView view, bool forceInclude)
        {
            var result = new List<XAccessibilityNode>();
            if (!IsVisibleToAccessibility(view))
            {
                return result;
            }

            var children = new List<XAccessibilityNode>();
            if (!view.Accessibility.MergeDescendants)
            {
                for (var i = 0; i < view.ChildCount(); i++)
                {
                    children.AddRange(BuildNodes(view.ChildElemnt(i), false));
                }
            }

            if (forceInclude || ShouldInclude(view))
            {
                result.Add(CreateNode(view, children));
            }
            else
            {
                result.AddRange(children);
            }

            return result;
        }

        private static XAccessibilityNode CreateNode(XView view, List<XAccessibilityNode> children)
        {
            var accessibility = view.Accessibility;
            var node = new XAccessibilityNode()
            {
                View = view,
                Key = view.Key,
                Role = GetRole(view),
                Name = GetAccessibleName(view),
                Description = GetAccessibleDescription(view),
                Value = GetAccessibleValue(view),
                Hint = accessibility.Hint,
                Bounds = view.RenderRect,
                Enabled = IsEnabled(view),
                Focusable = IsKeyboardFocusable(view),
                Focused = view == XEvent.FocusView,
                Hidden = accessibility.IsHidden,
                Checked = accessibility.IsChecked,
                Selected = accessibility.IsSelected,
                Expanded = accessibility.IsExpanded,
                Pressed = accessibility.IsPressed,
                ReadOnly = accessibility.IsReadOnly,
                Required = accessibility.IsRequired,
                Invalid = accessibility.IsInvalid,
                Multiline = accessibility.IsMultiline,
                Password = accessibility.IsPassword,
                HeadingLevel = accessibility.HeadingLevel,
                PositionInSet = accessibility.PositionInSet,
                SetSize = accessibility.SetSize,
                LiveRegion = accessibility.LiveRegion
            };
            node.Children.AddRange(children);
            return node;
        }

        private static bool ShouldInclude(XView view)
        {
            var role = GetRole(view);
            if (role == XAccessibilityRole.None)
            {
                return HasAccessibilityState(view) || IsInteractive(view);
            }

            if (role == XAccessibilityRole.Group && !HasAccessibilityState(view) && !IsInteractive(view))
            {
                return false;
            }

            if (view is XText && string.IsNullOrWhiteSpace(((XText)view).Text) && !HasAccessibilityState(view))
            {
                return false;
            }

            if (view is XIcon && string.IsNullOrWhiteSpace(view.Accessibility.Name) && !IsInteractive(view))
            {
                return false;
            }

            return true;
        }

        private static bool HasAccessibilityState(XView view)
        {
            var accessibility = view.Accessibility;
            return !string.IsNullOrWhiteSpace(accessibility.Name)
                || !string.IsNullOrWhiteSpace(accessibility.Description)
                || !string.IsNullOrWhiteSpace(accessibility.Value)
                || !string.IsNullOrWhiteSpace(accessibility.Hint)
                || accessibility.LabelledBy != null
                || accessibility.DescribedBy != null
                || accessibility.IsEnabled.HasValue
                || accessibility.IsChecked.HasValue
                || accessibility.IsSelected.HasValue
                || accessibility.IsExpanded.HasValue
                || accessibility.IsPressed.HasValue
                || accessibility.IsReadOnly.HasValue
                || accessibility.IsRequired.HasValue
                || accessibility.IsInvalid.HasValue
                || accessibility.IsMultiline.HasValue
                || accessibility.IsPassword.HasValue
                || accessibility.HeadingLevel > 0
                || accessibility.PositionInSet > 0
                || accessibility.SetSize > 0
                || accessibility.LiveRegion != XAccessibilityLiveRegion.Off;
        }

        private static void AddFocusableViews(XView view, List<XView> views)
        {
            if (view == null || !IsVisibleToAccessibility(view))
            {
                return;
            }

            if (IsKeyboardFocusable(view))
            {
                views.Add(view);
            }

            for (var i = 0; i < view.ChildCount(); i++)
            {
                AddFocusableViews(view.ChildElemnt(i), views);
            }
        }

        private static string GetText(XView view)
        {
            if (view == null || !IsVisibleToAccessibility(view))
            {
                return "";
            }

            var text = view as XText;
            if (text != null)
            {
                return text.Text ?? "";
            }

            return GetDescendantText(view);
        }

        private static string GetDescendantText(XView view)
        {
            var parts = new List<string>();
            AddDescendantText(view, parts);
            return string.Join(" ", parts.Where(n => !string.IsNullOrWhiteSpace(n)).ToArray()).Trim();
        }

        private static void AddDescendantText(XView view, List<string> parts)
        {
            if (view == null || !IsVisibleToAccessibility(view))
            {
                return;
            }

            for (var i = 0; i < view.ChildCount(); i++)
            {
                var child = view.ChildElemnt(i);
                var text = child as XText;
                if (text != null)
                {
                    if (!string.IsNullOrWhiteSpace(text.Text))
                    {
                        parts.Add(text.Text);
                    }
                    continue;
                }

                AddDescendantText(child, parts);
            }
        }
    }
}
