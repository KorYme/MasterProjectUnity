using MasterProject;
using System;
using UnityEngine;

namespace TLNTH
{
    public interface IInputService : IService
    {
        Action<Vector2> OnLook { get; set; }
        Action<Vector2> OnMove { get; set; }
        Action OnJump { get; set; }
    }
}
