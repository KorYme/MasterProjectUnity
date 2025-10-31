using System.Collections.Generic;
using UnityEngine;

namespace SimpleGraph
{
    [CreateAssetMenu(fileName = "SimpleGraphData", menuName = "SimpleGraph/SimpleGraphData")]
    public class SimpleGraphData : ScriptableObject
    {
        [field: SerializeReference] public List<SimpleNodeData> Nodes { get; protected set; } = new List<SimpleNodeData>();

        /// <summary>
        /// Name of each assembly to include nodes from inside this graph<br />
        /// Choose only one class per assembly and type "yield return typeof(CLASS_CHOSEN).Assembly.FullName;"
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<string> GetAllNodeDataAssembliesForGraphEditor()
        {
            yield return typeof(SimpleNodeData).Assembly.FullName;
        }

        /// <summary>
        /// Name of each stylesheets you wanna use for this Graph<br />
        /// Resources.Load is used on the "StyleSheets" folder, so the path is relative to any Resources/StyleSheets/(Path) 
        /// </summary>
        /// <returns>Each relative path from Resources/StyleSheets/</returns>
        public virtual IEnumerable<string> GetStylesheetsForGraphEditor()
        {
            yield return "GraphViewStyles";
            yield return "NodeStyles";
        }
    }
}
