using System;
using System.Collections.Generic;
using System.Linq;
using GraphTool.Editor.Interfaces;
using GraphTool.Editor.Utils;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace GraphTool.Editor
{
    public class InitialNode : GraphNode, IGraphOutputable
    {
        Action _savePortsAction;
        public InitialNodeData DerivedData => NodeData as InitialNodeData;

        Port OutputPort { get; set; }

        protected override void GenerateNodeData()
        {
            NodeData = ScriptableObject.CreateInstance<InitialNodeData>();
        }

        protected override void DrawOutputContainer()
        {
            OutputPort = CreateOutputPort(DerivedData.OutputNode);
        }

        protected Port CreateOutputPort(OutputPortData choicePortData)
        {
            Port outputPort = this.CreatePort(choicePortData.InputPortConnected?.ID ?? null);
            _savePortsAction += () => choicePortData.InputPortConnected = (outputPort.connections?.FirstOrDefault()?.input.node as GraphNode)?.NodeData;
            Label choiceTextField = new Label("First Dialogue");
            outputPort.Add(choiceTextField);
            outputContainer.Add(outputPort);
            return outputPort;
        }

        public void InitializeEdgeConnections(IEnumerable<IGraphInputable> inputables)
        {
            Port otherPort = inputables.FirstOrDefault(inputable => inputable.ID == OutputPort.name)?.InputPort;
            if (otherPort is null) return;
            _graphView.AddElement(OutputPort.ConnectTo(otherPort));
        }

        public void DisconnectAllOutputPorts() => DisconnectAllPorts(outputContainer);

        public override void Save()
        {
            base.Save();
            _savePortsAction?.Invoke();
        }
    }
}
