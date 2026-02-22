using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace CustomPlayButton
{
    [FilePath("UserSettings/SceneBookmarkSettings.asset", FilePathAttribute.Location.ProjectFolder)]
    public class SceneBookmarkSettings : ScriptableSingleton<SceneBookmarkSettings>
    {
        [SerializeField]
        private List<SceneAsset> _scenes = new List<SceneAsset>();
        
        private List<SceneAsset> _pendingRemovedScenes = new List<SceneAsset>();

        public IEnumerable<SceneAsset> Scenes => _scenes;
        
        public int ScenesCount => _scenes.Count;

        public bool Contains(SceneAsset scene)
        {
            return _scenes.Contains(scene);
        }

        public void AddScene(SceneAsset scene)
        {
            _scenes.Add(scene);
        }

        public bool RemoveScene(SceneAsset scene)
        {
            bool result = _scenes.Contains(scene);
            _pendingRemovedScenes.Add(scene);
            return result;
        }

        public void Refresh()
        {
            foreach (SceneAsset scene in _pendingRemovedScenes)
            {
                _scenes.Remove(scene);
            }
            _pendingRemovedScenes.Clear();
            _scenes.RemoveAll(item => item == null);
            Save(true);
        }
    }
}
