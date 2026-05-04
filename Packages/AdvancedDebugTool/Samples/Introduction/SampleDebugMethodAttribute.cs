using AdvancedDebugTool;

public class SampleDebugMethodAttribute : DebugMethodBaseAttribute
{
    public SampleDebugMethodAttribute(string menuTitle, SampleCategory category = SampleCategory.General, int order = 0) : base(menuTitle, (int)category, order)
    {
    }
}