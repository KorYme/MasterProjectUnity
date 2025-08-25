using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GraphTool
{
    public class ChoiceNodeData : NodeData
    {
        [field:SerializeField , TextArea(3,10)] public string DialogueText { get; set; } = "Random dialogue !?";
        [field: SerializeField] public List<OutputPortData> OutputNodes { get; set; } = Enumerable.Repeat(new OutputPortData(), 2).ToList();
    }
}