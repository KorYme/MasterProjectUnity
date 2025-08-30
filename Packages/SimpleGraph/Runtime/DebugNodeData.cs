using UnityEngine;

namespace SimpleGraph
{
    [SimpleNodeInfo("Debug Node", "Simple Graph/Debug in Unity console node")]
    public class DebugNodeData : SimpleNodeData
    {
        [field: SerializeField] public string DebugValue { get; private set; }
    }
}