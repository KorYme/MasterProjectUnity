using UnityEngine;

namespace ManagerInjection.Samples
{
    public class SampleScript : MonoBehaviour
    {
        private void Start()
        {
            SampleGlobals.TestManager.Test();
            PrintDebug();
        }

        public void PrintDebug()
        {
            Debug.Log("Warning : " + SampleGlobals.WarningManager?.GetWarningLevel);
            Debug.Log("Error : " + SampleGlobals.ErrorManager?.GetErrorLevel);
        }
    }
    
        
#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(SampleScript))]
    public class SampleScriptEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            SampleScript script = (SampleScript)target;
            if (GUILayout.Button("Test"))
            {
                script.PrintDebug();
            }
        }
    }
#endif
}
