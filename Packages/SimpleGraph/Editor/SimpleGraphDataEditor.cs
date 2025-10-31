using UnityEditor;
using UnityEditor.Callbacks;
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

        /// <summary>
        /// Open Graph when double clicking on the asset
        /// </summary>
        /// <param name="instanceID">Graph asset ID</param>
        /// <param name="index"></param>
        /// <returns></returns>
        [OnOpenAsset]
        public static bool OnOpenAsset(int instanceID)
        {
            Object asset = EditorUtility.InstanceIDToObject(instanceID);
            if (typeof(SimpleGraphData).IsAssignableFrom(asset.GetType()))
            {
                SimpleGraphEditorWindow.Open((SimpleGraphData)asset);
                return true;
            }
            
            return false;
        }

        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Open"))
            {
                SimpleGraphEditorWindow.Open((SimpleGraphData)target);
            }
        }
    }
}
