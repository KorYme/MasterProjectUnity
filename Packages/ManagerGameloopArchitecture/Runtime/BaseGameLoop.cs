using ManagerInjection.Managers;
using ManagerInjection.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ManagerInjection
{
    #if ODIN_INSPECTOR
    public abstract class BaseGameLoop : Sirenix.OdinInspector.SerializedMonoBehaviour
    #else
    public abstract class BaseGameLoop : MonoBehaviour
    #endif
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Init()
        {
            s_mainGameloop = null;
        }

        public static bool TryInitialize(BaseGameLoop gameLoop)
        {
            if (!s_mainGameloop)
            {
                s_mainGameloop = gameLoop;
                return true;
            }
            return false;
        }

        private static BaseGameLoop s_mainGameloop;
        
        protected BindingContainer _managersContainer;

        protected virtual void Awake()
        {
            if (!TryInitialize(this))
            {
                Destroy(gameObject);
                return;
            }
            
            InstantiateContainer();
            InstantiateManagers(_managersContainer);
            SetupManagerInjection();
            DontDestroyOnLoad(gameObject);
            LoadScenes();
        }

        protected virtual void OnDestroy()
        {
            if (s_mainGameloop != this) return;
            
            UnbindManagers();
        }

        protected virtual void InstantiateContainer()
        {
            _managersContainer = new BindingContainer(transform);
        }

        protected abstract void InstantiateManagers(in BindingContainer container);

        protected virtual void SetupManagerInjection()
        {
            foreach (KeyValuePair<Type, IManager> kvp in _managersContainer.AllManagers)
            {
                InjectionUtilities.InjectDependencies(typeof(InjectManager), _managersContainer.AllManagers, kvp.Value);
            }
        }

        protected virtual void LoadScenes() { }
        protected abstract void UnbindManagers();
    }
}
