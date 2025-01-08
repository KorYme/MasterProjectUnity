using UnityEngine;

namespace MasterProject.Services
{
    public abstract class BaseService : MonoBehaviour, IService
    {
        public bool IsLoaded { get; private set; }

        public virtual void Initialize()
        {
            IsLoaded = true;
        }

        public virtual void Unload()
        {
            IsLoaded = false;
        }

        public virtual void BaseUpdate(float deltaTime)
        {
        }

        public virtual void BaseLateUpdate(float deltaTime)
        {
        }

        private void OnDestroy()
        {
            if (IsLoaded)
            {
                Unload();
            }
        }
    }
}