using System;

namespace AdvancedDebugTool
{
    #if JETBRAINS
    [JetBrains.Annotations.MeansImplicitUse]
    #endif
    public abstract class DebugMethodBaseAttribute : Attribute
    {
        public DebugMethodBaseAttribute(string menuTitle, int categoryValue, int order = 0)
        {
            MenuTitle = menuTitle;
            CategoryValue = categoryValue;
            Order = order;
        }
        
        internal string MenuTitle { get; }
        internal int CategoryValue { get; }
        internal int Order { get; }
    }
}