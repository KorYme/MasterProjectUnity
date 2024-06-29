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
        private Dictionary<Type, BaseManager> m_allManagers;
        private LinkedList<BaseManager> m_managersUpdateOrder;

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
            LinkedList<BaseManager>.Enumerator currentManager = m_managersUpdateOrder.GetEnumerator();
            while (currentManager.MoveNext())
            {
                currentManager.Current.Update(deltaTime);
            }
        }

        private void InstantiateManagersContainers()
        {
            m_allManagers = new Dictionary<Type, BaseManager>();
            foreach (Type type in GameStatesAndManagers.AllManagers)
            {
                if (type.IsSubclassOf(typeof(BaseManager)) && !type.IsAbstract)
                {
                    m_allManagers.Add(type, (BaseManager)Activator.CreateInstance(type));
                }
            }
            m_managersUpdateOrder = new LinkedList<BaseManager>();
            foreach (Type type in GameStatesAndManagers.UpdateManagersOrder)
            {
                if (m_allManagers.TryGetValue(type, out BaseManager manager))
                {
                    m_managersUpdateOrder.AddLast(manager);
                }
            }
        }

        private void InitializeAndLinkReferencesManagers()
        {
            foreach (Type type in GameStatesAndManagers.InitializeManagersOrder)
            {
                if (m_allManagers.TryGetValue(type, out BaseManager manager))
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
            foreach (KeyValuePair<Type, BaseManager> kvp in m_allManagers)
            {
                InjectionUtilities.InjectDependencies(kvp.Value, typeof(ManagerDepencency), m_allManagers);
            }
        }

        public void GenerateScenes()
        {
            foreach (Type type in GameStatesAndManagers.InitializeManagersOrder)
            {
                if (m_allManagers.TryGetValue(type, out BaseManager manager) && manager is UnitySceneManager unityManager)
                {
                    SceneManager.LoadSceneAsync(unityManager.SceneName, LoadSceneMode.Additive).allowSceneActivation = true;
                }
            }
        }
    }
}
