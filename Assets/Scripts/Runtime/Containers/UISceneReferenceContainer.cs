using UnityEngine;

namespace MasterProject.Services
{
    public abstract class UISceneReferenceContainer : SceneReferenceContainer
    {
        public Canvas Canvas;

        private void OnValidate()
        {
            TryGetComponent(out Canvas);
        }
    }
}