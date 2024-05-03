using UnityEngine;
using UnityEngine.UI;

namespace MasterProject.Utilities
{
    [RequireComponent(typeof(Graphic))]
    public class UIMaterialInstantiator : MonoBehaviour
    {
        private Graphic m_Graphic;

        private void Awake()
        {
            m_Graphic = GetComponent<Graphic>();
            m_Graphic.material = new Material(m_Graphic.material);
        }

        private void OnDestroy()
        {
            if (m_Graphic.material)
            {
                Destroy(m_Graphic.material);
            }
        }
    }
}