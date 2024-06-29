using UnityEngine;
using UnityEngine.UI;

namespace MasterProject.Utilities
{
    [RequireComponent(typeof(Graphic))]
    public class UIMaterialInstantiator : MonoBehaviour
    {
        private Graphic m_graphic;

        private void Awake()
        {
            m_graphic = GetComponent<Graphic>();
            m_graphic.material = new Material(m_graphic.material);
        }

        private void OnDestroy()
        {
            if (m_graphic.material)
            {
                Destroy(m_graphic.material);
            }
        }
    }
}