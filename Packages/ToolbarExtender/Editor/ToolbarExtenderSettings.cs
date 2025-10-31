using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class ToolbarExtenderSettings : ScriptableObject
{
    public const string TOOLBAR_EXTENDER_SETTINGS_PATH = "Assets/ToolbarExtender/ToolbarExtenderSettings.asset";

    [SerializeField] private string folderToFocus;
    public string FolderToFocus => folderToFocus;
    
    internal static ToolbarExtenderSettings GetOrCreateSettings()
    {
        ToolbarExtenderSettings settings = AssetDatabase.LoadAssetAtPath<ToolbarExtenderSettings>(TOOLBAR_EXTENDER_SETTINGS_PATH);
        if (!settings)
        {
            settings = CreateInstance<ToolbarExtenderSettings>();
            settings.folderToFocus = string.Empty;
            Directory.CreateDirectory(Path.GetDirectoryName(TOOLBAR_EXTENDER_SETTINGS_PATH) ?? throw new ArgumentException($"The folder for {nameof(ToolbarExtenderSettings)} the couldn't be created."));
            AssetDatabase.CreateAsset(settings, TOOLBAR_EXTENDER_SETTINGS_PATH);
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
    [SettingsProvider]
    public static SettingsProvider CreateToolbarExtenderSettingsProvider()
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
                SerializedObject settings = ToolbarExtenderSettings.GetSerializedSettings();
                EditorGUILayout.PropertyField(settings.FindProperty("folderToFocus"), new GUIContent("Folder to Focus"));
                settings.ApplyModifiedPropertiesWithoutUndo();
                GUILayout.EndVertical();
            },

            // Populate the search keywords to enable smart search filtering and label highlighting:
            keywords = new HashSet<string>(new[] { "Toolbar", "Extender", "Scene" })
        };

        return provider;
    }
}