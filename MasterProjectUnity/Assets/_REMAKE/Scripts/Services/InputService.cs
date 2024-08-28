using MasterProject.Services;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TLNTH
{
    public class InputService : BaseService, IInputService
    {
        [Header("Service Container")]
        [SerializeField] private InputServiceContainer m_referenceContainer;

        [SerializeField] private PlayerInput m_playerInput;

        public Action<Vector2> OnMove { get; set; }
        public Action<Vector2> OnLook { get; set; }
        public Action OnJump { get; set; }

        public override void Initialize()
        {
            base.Initialize();
            m_playerInput.enabled = true;
            m_referenceContainer.SetupServiceRef(this);
        }

        public override void Unload()
        {
            base.Unload();
            OnMove = null;
            OnLook = null;
            OnJump = null;
        }

        public void Move(InputAction.CallbackContext ctx)
        {
            OnMove?.Invoke(ctx.ReadValue<Vector2>());
        }

        public void Look(InputAction.CallbackContext ctx)
        {
            OnLook?.Invoke(ctx.ReadValue<Vector2>());
        }

        public void Jump(InputAction.CallbackContext ctx)
        {
            if (ctx.performed)
                OnJump?.Invoke();
        }
    }
}
