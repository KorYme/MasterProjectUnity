using MasterProject.Services;
using MasterProject.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MasterProject
{
    public abstract class BaseGameLoop : MonoBehaviour
    {
        protected BindingContainer m_servicesContainer;

        protected abstract IReadOnlyList<Type> InitializeServicesOrder { get; }
        protected abstract IReadOnlyList<Type> UpdateServicesOrder { get; }
        protected abstract IReadOnlyList<Type> LateUpdateServicesOrder { get; }

        protected virtual void Awake()
        {
            DontDestroyOnLoad(gameObject);
            InstantiateContainer();
            InstantiateServices(m_servicesContainer);
            SetupServicesContainers();
            GenerateInitialScenes();
            SetupServicesDependencies();
            InitializeServices();
        }

        protected virtual void InstantiateContainer()
        {
            m_servicesContainer = new BindingContainer(transform);
        }

        protected abstract void InstantiateServices(in BindingContainer container);

        protected virtual void SetupServicesContainers()
        {
            m_servicesContainer.SetupOrders(UpdateServicesOrder, LateUpdateServicesOrder);
        }

        protected virtual void GenerateInitialScenes()
        {
            if (m_servicesContainer.AllServices.TryGetValue(typeof(ISceneLoaderService), out IService service) &&
                service is ISceneLoaderService sceneLoaderService)
            {
                sceneLoaderService.GenerateInitialScene();
            }
        }

        protected virtual void SetupServicesDependencies()
        {
            foreach (KeyValuePair<Type, IService> kvp in m_servicesContainer.AllServices)
            {
                InjectionUtilities.InjectDependencies(typeof(ServiceDepencency), m_servicesContainer.AllServices, kvp.Value);
            }
        }

        protected virtual void InitializeServices()
        {
            foreach (Type type in InitializeServicesOrder)
            {
                if (m_servicesContainer.AllServices.TryGetValue(type, out IService service))
                {
                    service.Initialize();
                }
            }
        }

        protected virtual void Update()
        {
            float deltaTime = Time.deltaTime;
            IEnumerator<IService> currentService = m_servicesContainer.ServicesUpdateOrder.GetEnumerator();
            while (currentService.MoveNext())
            {
                currentService.Current.BaseUpdate(deltaTime);
            }
        }

        protected virtual void LateUpdate()
        {
            float deltaTime = Time.deltaTime;
            IEnumerator<IService> currentService = m_servicesContainer.ServicesLateUpdateOrder.GetEnumerator();
            while (currentService.MoveNext())
            {
                currentService.Current.BaseLateUpdate(deltaTime);
            }
        }

        protected virtual void OnDestroy()
        {
            foreach (Type type in InitializeServicesOrder)
            {
                if (m_servicesContainer.AllServices.TryGetValue(type, out IService service) && (service?.IsLoaded ?? false))
                {
                    service.Unload();
                }
            }
        }
    }
}
