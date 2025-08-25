using System;
using System.Collections.Generic;
using System.Linq;
using GraphTool.Editor.Interfaces;
using GraphTool.Utils.Editor;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace GraphTool.Editor
{
    public class GraphView : UnityEditor.Experimental.GraphView.GraphView
    {
        #region PROPERTIES_AND_FIELDS
        SearchWindow _searchWindow;
        MiniMap _miniMap;
        GraphEditorWindow _graphEditorWindow;

        IEnumerable<GraphNode> _AllDSNodes => nodes.OfType<GraphNode>();
        InitialNode _initialNode;
        #endregion

        #region CONSTRUCTOR
        public GraphView(GraphEditorWindow graphEditorWindow)
        {
            _graphEditorWindow = graphEditorWindow;
            AddManipulators();
            AddSearchWindow();
            AddMinimap();
            AddStyles();
            AddMiniMapStyles();
            AddGridBackground();
            _initialNode = CreateAndAddNode<InitialNode>(Vector2.zero);
        }
        #endregion

        #region MAIN_ELEMENTS_METHODS
        protected void AddMinimap()
        {
            _miniMap = new MiniMap()
            {
                anchored = true,
                visible = false,
            };
            _miniMap.SetPosition(new Rect(15, 50, 200, 125));
            Add(_miniMap);
        }

        public bool ToggleMinimapVisibility()
        {
            _miniMap.visible = !_miniMap.visible;
            return _miniMap.visible;
        }

        protected void AddManipulators()
        {
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
        }

        protected void AddSearchWindow()
        {
            if (_searchWindow != null) return;
            _searchWindow = ScriptableObject.CreateInstance<SearchWindow>();
            _searchWindow.Initialize(this);
            nodeCreationRequest = context => UnityEditor.Experimental.GraphView.SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), _searchWindow);
        }

        public void AddGridBackground()
        {
            GridBackground gridBackground = new GridBackground()
            {
                name = "Background"
            };
            gridBackground.StretchToParentSize();
            Insert(0, gridBackground);
        }

        public void ClearGraph()
        {
            DeleteElements(graphElements);
            _initialNode = CreateAndAddNode<InitialNode>(Vector2.zero);
        }
        #endregion

        #region CONTEXT_MENU_METHODS
        public IManipulator GenerateContextMenuManipulator(string funcName, Func<Vector2, GraphElement> func)
            => new ContextualMenuManipulator(menuEvent => 
            menuEvent.menu.AppendAction(funcName, action => 
            func(GetLocalMousePosition(action.eventInfo.localMousePosition))));

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            switch (evt.target)
            {
                case GraphNode node:
                    if (node is IGraphInputable) evt.menu.AppendAction("Disconnect All Inputs Ports", _ => (node as IGraphInputable).DisconnectAllInputPorts());
                    if (node is IGraphOutputable) evt.menu.AppendAction("Disconnect All Outputs Ports", _ => (node as IGraphOutputable).DisconnectAllOutputPorts());
                    break;
            }
            base.BuildContextualMenu(evt);
        }
        #endregion

        #region NODES_AND_GROUPS_CREATION_METHODS
        public T CreateAndAddNode<T>(Vector2 position) where T : GraphNode, new()
        {
            T node = new T();
            node.InitializeElement(this, position);
            node.Draw();
            AddElement(node);
            return node;
        }

        public T CreateAndAddNode<T,Y>(Y data) where T: GraphNode, new() where Y : NodeData, new()
        {
            T node = new T();
            node.InitializeElement(this, data);
            node.Draw();
            AddElement(node);
            return node;
        }

        public T CreateAndAddGroup<T>(Vector2 position, IEnumerable<GraphElement> allChildren = null) where T : GraphGroup, new()
        {
            T group = new T();
            group.InitializeElement(position);
            group.AddElements((allChildren is null ? selection.OfType<GraphNode>() : allChildren));
            AddElement(group);
            return group;
        }

        public T CreateAndAddGroup<T>(GraphGroupData data, IEnumerable<GraphElement> allChildren = null) where T : GraphGroup, new()
        {
            T group = new T();
            group.InitializeElement(data);
            group.AddElements((allChildren is null ? selection.OfType<GraphElement>() : allChildren));
            AddElement(group);
            return group;
        }
        #endregion

        #region SAVE_AND_LOAD_METHODS
        public void LoadGraph(GraphData graphData)
        {
            if (graphData == null) return;
            // Generate all nodes
            graphData.AllGroups.RemoveAll(x => x == null);
            graphData.AllNodes.RemoveAll(x => x == null);
            if (graphData.InitialNode != null)
            {
                _initialNode.InitializeElement(this, graphData.InitialNode);
            }
            foreach (NodeData nodeData in graphData.AllNodes)
            {
                switch (nodeData)
                {
                    case ChoiceNodeData choiceNodeData:
                        CreateAndAddNode<ChoiceNode, ChoiceNodeData>(choiceNodeData);
                        break;
                    default:
                        CreateAndAddNode<GraphNode, NodeData>(nodeData);
                        break;
                }
            }
            // Generate all groups
            foreach (GraphGroupData group in graphData.AllGroups)
            {
                CreateAndAddGroup<GraphGroup>(group, _AllDSNodes.Where(dsNode => group.ChildrenNodes.Contains(dsNode.NodeData)));
            }
            // Link all nodes
            foreach (IGraphOutputable outputable in nodes.OfType<IGraphOutputable>())
            {
                outputable.InitializeEdgeConnections(nodes.OfType<IGraphInputable>());
            }
        }

        public void SaveGraph(GraphData graphData)
        {
            if (graphData == null) return;
            // Place all nodes in the ready-to-remove-list
            List<ElementData> allRemovedData = new List<ElementData>();
            allRemovedData.AddRange(graphData.AllNodes);
            allRemovedData.AddRange(graphData.AllGroups);
            graphData.AllNodes.Clear();
            graphData.AllGroups.Clear();
            allRemovedData.RemoveAll(x => x == null);
            // Check if the node still exist or if it needs to be instantiated as a scriptable object
            IEnumerable<IGraphSavable> allElements = graphElements.OfType<IGraphSavable>();
            foreach (IGraphSavable element in allElements)
            {
                switch (element)
                {
                    case InitialNode initialNode:
                        if (graphData.InitialNode == null)
                        {
                            SaveDataInProject(initialNode.NodeData, graphData);
                        }
                        graphData.InitialNode = _initialNode.DerivedData;
                        EditorUtility.SetDirty(initialNode.NodeData);
                        break;
                    case GraphNode node:
                        AddToNodes(node.NodeData, graphData, allRemovedData);
                        break;
                    case GraphGroup group:
                        AddToGroups(group.GraphGroupData, graphData, allRemovedData);
                        break;
                }
                element.Save();
            }
            // Delete all the nodes which doesn't exist anymore on the graph
            foreach (ElementData elementData in allRemovedData)
            {
                if (elementData == null) continue;
                AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(elementData));
            }
        }

        protected void SaveDataInProject<T>(T elementData, GraphData graphData) where T : ElementData
            => _graphEditorWindow.GraphSaveHandler.SaveDataInProject(elementData, graphData.name);

        protected void AddToNodes<T>(T nodeData, GraphData graphData, List<ElementData> allRemovedData) where T : NodeData
        {
            if (!allRemovedData.Remove(nodeData))
            {
                SaveDataInProject(nodeData, graphData);
            }
            graphData.AllNodes.Add(nodeData);
            EditorUtility.SetDirty(nodeData);
        }

        protected void AddToGroups<T>(T groupData, GraphData graphData, List<ElementData> allRemovedData) where T : GraphGroupData
        {
            if (!allRemovedData.Remove(groupData))
            {
                SaveDataInProject(groupData, graphData);
            }
            graphData.AllGroups.Add(groupData);
            EditorUtility.SetDirty(groupData);
        }
        #endregion

        #region STYLES_ADDITION_METHODS
        protected void AddStyles() => this.LoadAndAddStyleSheets(
            "GraphViewStyles",
            "NodeStyles"
        );

        protected void AddMiniMapStyles()
        {
            _miniMap.style.backgroundColor = new StyleColor(new Color32(29,29,29,255));
            _miniMap.style.borderBottomColor = new StyleColor(new Color32(51,51,51,255));
            _miniMap.style.borderTopColor = new StyleColor(new Color32(51,51,51,255));
            _miniMap.style.borderLeftColor = new StyleColor(new Color32(51,51,51,255));
            _miniMap.style.borderRightColor = new StyleColor(new Color32(51,51,51,255));
        }
        #endregion

        #region OVERRIDED_METHODS
        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
            => ports.Where(port => port.node != startPort.node && port.direction != startPort.direction).ToList();
        #endregion

        #region UTILITIES
        public Vector2 GetLocalMousePosition(Vector2 mousePosition, bool isSearchWindow = false)
        {
            Vector2 worldMousePosition = mousePosition;
            if (isSearchWindow)
            {
                worldMousePosition = _graphEditorWindow.rootVisualElement.ChangeCoordinatesTo(
                    _graphEditorWindow.rootVisualElement.parent, mousePosition - _graphEditorWindow.position.position) + Vector2.down * 20;
            }
            return contentViewContainer.WorldToLocal(worldMousePosition);
        }
        #endregion
    }
}
