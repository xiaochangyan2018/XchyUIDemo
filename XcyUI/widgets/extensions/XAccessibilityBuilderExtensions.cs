using XcyUI.events;
using XcyUI.models;
using XcyUI.views;

namespace XcyUI.widgets.extensions
{
    public static class XAccessibilityBuilderExtensions
    {
        public static XViewBuilder Accessibility(this XViewBuilder builder, XAccessibilityRole role, string name = null)
        {
            return builder.AccessibilityRole(role).AccessibilityName(name);
        }

        public static XViewBuilder AccessibilityRole(this XViewBuilder builder, XAccessibilityRole role)
        {
            builder.View.Accessibility.Role = role;
            return builder;
        }

        public static XViewBuilder AccessibilityName(this XViewBuilder builder, string name)
        {
            if (name != null)
            {
                builder.View.Accessibility.Name = name;
            }
            return builder;
        }

        public static XViewBuilder AccessibilityDescription(this XViewBuilder builder, string description)
        {
            builder.View.Accessibility.Description = description;
            return builder;
        }

        public static XViewBuilder AccessibilityValue(this XViewBuilder builder, string value)
        {
            builder.View.Accessibility.Value = value;
            return builder;
        }

        public static XViewBuilder AccessibilityHint(this XViewBuilder builder, string hint)
        {
            builder.View.Accessibility.Hint = hint;
            return builder;
        }

        public static XViewBuilder AccessibilityLabelledBy(this XViewBuilder builder, XView labelView)
        {
            builder.View.Accessibility.LabelledBy = labelView;
            return builder;
        }

        public static XViewBuilder AccessibilityLabelledBy(this XViewBuilder builder, XViewBuilder labelBuilder)
        {
            return builder.AccessibilityLabelledBy(labelBuilder?.View);
        }

        public static XViewBuilder AccessibilityDescribedBy(this XViewBuilder builder, XView descriptionView)
        {
            builder.View.Accessibility.DescribedBy = descriptionView;
            return builder;
        }

        public static XViewBuilder AccessibilityDescribedBy(this XViewBuilder builder, XViewBuilder descriptionBuilder)
        {
            return builder.AccessibilityDescribedBy(descriptionBuilder?.View);
        }

        public static XViewBuilder AccessibilityHidden(this XViewBuilder builder, bool hidden = true)
        {
            builder.View.Accessibility.IsHidden = hidden;
            return builder;
        }

        public static XViewBuilder AccessibilityMergeDescendants(this XViewBuilder builder, bool merge = true)
        {
            builder.View.Accessibility.MergeDescendants = merge;
            return builder;
        }

        public static XViewBuilder AccessibilityEnabled(this XViewBuilder builder, bool? enabled)
        {
            builder.View.Accessibility.IsEnabled = enabled;
            return builder;
        }

        public static XViewBuilder AccessibilityChecked(this XViewBuilder builder, bool? isChecked)
        {
            builder.View.Accessibility.IsChecked = isChecked;
            return builder;
        }

        public static XViewBuilder AccessibilitySelected(this XViewBuilder builder, bool? isSelected)
        {
            builder.View.Accessibility.IsSelected = isSelected;
            return builder;
        }

        public static XViewBuilder AccessibilityExpanded(this XViewBuilder builder, bool? isExpanded)
        {
            builder.View.Accessibility.IsExpanded = isExpanded;
            return builder;
        }

        public static XViewBuilder AccessibilityPressed(this XViewBuilder builder, bool? isPressed)
        {
            builder.View.Accessibility.IsPressed = isPressed;
            return builder;
        }

        public static XViewBuilder AccessibilityReadOnly(this XViewBuilder builder, bool? isReadOnly)
        {
            builder.View.Accessibility.IsReadOnly = isReadOnly;
            return builder;
        }

        public static XViewBuilder AccessibilityRequired(this XViewBuilder builder, bool? isRequired)
        {
            builder.View.Accessibility.IsRequired = isRequired;
            return builder;
        }

        public static XViewBuilder AccessibilityInvalid(this XViewBuilder builder, bool? isInvalid)
        {
            builder.View.Accessibility.IsInvalid = isInvalid;
            return builder;
        }

        public static XViewBuilder AccessibilityMultiline(this XViewBuilder builder, bool? isMultiline)
        {
            builder.View.Accessibility.IsMultiline = isMultiline;
            return builder;
        }

        public static XViewBuilder AccessibilityPassword(this XViewBuilder builder, bool? isPassword)
        {
            builder.View.Accessibility.IsPassword = isPassword;
            return builder;
        }

        public static XViewBuilder AccessibilityHeadingLevel(this XViewBuilder builder, int level)
        {
            builder.View.Accessibility.Role = XAccessibilityRole.Heading;
            builder.View.Accessibility.HeadingLevel = level;
            return builder;
        }

        public static XViewBuilder AccessibilitySetPosition(this XViewBuilder builder, int position, int setSize)
        {
            builder.View.Accessibility.PositionInSet = position;
            builder.View.Accessibility.SetSize = setSize;
            return builder;
        }

        public static XViewBuilder AccessibilityLiveRegion(this XViewBuilder builder, XAccessibilityLiveRegion liveRegion)
        {
            builder.View.Accessibility.LiveRegion = liveRegion;
            return builder;
        }

        public static XViewBuilder TabIndex(this XViewBuilder builder, int tabIndex)
        {
            builder.View.Accessibility.TabIndex = tabIndex;
            return builder;
        }

        public static XViewBuilder RequestFocus(this XViewBuilder builder)
        {
            XEvent.SetFocus(builder.View);
            return builder;
        }
    }
}
