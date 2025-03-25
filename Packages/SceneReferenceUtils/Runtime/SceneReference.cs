using System;
using UnityEngine;

namespace SceneReferenceUtils
{
    [Serializable]
    public struct SceneReference : IEquatable<SceneReference>
    {
#if UNITY_EDITOR
        [SerializeField] private UnityEditor.SceneAsset m_sceneObject;
#endif
        [SerializeField] public string m_name;
        public string Name => m_name;
        
        public bool Equals(SceneReference other)
        {
            return Name.Equals(other.Name);
        }
    }
}
