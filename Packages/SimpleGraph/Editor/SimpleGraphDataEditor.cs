using UnityEditor;
using UnityEngine;

namespace SimpleGraph.Editor
{
    [CustomEditor(typeof(SimpleGraphData), true)]
    public class SimpleGraphDataEditor : 
        #if ODIN_INSPECTOR
        UnityEditor.Editor // TMP place Odin Inspector class instead
        #else
        UnityEditor.Editor
        #endif
    {

        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Open"))
            {
                SimpleGraphEditorWindow.Open((SimpleGraphData)target);
            }
        }
    }
}
