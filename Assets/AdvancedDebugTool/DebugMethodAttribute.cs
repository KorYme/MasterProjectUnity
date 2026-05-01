using System;

namespace AdvancedDebugTool
{
    [JetBrains.Annotations.MeansImplicitUse]
    public class DebugMethodAttribute : Attribute
    {
        public DebugMethodAttribute(string menuTitle, DebugCategory category = DebugCategory.Other, int order = 0)
        {
            MenuTitle = menuTitle;
            Category = category;
            Order = order;
        }
        
        public string MenuTitle { get; }
        public DebugCategory Category { get; }
        public int Order { get; }
    }
}