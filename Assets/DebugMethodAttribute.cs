using AdvancedDebugTool;

public enum DebugCategory
{
    General,
    Gameplay,
    Rendering,
    Other,
}

public class DebugMethodAttribute : DebugMethodBaseAttribute
{
    public DebugMethodAttribute(string menuTitle, DebugCategory category = DebugCategory.General, int order = 0) : base(menuTitle, (int)category, order)
    {
    }
}