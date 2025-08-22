using System.IO;
using UnityEditor;
using UnityEngine.UIElements;

namespace KorYmeLibrary.Utilities.Editor
{
    public static class EditorUIStyleUtility
    {
        private const string PATH = "Assets/DialogTool/StyleSheets";
        
        public static VisualElement LoadAndAddStyleSheets(this VisualElement element, params string[] styleSheetNames)
        {
            foreach (string styleSheetName in styleSheetNames)
            {
                // element.styleSheets.Add((StyleSheet)EditorGUIUtility.Load(styleSheetName));
                element.styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>(Path.Combine(PATH, styleSheetName)));
            }
            return element;
        }
    }
}
