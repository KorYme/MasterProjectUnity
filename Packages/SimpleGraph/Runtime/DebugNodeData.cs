using System.Collections.Generic;
using UnityEngine;

namespace SimpleGraph
{
    [SimpleNodeInfo("Debug Node", "Simple Graph/Debug in Unity console node")]
    public class DebugNodeData : SimpleNodeData
    {
        [SerializeField, ExposedInputPort] 
        private SimplePortData<int> _testIntInputPort;
        [SerializeField, ExposedInputPort] 
        private SimplePortData<float> _testInputPort;
        [field:SerializeField, ExposedInputPort] 
        public SimplePortData<Vector2> TestInputPort { get; private set; }
        
        [SerializeField, ExposedOutputPort] 
        private SimplePortData<float> _testFloatOutputPort;
        [SerializeField, ExposedOutputPort]
        private SimplePortData<Vector2> _testOutputPort;
        [field:SerializeField, ExposedOutputPort] 
        public SimplePortData<Vector3> TestOutputPort { get; private set; }
        
        [field: SerializeField, ExposedProperty] public string DebugValue { get; private set; }
        
        [SerializeField, ExposedProperty]
        private float _debugValueFloat;
        
        [SerializeField, ExposedProperty]
        private List<ScriptableObject> _debugValueList = new List<ScriptableObject>();
    }
}