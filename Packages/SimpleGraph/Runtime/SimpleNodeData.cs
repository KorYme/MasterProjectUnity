using System;
using UnityEngine;

namespace SimpleGraph
{
    [Serializable]
    [SimpleNodeInfo("Simple Node", "Simple Graph/Basic node")]
    public class SimpleNodeData
    {
        [field: SerializeField] public string Id { get; private set; }
        
        [field: SerializeField] public Rect Position { get; set; }

        public string TypeName { get; set; }

        public SimpleNodeData()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}
