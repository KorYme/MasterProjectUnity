using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace SaveSystem.Editor
{
    public class DataSaveSystemSettings : ScriptableObject
    {
        public const string DATA_SAVE_SYSTEM_SETTINGS_PATH = "Assets/DataSaveSystem/DataSaveSystemSettings.asset";

        [SerializeField] private string _generatedClassLocation;
        public string GeneratedClassLocation => _generatedClassLocation;
        
        internal static DataSaveSystemSettings GetOrCreateSettings()
        {
            DataSaveSystemSettings settings = AssetDatabase.LoadAssetAtPath<DataSaveSystemSettings>(DATA_SAVE_SYSTEM_SETTINGS_PATH);
            if (!settings)
            {
                settings = CreateInstance<DataSaveSystemSettings>();
                settings._generatedClassLocation = string.Empty;
                Directory.CreateDirectory(Path.GetDirectoryName(DATA_SAVE_SYSTEM_SETTINGS_PATH));
                AssetDatabase.CreateAsset(settings, DATA_SAVE_SYSTEM_SETTINGS_PATH);
                AssetDatabase.SaveAssets();
            }
            return settings;
        }

        internal static SerializedObject GetSerializedSettings()
        {
            return new SerializedObject(GetOrCreateSettings());
        }
    }

    public static class ToolbarExtenderSettingsIMGUIRegister
    {
#if false
        [SettingsProvider]
        public static SettingsProvider CreateDataSaveSystemSettingsProvider()
        {
            return new SettingsProvider("Project/Data Save System", SettingsScope.Project)
            {
                label = "Data Save System",
                guiHandler = _ =>
                {
                    SerializedObject settings = DataSaveSystemSettings.GetSerializedSettings();
                    EditorGUILayout.PropertyField(settings.FindProperty("_generatedClassLocation"), new GUIContent("Generated class location"));
                    settings.ApplyModifiedPropertiesWithoutUndo();
                },
                keywords = new HashSet<string>(new[] { "Data", "Save", "System" })
            };
        }
#endif
    }
}