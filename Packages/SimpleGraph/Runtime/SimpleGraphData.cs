using System.Collections.Generic;
using UnityEngine;

namespace SimpleGraph
{
    
    [CreateAssetMenu(fileName = "SimpleGraphData", menuName = "SimpleGraph/SimpleGraphData")]
    public class SimpleGraphData : ScriptableObject
    {
        [field: SerializeReference] public List<SimpleNodeData> Nodes { get; protected set; } = new List<SimpleNodeData>();

        public virtual IEnumerable<string> GetAllNodeDataAssembliesForGraphEditor()
        {
            yield return typeof(SimpleNodeData).Assembly.FullName;
        }
    }
}
