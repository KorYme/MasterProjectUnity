using UnityEngine;

namespace SceneReferenceUtils
{
    [System.Serializable]
    public struct SceneReference : System.IEquatable<SceneReference>
    {
        [SerializeField] private Object m_sceneObject;
        public Object SceneObject => m_sceneObject;
        
        [SerializeField] public string m_name;
        public string Name => m_name;
        
        public bool Equals(SceneReference other)
        {
            return Name.Equals(other.Name);
        }
    }
}
