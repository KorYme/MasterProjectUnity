using System.IO;
using UnityEngine;
using UnityEngine.UIElements;

namespace GraphTool.Utils.Editor
{
    public static class EditorUIStyleUtility
    {
        private const string RESOURCES_PATH = "StyleSheets";
        
        public static VisualElement LoadAndAddStyleSheets(this VisualElement element, params string[] styleSheetNames)
        {
            foreach (string styleSheetName in styleSheetNames)
            {
                element.styleSheets.Add(Resources.Load<StyleSheet>(Path.Combine(RESOURCES_PATH, styleSheetName)));
            }
            return element;
        }
    }
}
