using KorYmeLibrary.DialogueSystem.Windows;
using UnityEditor.ShortcutManagement;
using UnityEditor;
using UnityEngine;

public static class DSGraphViewShortcut
{
    [Shortcut("Generic Graph View/Save", typeof(DSEditorWindow), KeyCode.S, ShortcutModifiers.Control)]
    public static void SaveGraph()
    {
        EditorWindow.GetWindow<DSEditorWindow>().SaveChanges();
    }
}
