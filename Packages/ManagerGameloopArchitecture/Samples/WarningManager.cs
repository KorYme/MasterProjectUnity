using ManagerInjection.Managers;
using UnityEngine;

namespace ManagerInjection.Samples
{
    public interface IWarningManager : IManager
    {
        float GetWarningLevel { get; }
    }
    
    public class WarningManager : BaseManager, IWarningManager
    {
        [SerializeField] private float _warningLevel;

        float IWarningManager.GetWarningLevel => _warningLevel;
    }
}
