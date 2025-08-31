using System.Collections.Generic;
using UnityEngine;

namespace SimpleGraph
{
    [SimpleNodeInfo("Debug Node", "Simple Graph/Debug in Unity console node")]
    public class DebugNodeData : SimpleNodeData
    {
        [field: SerializeField, ExposedProperty] public string DebugValue { get; private set; }
        
        [SerializeField, ExposedProperty]
        private float _debugValueFloat;
        
        [SerializeField, ExposedProperty]
        private List<ScriptableObject> _debugValueList = new List<ScriptableObject>();
    }
}