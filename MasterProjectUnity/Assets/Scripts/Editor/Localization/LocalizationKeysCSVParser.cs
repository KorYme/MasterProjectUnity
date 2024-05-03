using System.Collections.Generic;
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

            VisualElement root = rootVisualElement;
        }
    }
}
