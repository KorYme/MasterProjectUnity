using UnityEngine;

namespace ManagerInjection.Samples
{
    public static class SampleGlobals
    {
        public static ITestManager TestManager { get; set; }
        public static IWarningManager WarningManager { get; set; }
    }
    
    public class SampleGameLoop : BaseGameLoop
    {
        [SerializeField] private ManagerBinder<ITestManager, TestManager> _testManagerBind;
        [SerializeField] private ManagerBinder<IWarningManager, WarningManager> _warningManagerBind;
        
        protected override void InstantiateManagers(in BindingContainer container)
        {
            SampleGlobals.TestManager = container.Bind(_testManagerBind);
            SampleGlobals.WarningManager = container.Bind(_warningManagerBind);
            Debug.Log("All Managers Instantiated");
        }
    }
}
