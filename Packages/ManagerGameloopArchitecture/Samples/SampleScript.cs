using UnityEngine;

namespace ManagerInjection.Samples
{
    public class SampleScript : MonoBehaviour
    {
        private void Start()
        {
            SampleGlobals.TestManager.Test();
            Debug.Log(SampleGlobals.WarningManager.GetWarningLevel);
        }
    }
}
