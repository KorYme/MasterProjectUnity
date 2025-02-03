using UnityEditor;
using UnityEngine;

namespace MasterProject.SaveSystem.Editor
{
    [CustomEditor(typeof(DataSaveSystemGenerator))]
    public class DataSaveSystemGeneratorEditor : UnityEditor.Editor
    {
        private DataSaveSystemGenerator m_instance;

        private void OnEnable()
        {
            m_instance = (DataSaveSystemGenerator)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Generate SaveSystem Folder"))
            {
                m_instance.GenerateSaveSystemFolder();
            }
            if (GUILayout.Button("Generate GameData Class"))
            {
                m_instance.GenerateGameDataClass();
            }
            //if (GUILayout.Button("Attach DataSave Manager"))
            //{

            //}
        }
    }
}