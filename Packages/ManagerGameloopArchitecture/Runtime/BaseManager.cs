namespace ManagerInjection.Managers
{
    public interface IManager
    {
    }
    
    #if ODIN_INSPECTOR
    public abstract class BaseManager : Sirenix.OdinInspector.SerializedMonoBehaviour, IManager
    #else
    public abstract class BaseManager : UnityEngine.MonoBehaviour, IManager
    #endif
    {
    }
}