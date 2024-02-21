using MasterProject.Managers;
using MasterProject.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MasterProject
{
    public class GameLoop : MonoBehaviour
    {
        Dictionary<Type, BaseManager> m_AllManagers;
        LinkedList<BaseManager> m_ManagersUpdateOrder;

        private void Awake()
        {
            InstantiateManagersContainers();
            SetupDependencies();
            GenerateScenes();
            InitializeAndLinkReferencesManagers();
        }

        private void Update()
        {
            float deltaTime = Time.deltaTime;
            LinkedList<BaseManager>.Enumerator currentManager = m_ManagersUpdateOrder.GetEnumerator();
            while (currentManager.MoveNext())
            {
                currentManager.Current.Update(deltaTime);
            }
        }

        private void InstantiateManagersContainers()
        {
            m_AllManagers = new Dictionary<Type, BaseManager>();
            foreach (Type type in GameStatesAndManagers.AllManagers)
            {
                if (type.IsSubclassOf(typeof(BaseManager)) && !type.IsAbstract)
                {
                    m_AllManagers.Add(type, (BaseManager)Activator.CreateInstance(type));
                }
            }
            m_ManagersUpdateOrder = new LinkedList<BaseManager>();
            foreach (Type type in GameStatesAndManagers.UpdateManagersOrder)
            {
                if (m_AllManagers.TryGetValue(type, out BaseManager manager))
                {
                    m_ManagersUpdateOrder.AddLast(manager);
                }
            }
        }

        private void InitializeAndLinkReferencesManagers()
        {
            foreach (Type type in GameStatesAndManagers.InitializeManagersOrder)
            {
                if (m_AllManagers.TryGetValue(type, out BaseManager manager))
                {
                    if (manager is UnitySceneManager sceneManager)
                    {
                        sceneManager.LinkSceneContainer();
                    }
                    manager.Initialize();
                }
            }
        }

        public void SetupDependencies()
        {
            foreach (KeyValuePair<Type, BaseManager> kvp in m_AllManagers)
            {
                InjectionUtilities.InjectDependencies(kvp.Key, typeof(InjectionDependency), m_AllManagers);
            }
        }

        public void GenerateScenes()
        {
            foreach (Type type in GameStatesAndManagers.InitializeManagersOrder)
            {
                if (m_AllManagers.TryGetValue(type, out BaseManager manager) && manager is UnitySceneManager unityManager)
                {
                    SceneManager.LoadSceneAsync(unityManager.SceneName, LoadSceneMode.Additive).allowSceneActivation = true;
                }
            }
        }
    }
}
