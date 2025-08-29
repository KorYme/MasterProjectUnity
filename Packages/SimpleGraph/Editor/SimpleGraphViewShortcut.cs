using UnityEditor.ShortcutManagement;
using UnityEngine;

namespace SimpleGraph.Editor
{
    public static class SimpleGraphViewShortcut
    {
        [Shortcut("Simple Graph View/Save Changes", typeof(SimpleGraphEditorWindow), KeyCode.S, ShortcutModifiers.Control)]
        public static void SaveGraph()
        {
            UnityEditor.EditorWindow.GetWindow<SimpleGraphEditorWindow>().SaveChanges();
        }
    }
}
