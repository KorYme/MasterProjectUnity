using System.Collections.Generic;
using UnityEngine;

namespace GraphTool
{
    public class GraphGroupData : ElementData
    {
        [field: SerializeField] public List<NodeData> ChildrenNodes { get; set; } = new List<NodeData>();
    }
}