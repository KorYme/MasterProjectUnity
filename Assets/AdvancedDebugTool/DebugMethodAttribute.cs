using System;

namespace AdvancedDebugTool
{
    [JetBrains.Annotations.MeansImplicitUse]
    public class DebugMethodAttribute : Attribute
    {
        public DebugMethodAttribute(string menuTitle)
        {
            MenuTitle = menuTitle;
        }
        
        public string MenuTitle { get; }
    }
}