using UnityEngine;

namespace GraphTool
{
    public class InitialNodeData : NodeData
    {
        [field: SerializeField] public OutputPortData OutputNode { get; set; } = new OutputPortData();
    }
}