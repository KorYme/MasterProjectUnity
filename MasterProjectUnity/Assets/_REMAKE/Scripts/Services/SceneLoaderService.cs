using MasterProject;
using MasterProject.Services;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TLNTH
{
    public class SceneLoaderService : BaseService, ISceneLoaderService
    {
        [SerializeField] private SceneReference[] m_initialScenesToLaunch;

        public override void Initialize()
        {
            base.Initialize();
        }

        void ISceneLoaderService.GenerateInitialScene()
        {
            foreach (SceneReference sceneRef in m_initialScenesToLaunch)
            {
                SceneManager.LoadScene(sceneRef.Name, LoadSceneMode.Additive);
            }
        }
    }
}
