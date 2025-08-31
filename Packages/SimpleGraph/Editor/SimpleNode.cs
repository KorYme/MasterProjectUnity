using System;
using System.Reflection;
using GraphTool.Utils;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace SimpleGraph.Editor
{
    public class SimpleNode : Node
    {
        private readonly SimpleGraphView _graphView;
        public readonly SimpleNodeData NodeData;
        
        public SimpleNode(SimpleGraphView graphView, SimpleNodeData nodeData)
        {
            _graphView = graphView;
            NodeData = nodeData;
            InitializeNode();
        }

        private void InitializeNode()
        {
            mainContainer.AddClasses("node__main-container");
            extensionContainer.AddClasses("node__extension-container");
            
            SetPosition(NodeData.Position);

            Type typeInfo = NodeData.GetType();
            SimpleNodeInfoAttribute info = typeInfo.GetCustomAttribute<SimpleNodeInfoAttribute>();
            title = info.NodeName;
            name = typeInfo.Name;

            string[] depths = info.MenuItem.Split('/');
            foreach (string depth in depths)
            {
                this.AddToClassList(depth.ToLower().Replace(" ", "-"));
            }
        }

        public void SavePosition()
        {
            NodeData.Position = GetPosition();
        }
        
        #region CONTAINERS_DRAWERS
        public void Draw()
        {
            DrawTitleContainer(titleContainer);

            DrawMainContainer(mainContainer);

            DrawInputContainer(inputContainer);

            DrawOutputContainer(outputContainer);

            DrawExtensionContainer(extensionContainer);

            // USEFUL CALL
            RefreshExpandedState();
        }

        private void DrawTitleContainer(VisualElement container) { }

        private void DrawMainContainer(VisualElement container) { }
        
        private void DrawInputContainer(VisualElement container) { }

        private void DrawOutputContainer(VisualElement container) { }

        private void DrawExtensionContainer(VisualElement container) { }
        #endregion
    }
}
