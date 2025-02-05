using UnityEngine;
using UnityEngine.SceneManagement;

namespace SceneReferenceUtils
{
    public static class SceneReferenceUtils
    {
        public static void LoadScene(this SceneReference sceneRef, LoadSceneMode mode = LoadSceneMode.Single)
        {
            SceneManager.LoadScene(sceneRef.Name, mode);
        }
        
        public static void LoadScene(this SceneReference sceneRef, LoadSceneParameters parameters)
        {
            SceneManager.LoadScene(sceneRef.Name, parameters);
        }
        
        public static AsyncOperation LoadSceneAsync(this SceneReference sceneRef, LoadSceneMode mode = LoadSceneMode.Single)
        {
            return SceneManager.LoadSceneAsync(sceneRef.Name, mode);
        }
        
        public static AsyncOperation LoadSceneAsync(this SceneReference sceneRef, LoadSceneParameters parameters)
        {
            return SceneManager.LoadSceneAsync(sceneRef.Name, parameters);
        }
    }
}