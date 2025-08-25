using UnityEngine;

namespace GraphTool.Editor
{
    public class DialogueGraphWindowData : ScriptableObject
    {
        [field: SerializeField] public bool IsSaveOnLoad { get; set; } = false;
        [field: SerializeField] public bool IsMinimapVisible { get; set; } = false;
        [field: SerializeField] public string FileName { get; set; } = "";
        [field: SerializeField] public GraphData LastGraphData { get; set; } = null;
    }
}
