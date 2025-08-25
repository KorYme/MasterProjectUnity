using UnityEngine;

namespace GraphTool.Editor
{
    public class GraphWindowSettings : ScriptableObject
    {
        [field: SerializeField] public bool IsSaveOnLoad { get; set; }
        [field: SerializeField] public bool IsMinimapVisible { get; set; }
        [field: SerializeField] public string FileName { get; set; }
        [field: SerializeField] public GraphData LastGraphData { get; set; }
    }
}
