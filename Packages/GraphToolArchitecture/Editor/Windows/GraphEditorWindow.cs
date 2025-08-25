using System;
using GraphTool.Utils;
using GraphTool.Utils.Editor;
using SerializationUtils;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace GraphTool.Editor
{
    public class GraphEditorWindow : EditorWindow
    {
        #region PROPERTIES_AND_FIELDS
        GraphWindowSettings _graphWindowSettings;
        public GraphWindowSettings GraphWindowSettings
        {
            get
            {
                if (!_graphWindowSettings)
                {
                    _graphWindowSettings = GraphSaveHandler.GetOrGenerateNewWindowData();
                }
                return _graphWindowSettings;
            }
        }
        GraphView _graphView;
        GraphData _graphData;
        public GraphData GraphData
        {
            get => _graphData;
            set
            {
                if (value != _graphData)
                {
                    if (GraphData != null && GraphWindowSettings.IsSaveOnLoad)
                    {
                        SaveData();
                    }
                    _graphData = value;
                    _onGraphDataChange?.Invoke(value != null);
                    LoadData();
                }
            }
        }
        public GraphSaveHandler GraphSaveHandler { get; private set; }

        event Action<bool> _onGraphDataChange;
        event Action<string> _onFileNameChange;
        event Action<bool> _onMiniMapVisibilityChanged;
        #endregion


        public override void SaveChanges()
        {
            base.SaveChanges();
            SaveData();
        }

        #region UNITY_METHODS
        [MenuItem("Window/Dialog System/Dialogue Graph")]
        public static void OpenGraphWindow()
        {
            GetWindow<GraphEditorWindow>("Dialogue Graph");
        }

        private void OnEnable()
        {
            GraphSaveHandler = new GraphSaveHandler();
            _graphData = GraphWindowSettings.LastGraphData;
            _onGraphDataChange += value => { if (value) GraphWindowSettings.LastGraphData = GraphData; };
            rootVisualElement.LoadAndAddStyleSheets("Variables");
            AddGraphView();
            AddToolbar();
            LoadData();
        }

        private void OnDisable()
        {
            GraphData = null;
        }
        #endregion

        #region MAIN_VISUAL_ELEMENT_CREATION
        private void AddGraphView()
        {
            _graphView = new GraphView(this);
            _graphView.StretchToParentSize();
            rootVisualElement.Add(_graphView);
        }

        private void AddToolbar()
        {
            Toolbar toolbar = new Toolbar();

            ObjectField graphFileField = EditorUIElementUtility.CreateObjectField("Graph File :", typeof(GraphData), GraphData == null ? null : GraphData, ChangeGraphDataFile);
            Button saveButton = UIElementUtility.CreateButton("Save", SaveData);
            Toggle autoSavetoggle = UIElementUtility.CreateToggle(GraphWindowSettings.IsSaveOnLoad ,"Save on Load :", ChangeSaveOnLoad);
            Button clearButton = UIElementUtility.CreateButton("Clear", ClearGraph);
            TextField fileNameTextfield = UIElementUtility.CreateTextField(GraphWindowSettings.FileName, "New File Name :", ChangeFileName);
            Button newGraphButton = UIElementUtility.CreateButton("New Graph", GenerateNewGraph);
            Button miniMapButton = UIElementUtility.CreateButton("Mini Map", ToggleMiniMap);

            saveButton.SetEnabled(GraphData != null);
            _onGraphDataChange += saveButton.SetEnabled;
            if (GraphWindowSettings.IsMinimapVisible)
            {
                _graphView.ToggleMinimapVisibility();
                miniMapButton.AddClasses("toolbar__button__selected");
            }
            _onMiniMapVisibilityChanged += _ => miniMapButton.ToggleInClassList("toolbar__button__selected");

            _onGraphDataChange += _ => graphFileField.SetValueWithoutNotify(GraphData);
            _onFileNameChange += fileNameTextfield.SetValueWithoutNotify;

            toolbar.Add(graphFileField, saveButton, autoSavetoggle, clearButton, fileNameTextfield, newGraphButton, miniMapButton);
            toolbar.LoadAndAddStyleSheets("ToolbarStyles");
            rootVisualElement.Add(toolbar);
        }
        #endregion

        #region TOOLBAR_METHODS
        private void ChangeGraphDataFile(ChangeEvent<Object> callbackData) => GraphData = callbackData.newValue as GraphData;

        private void SaveData()
        {
            if (GraphData != null)
            {
                _graphView?.SaveGraph(GraphData);
                EditorUtility.SetDirty(GraphData);
                EditorUtility.SetDirty(GraphWindowSettings);
                AssetDatabase.SaveAssets();
                Debug.Log($"The graph data {GraphData.name} has been saved.");
            }
            else
            {
                Debug.LogWarning("There is no GraphData Loaded");
            }
        }

        private void LoadData()
        {
            ClearGraph();
            _graphView?.LoadGraph(GraphData);
        }

        private void ClearGraph() => _graphView?.ClearGraph();
        
        private void ChangeSaveOnLoad(ChangeEvent<bool> callbackData) => GraphWindowSettings.IsSaveOnLoad = callbackData.newValue;
        private void ChangeFileName(ChangeEvent<string> callbackData) => GraphWindowSettings.FileName = callbackData.newValue;

        private void GenerateNewGraph()
        {
            if (!GraphWindowSettings.FileName.IsSerializableFriendly())
            {
                Debug.LogWarning($"The file name \"{GraphWindowSettings.FileName}\" could not be serialized, all non serializable characters have been removed.");
                GraphWindowSettings.FileName = GraphWindowSettings.FileName.RemoveNonSerializableCharacters();
                _onFileNameChange?.Invoke(GraphWindowSettings.FileName);
                return;
            }
            GraphData newGraphData = GraphSaveHandler.GenerateGraphDataFile(GraphWindowSettings.FileName);
            InitialNodeData initialNodeData = CreateInstance<InitialNodeData>();
            initialNodeData.ID = Guid.NewGuid().ToString();
            GraphSaveHandler.SaveDataInProject(initialNodeData, GraphWindowSettings.FileName);
            newGraphData.InitialNode = initialNodeData;
            if (newGraphData != null)
            {
                if (GraphData != null)
                {
                    GraphData = newGraphData;
                }
                else
                {
                    _graphData = newGraphData;
                    _onGraphDataChange?.Invoke(true);
                    SaveData();
                }
            }
        }

        private void ToggleMiniMap()
        {
            GraphWindowSettings.IsMinimapVisible = _graphView.ToggleMinimapVisibility();
            _onMiniMapVisibilityChanged?.Invoke(GraphWindowSettings.IsMinimapVisible);
        }
        #endregion
    }
}