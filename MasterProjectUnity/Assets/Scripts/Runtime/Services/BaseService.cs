using UnityEngine;

namespace MasterProject.Services
{
    public abstract class BaseService : MonoBehaviour, IService
    {
        public virtual void Initialize()
        {
        }

        public virtual void Unload()
        {
        }

        public virtual void BaseUpdate(float deltaTime)
        {
        }

        public virtual void BaseLateUpdate(float deltaTime)
        {
        }
    }
}