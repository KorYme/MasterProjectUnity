using UnityEditor.ShortcutManagement;
using UnityEngine;

namespace GraphTool.Editor
{
    public static class GraphViewShortcut
    {
        [Shortcut("Generic Graph View/Save", typeof(GraphEditorWindow), KeyCode.S, ShortcutModifiers.Control)]
        public static void SaveGraph()
        {
            UnityEditor.EditorWindow.GetWindow<GraphEditorWindow>().SaveChanges();
        }
    }
}