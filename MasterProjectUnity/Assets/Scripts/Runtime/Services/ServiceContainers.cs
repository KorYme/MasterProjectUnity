using UnityEngine;

namespace MasterProject.Services
{
    public abstract class ServiceContainerReadOnly<T> : ScriptableObject where T : IService
    {
        public T Service { get; protected set; }
    }

    //[CreateAssetMenu(menuName = "Services Container/XXX", order = 0, fileName = "XXX Container")]
    public class ServiceContainer<T> : ServiceContainerReadOnly<T> where T : IService
    {
        public void SetupServiceRef(T serviceInstance)
        {
            Service = serviceInstance;
        }
    }
}
