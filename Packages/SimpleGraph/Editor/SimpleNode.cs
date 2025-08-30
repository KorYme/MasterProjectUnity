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
        private readonly SimpleNodeData _nodeData;
        
        public SimpleNode(SimpleGraphView graphView, SimpleNodeData nodeData)
        {
            _graphView = graphView;
            _nodeData = nodeData;
            InitializeNode();
        }

        private void InitializeNode()
        {
            mainContainer.AddClasses("node__main-container");
            extensionContainer.AddClasses("node__extension-container");
            
            SetPosition(_nodeData.Position);

            Type typeInfo = _nodeData.GetType();
            SimpleNodeInfoAttribute info = typeInfo.GetCustomAttribute<SimpleNodeInfoAttribute>();
            title = info.NodeName;
            name = typeInfo.Name;

            string[] depths = info.MenuItem.Split('/');
            foreach (string depth in depths)
            {
                this.AddToClassList(depth.ToLower().Replace(" ", "-"));
            }
        }
        
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
    }
}
