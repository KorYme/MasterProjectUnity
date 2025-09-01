using System;
using System.Collections.Generic;
using System.Linq;
using SimpleGraph.Editor.Utils;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace SimpleGraph.Editor
{
    public class SimpleGraphView : GraphView
    {
        private readonly SerializedObject _graphSerializedObject;
        private readonly SimpleGraphData _graphData;
        private readonly SimpleGraphEditorWindow _graphEditorWindow;
        
        private MiniMap _miniMap;
        private SimpleSearchWindow _searchWindow;
        
        private List<SimpleNode> _graphNodes  = new List<SimpleNode>();
        private Dictionary<string, SimpleNode> _nodeDictionnary =  new Dictionary<string, SimpleNode>();
        
        public event Action<bool> OnMiniMapVisibilityChanged;

        public bool IsMinimapVisible
        {
            get => _miniMap?.visible ?? false;
            set
            {
                if (_miniMap == null || IsMinimapVisible == value) return;
                _miniMap.visible = value;
                OnMiniMapVisibilityChanged?.Invoke(value);
            }
        }
        
        public SimpleGraphView(SimpleGraphEditorWindow graphEditorWindow, SerializedObject graphSerializedObject)
        {
            _graphEditorWindow =  graphEditorWindow;
            _graphSerializedObject = graphSerializedObject;
            _graphData = (SimpleGraphData)_graphSerializedObject.targetObject;
            Initialize();
            DrawNodes();
            
            // Undo.undoRedoEvent += UndoRedoEvent;
            // graphViewChanged += OnGraphViewChangedEvent;
        }

        // ~SimpleGraphView()
        // {
        //     Undo.undoRedoEvent -= UndoRedoEvent;
        // }

        public void SetDirty()
        {
            _graphEditorWindow.SetDirty();
        }
        
        private void UndoRedoEvent(in UndoRedoInfo undo)
        {
            // TODO : Still need to fix
            // if (undo.undoName.Contains("Node"))
            // {
            //     foreach (SimpleNode node in nodes)
            //     {
            //         if (_graphNodes.Contains(node)) continue;
            //         RemoveElement(node);
            //     }
            // }
        }

        private GraphViewChange OnGraphViewChangedEvent(GraphViewChange graphViewChange)
        {
            if (graphViewChange.movedElements != null)
            {
                // Undo.RecordObject(_serializedObject.targetObject, "Nodes Moved");
                List<SimpleNode> nodesToMove = graphViewChange.movedElements.OfType<SimpleNode>().ToList();
                foreach (SimpleNode simpleNode in nodesToMove)
                {
                    simpleNode.SavePosition();
                }
            }
            
            if (graphViewChange.elementsToRemove != null)
            {
                List<SimpleNode> nodesToRemove = graphViewChange.elementsToRemove.OfType<SimpleNode>().ToList();
                if (nodesToRemove.Count > 0)
                {
                    // Undo.RecordObject(_serializedObject.targetObject, "Nodes Removed");
                    for (int i = nodesToRemove.Count - 1; i >= 0; i--)
                    {
                        RemoveNode(nodesToRemove[i]);
                    }
                }
            }

            return graphViewChange;
        }

        #region INITIALIZATION_STEPS
        private void Initialize()
        {
            AddManipulators();
            AddSearchWindow();
            AddMinimap();
            AddStyles();
            AddMiniMapStyles();
            AddGridBackground();
        }

        private void AddManipulators()
        {
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            this.AddManipulator(new ClickSelector());
        }
        
        private void AddSearchWindow()
        {
            if (_searchWindow) return;
            _searchWindow = ScriptableObject.CreateInstance<SimpleSearchWindow>().Initialize(this);

            nodeCreationRequest = NodeCreationRequest;
            void NodeCreationRequest(NodeCreationContext context)
            {
                _searchWindow.Target = context.target;
                SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), _searchWindow);
            }
        }
        
        private void AddMinimap()
        {
            _miniMap = new MiniMap()
            {
                anchored = true,
                visible = false,
                graphView = this,
                maxWidth = 200,
                maxHeight = 125,
            };
            _miniMap.SetPosition(new Rect(15, 50, 200, 125));
            Add(_miniMap);
        }

        private void AddStyles()
        {
            this.LoadAndAddStyleSheets(_graphData.GetStylesheetsForGraphEditor().ToArray());
        } 
        
        private void AddMiniMapStyles()
        {
            // In the future it should be added in the .uss file
            _miniMap.style.backgroundColor = new StyleColor(new Color32(29,29,29,255));
            _miniMap.style.borderBottomColor = new StyleColor(new Color32(51,51,51,255));
            _miniMap.style.borderTopColor = new StyleColor(new Color32(51,51,51,255));
            _miniMap.style.borderLeftColor = new StyleColor(new Color32(51,51,51,255));
            _miniMap.style.borderRightColor = new StyleColor(new Color32(51,51,51,255));
        }
        
        private void AddGridBackground()
        {
            GridBackground gridBackground = new GridBackground()
            {
                name = "Grid"
            };
            gridBackground.StretchToParentSize();
            Insert(0, gridBackground);
        }
        #endregion

        #region NODES_AND_GROUPS_METHODS
        private void RemoveNode(SimpleNode node)
        {
            _graphData.Nodes.Remove(node.NodeData);
            _nodeDictionnary.Remove(node.NodeData.Id);
            _graphNodes.Remove(node);
            _graphSerializedObject.Update();
        }

        private void DrawNodes()
        {
            foreach (SimpleNodeData nodeData in _graphData.Nodes)
            {
                AddNodeToGraph(nodeData);
            }
            Bind();
        }
        
        public void CreateNewNode(Type nodeDataType, Vector2 position)
        {
            object nodeData = Activator.CreateInstance(nodeDataType);
            if (nodeData is not SimpleNodeData simpleNodeData)
            {
                Debug.LogError($"The Node data type was not a derived class or the {nameof(SimpleNodeData)} class");
                return;
            }
            // Undo.RecordObject(GraphSerializedObject.targetObject, "Node Created");
            
            simpleNodeData.Position = new Rect(position, Vector2.zero);
            _graphData.Nodes.Add(simpleNodeData);
            _graphSerializedObject.Update();

            AddNodeToGraph(simpleNodeData);
        }

        private void AddNodeToGraph(SimpleNodeData simpleNodeData)
        {
            SimpleNode simpleNode = new SimpleNode(simpleNodeData, _graphSerializedObject);
            simpleNode.OnNodeModified += SetDirty;
            _graphNodes.Add(simpleNode);
            _nodeDictionnary.Add(simpleNodeData.Id, simpleNode);
            AddElement(simpleNode);
            Bind();
        }

        private void Bind()
        {
            _graphSerializedObject.Update();
            this.Bind(_graphSerializedObject);
        }
        #endregion

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            // TODO : Still need to create port for specific type
            return ports.Where(port => port.node != startPort.node && port.direction != startPort.direction).ToList();
        }
        
        #region UTILITIES
        public string[] GetNodeDataAssembliesForGraphEditor() => _graphEditorWindow.CurrentGraphData.GetAllNodeDataAssembliesForGraphEditor().ToArray();
        
        public void ToggleMinimapVisibility()
        {
            IsMinimapVisible = !IsMinimapVisible;
        }
        
        public Vector2 GetGraphMousePosition(Vector2 screenMousePosition)
        {
            return contentViewContainer.WorldToLocal(this.ChangeCoordinatesTo(this, screenMousePosition - _graphEditorWindow.position.position));
        }
        #endregion
    }
}
