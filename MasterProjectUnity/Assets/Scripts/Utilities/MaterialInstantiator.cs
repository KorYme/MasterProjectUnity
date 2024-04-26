using UnityEngine;

namespace MasterProject.Utilities
{
    [RequireComponent(typeof(Renderer))]
    public class MaterialInstantiator : MonoBehaviour
    {
        private Renderer m_Renderer;

        private void Awake()
        {
            m_Renderer = GetComponent<Renderer>();
            m_Renderer.material = new Material(m_Renderer.material);
        }

        private void OnDestroy()
        {
            if (m_Renderer.material)
            {
                Destroy(m_Renderer.material);
            }
        }
    }
}