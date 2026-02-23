using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CustomPlayButton 
{
    [FilePath("ProjectSettings/ToolbarExtenderSettings.asset", FilePathAttribute.Location.ProjectFolder)]
    public class ToolbarExtenderSettings : ScriptableSingleton<ToolbarExtenderSettings>
    {
        // TODO : Add new sorting options (such as only use build settings scene or not, type of sorting...)
        // NICE TO HAVE : Create a choose your folder button
        [SerializeField] private string _folderToFocus;
        
        public string FolderToFocus => _folderToFocus;

        public void SaveData()
        {
            Save(true);
        }
    }

    public static class ToolbarExtenderSettingsIMGUIRegister
    {
        [SettingsProvider]
        internal static SettingsProvider CreateToolbarExtenderSettingsProvider()
        {
            // First parameter is the path in the Settings window.
            // Second parameter is the scope of this setting: it only appears in the Project Settings window.
            SettingsProvider provider = new SettingsProvider("Project/Toolbar Extender", SettingsScope.Project)
            {
                // By default the last token of the path is used as display name if no label is provided.
                label = "Toolbar Extender",
                // Create the SettingsProvider and initialize its drawing (IMGUI) function in place:
                guiHandler = _ =>
                {
                    GUILayout.BeginVertical(new GUIStyle()
                    {
                        margin = new RectOffset(10, 10, 10, 10),
                    });
                    EditorGUI.BeginChangeCheck();
                    SerializedObject settings = new SerializedObject(ToolbarExtenderSettings.instance);
                    EditorGUILayout.PropertyField(settings.FindProperty("_folderToFocus"), new GUIContent("Folder to Focus"));
                    settings.ApplyModifiedPropertiesWithoutUndo();
                    if (EditorGUI.EndChangeCheck())
                    {
                        ToolbarExtenderSettings.instance.SaveData();
                    }
                    GUILayout.EndVertical();
                },

                // Populate the search keywords to enable smart search filtering and label highlighting:
                keywords = new HashSet<string>(new[] { "Toolbar", "Extender", "Scene" })
            };

            return provider;
        }
    }
}
