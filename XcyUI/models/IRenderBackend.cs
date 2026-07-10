using System.Collections.Generic;
using XcyUI.navigation;
using XcyUI.views;

namespace XcyUI.models
{
    public interface IRenderBackend
    {
        void CreateSurface(int width, int height, object paramsData);
        void ResetSurface(int width, int height, object paramsData);
        void Render();
        void DispatchEvent(XView view, XEventInfo info);
        void DispatchEvent(XEventInfo info);
        void Layout(int width, int height);
        void Focus(bool foucus);
        void Open(XPage page);
        XAccessibilityNode GetAccessibilityTree();
        List<XAccessibilityIssue> AuditAccessibility();
        void Dispose();
    }
}
