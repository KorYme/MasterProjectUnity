using UnityEngine;

namespace GraphTool
{
    public abstract class ElementData : ScriptableObject
    {
        [field: SerializeField] public string ElementName { get; set; } = "New Element";
        [field: SerializeField] public string ID {  get; set; }
        [field: SerializeField] public Vector2 Position { get; set; }
    }
}
