using ManagerInjection.Managers;
using UnityEngine;

namespace ManagerInjection.Samples
{
    public interface IWarningManager : IManager
    {
        float GetWarningLevel { get; }
    }
    
    public interface IErrorManager : IManager
    {
        float GetErrorLevel { get; }
    }

    public class MessageManager : BaseManager, IWarningManager, IErrorManager
    {
        [SerializeField] private float _warningLevel;
        [SerializeField] private float _errorLevel;
    
        float IWarningManager.GetWarningLevel => _warningLevel;
        public float GetErrorLevel => _errorLevel;
    }
}
