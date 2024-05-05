using MasterProject.Utilities;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace MasterProject.Editor.Localization
{
    public class LocalizationKeysCSVParser : EditorWindow
    {
        //private static readonly string TAG = nameof(LocalizationKeysCSVParser);

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
            button.clicked += ParseCSV;

        }

        private void ParseCSV()
        {
            string path = EditorUtility.OpenFilePanel("Open CSV file", "", "csv");
            m_LastTable = CSVParser.ParseCSV(path);
            for (int i = 1; i < m_LastTable.Count; i++)
            {
                Dictionary<string, string> languageTable = CollectionUtils.MapListsIntoDictionary(m_LastTable[0], m_LastTable[i]);

            }
        }
    }
}
