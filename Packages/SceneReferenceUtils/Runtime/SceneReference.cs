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
        [field: SerializeField] public string Name { get; private set; }

        public bool Equals(SceneReference other)
        {
            return Name.Equals(other.Name);
        }
    }
}
