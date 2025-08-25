using UnityEngine;

namespace GraphTool
{
    [System.Serializable]
    public class OutputPortData
    {
        [field: SerializeField] public string ChoiceText { get; set; }
        [field: SerializeField] public NodeData InputPortConnected { get; set; }

        public OutputPortData(string choiceName = "", NodeData outputPort = null)
        {
            ChoiceText = choiceName;
            InputPortConnected = outputPort;
        }
    }
}