using AdvancedDebugTool;
using UnityEditor;
using UnityEngine;

public class DebugToolEditorWindow : EditorWindow
{
    private DebugTool m_DebugTool;
    
    private void OnEnable()
    {
        m_DebugTool ??= new DebugTool();
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
        if (GUILayout.Button("Toggle Window"))
        {
            m_DebugTool.ToggleDisplay();
        }
        BeginWindows();
        m_DebugTool.DrawOnGUI();
        EndWindows();
    }

    [DebugMethod("Test 1 Editor Window")]
    private void Test1(DebugContext ctx)
    {
        ctx.DrawLabelBlock("Test réussi ?", "OUI");
    }
}
