using System;
using System.Linq;
using GraphTool.Editor.Interfaces;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace GraphTool.Editor
{
    public class GraphGroup : Group, IGraphSavable
    {
        public GraphGroupData GraphGroupData { get; protected set; }

        public void InitializeElement(GraphGroupData graphGroupData)
        {
            GraphGroupData = graphGroupData;
            InitializeFieldsWithGroupData();
        }

        public void InitializeElement(Vector2 position)
        {
            GenerateGroupData();
            InitializeGroupDataFields(position);
            InitializeFieldsWithGroupData();
        }

        protected virtual void GenerateGroupData()
        {
            GraphGroupData = ScriptableObject.CreateInstance<GraphGroupData>();
        }

        protected virtual void InitializeGroupDataFields(Vector2 position)
        {
            GraphGroupData.ID = Guid.NewGuid().ToString();
            GraphGroupData.Position = position;
        }

        void InitializeFieldsWithGroupData()
        {
            SetPosition(new Rect(GraphGroupData.Position, Vector2.zero));
            title = GraphGroupData.ElementName;
        }

        public void RemoveAllSubElements() => RemoveElements(containedElements);

        protected override void OnGroupRenamed(string oldName, string newName)
        {
            base.OnGroupRenamed(oldName, newName);
            GraphGroupData.ElementName = newName;
        }

        public void Save()
        {
            GraphGroupData.Position = GetPosition().position;
            GraphGroupData.ChildrenNodes = containedElements.OfType<GraphNode>().Select(node => node.NodeData).ToList();
        }
    }
}
