using ManagerInjection.Managers;
using ManagerInjection.Utilities;
using UnityEngine;

namespace ManagerInjection.Samples
{
    public interface ITestManager : IManager
    {
        void Test();

        void Retest();
    }
    
    public class TestManager : BaseManager, ITestManager
    {
        [InjectManager] private IWarningManager _warningManager;
        
        public void Test()
        {
            Debug.Log($"Warning level : {_warningManager.GetWarningLevel}");
        }

        public void Retest()
        {
        }

        private void Awake()
        {
            Debug.Log("TestManager Awake : Showcasing the fact that awake is called before some managers initialization.");
        }
    }
}
