using System.Collections.Generic;
using UnityEngine;

namespace SimpleGraph
{
    [CreateAssetMenu(fileName = "TestSimpleGraphData", menuName = "SimpleGraph/TestSimpleGraphData")]
    public class TestSimpleGraphData : SimpleGraphData
    {
        public override IEnumerable<string> GetAllNodeDataAssembliesForGraphEditor()
        {
            foreach (string value in base.GetAllNodeDataAssembliesForGraphEditor())
            {
                yield return value;
            }
            yield return "TestSimpleGraphData";
        }
    }
}
