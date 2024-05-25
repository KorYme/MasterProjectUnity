using MasterProject.Debugging;
using MasterProject.Localization;
using MasterProject.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace MasterProject.Editor.Localization
{
    public class LocalizationKeysCSVParser : EditorWindow
    {
        [SerializeField] private VisualTreeAsset m_Tree;

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
            DropdownField languageDropdown = rootVisualElement.Q<DropdownField>("LanguagesDropdown");
            LocalizationHandler localizationHandler = LocalizationHandlerEditor.Instance;
            languageDropdown.choices = new List<string>(localizationHandler.LanguagesIDs);
            languageDropdown.RegisterValueChangedCallback(value =>
            {
                localizationHandler.SetLocalizationLanguage(value.newValue);
            });
        }

        private void ParseLocalizationCSV()
        {
            string path = EditorUtility.OpenFilePanel("Open CSV file", "", "csv");
            if (string.IsNullOrEmpty(path))
            {
                DebugLogger.Warning(this, "No file has been selected");
                return;
            }
            if (Directory.Exists(LocalizationHandler.LocalizationAssetsDirectory))
            {
                IEnumerable<string> files = Directory.GetFiles(LocalizationHandler.LocalizationAssetsDirectory);
                foreach (string file in files)
                {
                    File.Delete(file);
                }
            }
            List<List<string>> table = CSVParser.ParseCSV(path);
            for (int i = 1; i < table.Count; i++)
            {
                LocalizationData languageData = new LocalizationData()
                {
                    LanguageID = table[i][0],
                    LocalizationKeys = CollectionUtils.MapListsIntoDictionary(table[0].Skip(1), table[i].Skip(1))
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
            LocalizationHandlerEditor.Instance.Initialize();
            AssetDatabase.Refresh();
        }
    }
}
