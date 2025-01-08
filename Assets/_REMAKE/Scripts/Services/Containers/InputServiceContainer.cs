using MasterProject.Services;
using UnityEngine;

namespace TLNTH
{
    [CreateAssetMenu(menuName = "Service Containers/InputService", order = 0, fileName = "InputService Container")]
    public class InputServiceContainer : ReferenceContainer<IInputService>
    {
    }
}
