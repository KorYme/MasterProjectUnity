using System;
using GraphTool.Editor.Interfaces;
using GraphTool.Utils;
using GraphTool.Editor.Utils;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GraphTool.Editor
{
    public class GraphNode : Node, IGraphSavable
    {
        public NodeData NodeData { get; protected set; }
        protected GraphView _graphView;
        private Action _onNodeDataChanged;

        public GraphNode()
        {
            mainContainer.AddClasses("node__main-container");
            extensionContainer.AddClasses("node__extension-container");
        }

        public void InitializeElement(GraphView graphView, Vector2 position)
        {
            GenerateNodeData();
            InitializeNodeDataFields();
            _graphView = graphView;
            SetPosition(new Rect(position, Vector2.zero));
        }

        public void InitializeElement(GraphView graphView, NodeData data)
        {
            NodeData = data;
            _onNodeDataChanged?.Invoke();
            _graphView = graphView;
            SetPosition(new Rect(data.Position, Vector2.zero));
        }

        protected virtual void GenerateNodeData()
        {
            NodeData = ScriptableObject.CreateInstance<NodeData>();
        }

        protected virtual void InitializeNodeDataFields()
        {
            NodeData.ID = Guid.NewGuid().ToString();
            NodeData.ElementName = GetType().Name;
        }

        public virtual void Draw()
        {
            // TITLE CONTAINER
            DrawTitleContainer();

            // MAIN CONTAINER
            DrawMainContainer();

            // INPUT CONTAINER
            DrawInputContainer();

            // OUTPUT CONTAINER
            DrawOutputContainer();

            // EXTENSION CONTAINER
            DrawExtensionContainer();

            // USEFUL CALL
            RefreshExpandedState();
        }

        protected virtual void DrawTitleContainer() 
        {
            TextField dialogueNameTextField = UIElementUtility.CreateTextField(NodeData.ElementName, null, 
                callbackEvent => NodeData.ElementName = callbackEvent.newValue);
            titleContainer.Insert(0, dialogueNameTextField);
            dialogueNameTextField.AddClasses(
                "node__text-field",
                "node__text-field__hidden",
                "node__filename-text-field"
            );
        }

        protected virtual void DrawMainContainer()
        {
            Foldout scriptableReferenceFoldout = UIElementUtility.CreateFoldout("Scriptable Reference", true);
            ObjectField dataScriptable = UIElementEditorUtility.CreateObjectField(null, typeof(NodeData), NodeData);
            _onNodeDataChanged += () => dataScriptable.SetValueWithoutNotify(NodeData);
            dataScriptable.SetEnabled(false);
            scriptableReferenceFoldout.Add(dataScriptable);
            mainContainer.Insert(1, scriptableReferenceFoldout);
        }

        protected virtual void DrawInputContainer() { }

        protected virtual void DrawOutputContainer() { }

        protected virtual void DrawExtensionContainer() { }

        public virtual void Save()
        {
            NodeData.Position = GetPosition().position;
        }

        #region UTILITIES
        protected void DisconnectAllPorts(VisualElement container)
        {
            foreach (Port port in container.Children())
            {
                _graphView.DeleteElements(port.connections);
                port.DisconnectAll();
            }
        }
        #endregion
    }
}
