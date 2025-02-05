using SceneReferenceUtils;
using MasterProject.Services;
using MasterProject.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TLNTH
{
    public class SceneLoaderService : BaseService, ISceneLoaderService
    {
        [SerializeField] private SceneReference[] m_initialScenesToLaunch;

        [ServiceDepencency] private IInputService m_InputService;

        void ISceneLoaderService.GenerateInitialScene()
        {
            foreach (SceneReference sceneRef in m_initialScenesToLaunch)
            {
                SceneManager.LoadScene(sceneRef.Name, LoadSceneMode.Additive);
            }
        }
    }
}
