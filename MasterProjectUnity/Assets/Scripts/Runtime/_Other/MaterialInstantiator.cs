using UnityEngine;

namespace MasterProject.Utilities
{
    [RequireComponent(typeof(Renderer))]
    public class MaterialInstantiator : MonoBehaviour
    {
        private Renderer m_renderer;

        private void Awake()
        {
            m_renderer = GetComponent<Renderer>();
            m_renderer.material = new Material(m_renderer.material);
        }

        private void OnDestroy()
        {
            if (m_renderer.material)
            {
                Destroy(m_renderer.material);
            }
        }
    }
}