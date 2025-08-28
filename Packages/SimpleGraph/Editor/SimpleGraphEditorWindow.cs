using System;
using GraphTool.Utils;
using SimpleGraph.Editor.Utils;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace SimpleGraph.Editor
{
    public class SimpleGraphEditorWindow : EditorWindow
    {
        protected SimpleGraphView _graphView;
        
        [MenuItem("Window/SimpleGraph")]
        public static void OpenGraphWindow()
        {
            GetWindow<SimpleGraphEditorWindow>("Simple Graph");
        }

        private void OnEnable()
        {
            rootVisualElement.LoadAndAddStyleSheets("Variables");
            AddGraphView();
            AddToolbar();
        }
        
        private void AddGraphView()
        {
            _graphView = new SimpleGraphView(this);
            _graphView.Initialize();
            _graphView.StretchToParentSize();
            rootVisualElement.Add(_graphView);
        }
        
        private void AddToolbar()
        {
            Toolbar toolbar = new Toolbar();

            Button miniMapButton = UIElementUtility.CreateButton("Mini Map", _graphView.ToggleMinimapVisibility);
            
            if (_graphView.IsMinimapVisible)
            {
                _graphView.ToggleMinimapVisibility();
                miniMapButton.AddClasses("toolbar__button__selected");
            }
            _graphView.OnMiniMapVisibilityChanged += isVisible =>
            {
                if (isVisible)
                {
                    miniMapButton.AddToClassList("toolbar__button__selected");
                }
                else
                {
                    miniMapButton.RemoveFromClassList("toolbar__button__selected");
                }
            };

            toolbar.Add(miniMapButton);
            toolbar.LoadAndAddStyleSheets("ToolbarStyles");
            rootVisualElement.Add(toolbar);
        }
    }
}
