using UnityEngine;

namespace SceneReferenceUtils
{
    [System.Serializable]
    public struct SceneReference
    {
#if UNITY_EDITOR
        [SerializeField] private UnityEditor.SceneAsset m_sceneObject;
#endif
        [SerializeField] private string m_name;

        public string Name => m_name;
    }
}
