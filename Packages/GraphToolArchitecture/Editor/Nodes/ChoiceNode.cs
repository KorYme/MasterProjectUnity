using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using System.Collections.Generic;
using GraphTool.Editor.Interfaces;
using GraphTool.Utils;
using GraphTool.Editor.Utils;

namespace GraphTool.Editor
{
    public class ChoiceNode : GraphNode, IGraphInputable, IGraphOutputable
    {
        public ChoiceNodeData DerivedNodeData => NodeData as ChoiceNodeData;
        Action _savePortsAction;

        public string ID => NodeData.ID;
        public Port InputPort { get; protected set; }

        protected override void GenerateNodeData()
        {
            NodeData = ScriptableObject.CreateInstance<ChoiceNodeData>();
        }

        protected override void DrawMainContainer()
        {
            base.DrawMainContainer();
            Button addChoiceButton = UIElementUtility.CreateButton("Add Choice", () => CreateOutputPort());
            addChoiceButton.AddClasses("node__button");
            mainContainer.Insert(2, addChoiceButton);
        }

        protected override void DrawInputContainer()
        {
            InputPort = this.CreatePort(NodeData.ID, "Input Connection", direction: Direction.Input, capacity: Port.Capacity.Multi);
            inputContainer.Add(InputPort);
        }

        protected override void DrawOutputContainer()
        {
            foreach (var outputNode in DerivedNodeData.OutputNodes)
            {
                CreateOutputPort(outputNode);
            }
        }

        protected override void DrawExtensionContainer()
        {
            VisualElement customDataContainer = new VisualElement();
            customDataContainer.AddClasses("node__custom-data-container");
            Foldout textFoldout = UIElementUtility.CreateFoldout("Dialogue Text");
            TextField textTextField = UIElementUtility.CreateTextField(DerivedNodeData.DialogueText, null, 
                callbackData => DerivedNodeData.DialogueText = callbackData.newValue);
            textTextField.AddClasses(
                "node__text-field",
                "node__quote-text-field"
            );
            textTextField.multiline = true;
            textFoldout.Add(textTextField);
            customDataContainer.Add(textFoldout);
            extensionContainer.Add(customDataContainer);
        }

        protected Port CreateOutputPort(string choiceText = "New Choice")
        {
            OutputPortData portData = new OutputPortData(choiceText);
            DerivedNodeData.OutputNodes.Add(portData);
            return CreateOutputPort(portData);
        }

        protected Port CreateOutputPort(OutputPortData choicePortData)
        {
            Port outputPort = this.CreatePort(choicePortData.InputPortConnected?.ID ?? null);
            _savePortsAction += () => choicePortData.InputPortConnected = (outputPort.connections?.FirstOrDefault()?.input.node as GraphNode)?.NodeData;
            Button deleteChoiceButton = UIElementUtility.CreateButton("X",
                () => RemoveChoicePort(outputPort),
                () => DerivedNodeData.OutputNodes.Remove(choicePortData),
                () => _savePortsAction -= () => choicePortData.InputPortConnected = (outputPort.connections?.FirstOrDefault()?.input.node as GraphNode)?.NodeData
            );
            TextField choiceTextField = UIElementUtility.CreateTextField(choicePortData.ChoiceText, null, callbackData =>
            {
                choicePortData.ChoiceText = callbackData.newValue;
            });
            deleteChoiceButton.AddClasses("node__button");
            choiceTextField.AddClasses("node__text-field", "node__text-field__hidden", "node__choice-text-field");
            outputPort.Add(deleteChoiceButton, choiceTextField);
            outputContainer.Add(outputPort);
            return outputPort;
        }

        protected void RemoveChoicePort(Port port)
        {
            Edge edge = port.connections.FirstOrDefault();
            if (edge != null)
            {
                edge.input?.DisconnectAll();
                _graphView.RemoveElement(edge);
            }
            port.DisconnectAll();
            outputContainer.Remove(port);
        }

        public override void Save()
        {
            base.Save();
            _savePortsAction?.Invoke();
        }

        public virtual void InitializeEdgeConnections(IEnumerable<IGraphInputable> inputables)
        {
            foreach (Port port in outputContainer.Children().OfType<Port>())
            {
                Port otherPort = inputables.FirstOrDefault(inputable => inputable.ID == port.name)?.InputPort;
                if (otherPort is null) return;
                _graphView.AddElement(port.ConnectTo(otherPort));
            }
        }

        public void DisconnectAllOutputPorts() => DisconnectAllPorts(outputContainer);

        public void DisconnectAllInputPorts() => DisconnectAllPorts(inputContainer);
    }
}
