using UnityEngine;

namespace MasterProject.Managers
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