using System.Collections.Generic;
using UnityEngine;

namespace GraphTool
{
    public class GraphData : ScriptableObject
    {
        [field: SerializeField] public InitialNodeData InitialNode { get; set; }
        [field: SerializeField] public List<NodeData> AllNodes { get; set; } = new List<NodeData>();
        [field: SerializeField] public List<GraphGroupData> AllGroups { get; set; } = new List<GraphGroupData>();
    }
}