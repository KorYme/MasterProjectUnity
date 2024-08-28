using MasterProject.Services;
using UnityEngine;

namespace TLNTH
{
    [CreateAssetMenu(menuName = "Services Container/InputService", order = 0, fileName = "InputService Container")]
    public class InputServiceContainer : ServiceContainer<IInputService>
    {
    }
}
