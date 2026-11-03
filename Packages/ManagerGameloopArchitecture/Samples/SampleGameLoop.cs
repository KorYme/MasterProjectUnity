using UnityEngine;

namespace ManagerInjection.Samples
{
    public static class SampleGlobals
    {
        public static ITestManager TestManager { get; set; }
        public static IWarningManager WarningManager { get; set; }
        public static IErrorManager ErrorManager { get; set; }
    }
    
    public class SampleGameLoop : BaseGameLoop
    {
        [SerializeField] private ManagerBinder<ITestManager, TestManager> _testManagerBind;
        [SerializeField] private ManagerBinder<IWarningManager, MessageManager> _warningManagerBind;
        [SerializeField] private ManagerBinder<IErrorManager, MessageManager> _errorManagerBind;
        [SerializeField] private MockBinder<IWarningManager> _warningMockBind;
        
        protected override void InstantiateManagers(in BindingContainer container)
        {
            SampleGlobals.TestManager = container.Bind(_testManagerBind);
            SampleGlobals.WarningManager = container.Bind(_warningManagerBind);
            SampleGlobals.ErrorManager = container.Bind(_errorManagerBind);
            Debug.Log("All Managers Instantiated");
        }
    }
}
