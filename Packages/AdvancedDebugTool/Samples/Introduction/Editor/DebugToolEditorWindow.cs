using AdvancedDebugTool;
using UnityEditor;

public class DebugToolEditorWindow : EditorWindow
{
    private DebugTool<SampleDebugMethodAttribute, SampleCategory> m_DebugTool;
    
    private void OnEnable()
    {
        m_DebugTool ??= new DebugTool<SampleDebugMethodAttribute, SampleCategory>();
        m_DebugTool.AddObjectToMenu(this);
        m_DebugTool.Show();
    }

    [MenuItem ("Window/My Window")]
    public static void ShowWindow () 
    {
        GetWindow<DebugToolEditorWindow>().Show();
    }
    
    private void OnGUI()
    {
        BeginWindows();
        m_DebugTool.DrawOnGUI();
        EndWindows();
    }

    [SampleDebugMethod("Introduction")]
    private void Test1(DebugContext ctx)
    {
        ctx.DrawLabelBlock("Even in editor ?!?", "YES, thanks to Unity IMGui API !");
    }
}
