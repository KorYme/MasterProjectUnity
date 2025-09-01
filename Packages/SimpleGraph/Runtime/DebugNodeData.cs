using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace SimpleGraph
{
    [SimpleNodeInfo("Debug Node", "Simple Graph/Debug in Unity console node")]
    public class DebugNodeData : SimpleNodeData
    {
        [SerializeField, ExposedPort(Direction.Input)] private SimplePortData<float> _testInputPort;
        [field:SerializeField, ExposedPort(Direction.Input)] public SimplePortData<string> TestInputPort { get;
            private set; }
        
        [SerializeField, ExposedPort(Direction.Output)] private SimplePortData<Vector2> _testOutputPort;
        [field:SerializeField, ExposedPort(Direction.Output)] public SimplePortData<Vector3> TestOutputPort { get;
            private set; }

        
        [field: SerializeField, ExposedProperty] public string DebugValue { get; private set; }
        
        [SerializeField, ExposedProperty]
        private float _debugValueFloat;
        
        [SerializeField, ExposedProperty]
        private List<ScriptableObject> _debugValueList = new List<ScriptableObject>();
    }
}