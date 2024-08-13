using MasterProject.Utilities;
using UnityEngine;

namespace TLNTH
{
    public class TLNTHCharacterController : MonoBehaviour
    {
        [ServiceDepencency] private IInputService m_inputService;

        [SerializeField] private Rigidbody m_rb;
        [SerializeField] private float m_speed;

        private void Start()
        {
            m_inputService.OnMove += Move;
        }

        private void Move(Vector2 inputDir)
        {
            m_rb.velocity += (transform.forward * inputDir.y + transform.right * inputDir.x).normalized * m_speed;
        }
    }
}
