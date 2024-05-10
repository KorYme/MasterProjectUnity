using MasterProject.Debugging;
using MasterProject.Localization;
using MasterProject.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace MasterProject.Editor.Localization
{
    public class LocalizationKeysCSVParser : EditorWindow
    {
        [SerializeField] private VisualTreeAsset m_Tree;

        private List<List<string>> m_LastTable = null;

        [MenuItem("MasterProject/Localization/LocalizationKeysCSVParser")]
        public static void OpenWindow()
        {
            LocalizationKeysCSVParser window = GetWindow<LocalizationKeysCSVParser>();
            window.titleContent = new GUIContent(nameof(LocalizationKeysCSVParser));
        }

        private void CreateGUI()
        {
            m_Tree.CloneTree(rootVisualElement);
            Button button = rootVisualElement.Q<Button>("ParseCSVButton");
            button.clicked += ParseLocalizationCSV;

        }

        private void ParseLocalizationCSV()
        {
            string path = EditorUtility.OpenFilePanel("Open CSV file", "", "csv");
            m_LastTable = CSVParser.ParseCSV(path);
            for (int i = 1; i < m_LastTable.Count; i++)
            {
                LanguageData languageData = new LanguageData()
                {
                    LanguageID = m_LastTable[i][0],
                    LocalizationKeys = CollectionUtils.MapListsIntoDictionary(m_LastTable[0], m_LastTable[i])
                };
                string jsonContent = JsonConvert.SerializeObject(languageData, Formatting.Indented);
                string fullPath = LocalizationHandler.GetLocalizationAssetFullPath(languageData.LanguageID);
                try
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
                    using FileStream stream = new FileStream(fullPath, FileMode.Create);
                    using StreamWriter writer = new StreamWriter(stream);
                    writer.Write(jsonContent);
                }
                catch (Exception exc)
                {
                    DebugLogger.Error(this, "Error occured when trying to save data to file: " + fullPath + "\n" + exc);
                    throw;
                }
            }
            AssetDatabase.Refresh();
        }
    }
}
