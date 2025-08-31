using GraphTool.Utils;
using SimpleGraph.Editor.Utils;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace SimpleGraph.Editor
{
    public class SimpleGraphEditorWindow : EditorWindow
    {
        public static void Open(SimpleGraphData newGraphData)
        {
            SimpleGraphEditorWindow[] windows = Resources.FindObjectsOfTypeAll<SimpleGraphEditorWindow>();
            foreach (SimpleGraphEditorWindow w in windows)
            {
                if (w.CurrentGraphData == newGraphData)
                {
                    w.Focus();
                    return;
                }
            }
            SimpleGraphEditorWindow window = CreateWindow<SimpleGraphEditorWindow>(typeof(SimpleGraphEditorWindow), typeof(SceneView));
            window.titleContent = new GUIContent(newGraphData.name, EditorGUIUtility.ObjectContent(null, typeof(SimpleGraphData)).image);
            window.Load(newGraphData);
        }
        
        [field: SerializeField] 
        public SimpleGraphData CurrentGraphData { get; private set; }
        
        [SerializeField] 
        private SerializedObject _serializedObject;
        
        [SerializeField] 
        protected SimpleGraphView _graphView;

        private void OnEnable()
        { 
            DrawGraph();
        }
        
        private void Load(SimpleGraphData newGraphData)
        {
            CurrentGraphData = newGraphData;
            DrawGraph();
        }
        
        private void DrawGraph()
        {
            if (!CurrentGraphData) return;
            
            rootVisualElement.Clear();
            rootVisualElement.LoadAndAddStyleSheets("Variables");
            _serializedObject = new SerializedObject(CurrentGraphData);
            _graphView = new SimpleGraphView(this, _serializedObject);
            _graphView.StretchToParentSize();
            _graphView.graphViewChanged += GraphViewChanged;
            rootVisualElement.Add(_graphView);
            AddToolbar();
        }

        private GraphViewChange GraphViewChanged(GraphViewChange graphViewChange)
        {
            hasUnsavedChanges = true;
            EditorUtility.SetDirty(CurrentGraphData);
            
            return graphViewChange;
        }
        
        public override void SaveChanges()
        {
            base.SaveChanges();
            AssetDatabase.SaveAssets();
        }


        private void AddToolbar()
        {
            Toolbar toolbar = new Toolbar();

            Button miniMapButton = UIElementUtility.CreateButton("Mini Map", _graphView.ToggleMinimapVisibility);
            
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
            _graphView.IsMinimapVisible = false;

            toolbar.Add(miniMapButton);
            toolbar.LoadAndAddStyleSheets("ToolbarStyles");
            rootVisualElement.Add(toolbar);
        }
    }
}
