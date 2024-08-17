using MasterProject.Utilities;
using UnityEngine;

namespace TLNTH
{
    public class TLNTHCharacterController : MonoBehaviour
    {
        [ServiceDepencency] private IInputService m_inputService;

        [SerializeField] private Rigidbody m_rb;
        [SerializeField] private Camera m_camera;
        
        [Header("Parameters")]
        [SerializeField] private float m_speed;
        [Tooltip("Limits vertical camera rotation. Prevents the flipping that happens when rotation goes above 90.")]
        [Range(0f, 90f)][SerializeField] float yRotationLimit = 88f;
        [SerializeField] private float m_sensitivity;

        private Vector2 m_rotation;

        private void Start()
        {
            m_inputService.OnMove += Move;
            m_inputService.OnLook += Look;
            m_rotation = Vector2.zero;
            Cursor.visible = false;
        }

        private void Move(Vector2 inputDir)
        {
            Vector3 cameraDirFromTop = new Vector3(m_camera.transform.forward.x, 0, m_camera.transform.forward.z);
            Vector3 cameraRightFromTop = new Vector3(cameraDirFromTop.z, 0, -cameraDirFromTop.x);
            m_rb.velocity += (cameraDirFromTop * inputDir.y + cameraRightFromTop * inputDir.x).normalized * m_speed;
        }

        private void Look(Vector2 inputDirDelta)
        {
            m_rotation.x += inputDirDelta.x * m_sensitivity;
            m_rotation.y += inputDirDelta.y * m_sensitivity;
            m_rotation.y = Mathf.Clamp(m_rotation.y, -yRotationLimit, yRotationLimit);
            var xQuat = Quaternion.AngleAxis(m_rotation.x, Vector3.up);
            var yQuat = Quaternion.AngleAxis(m_rotation.y, Vector3.left);
            transform.localRotation = xQuat * yQuat;
        }
    }
}
