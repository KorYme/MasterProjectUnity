using UnityEngine;

namespace MasterProject.Services
{
    public abstract class ReferenceContainerReadOnly<T> : ScriptableObject
    {
        public T Instance { get; protected set; }
    }

    //[CreateAssetMenu(menuName = "Reference Container/XXX", order = 0, fileName = "XXX Container")]
    public class ReferenceContainer<T> : ReferenceContainerReadOnly<T>
    {
        public void SetupRef(T instanceRef)
        {
            Instance = instanceRef;
        }
    }
}
