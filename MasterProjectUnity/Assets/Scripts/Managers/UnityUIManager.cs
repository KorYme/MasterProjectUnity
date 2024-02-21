using UnityEngine;

namespace MasterProject.Managers
{
    public abstract class UnityUIManager : UnitySceneManager
    {
        public Canvas Canvas => UIContainer.Canvas;
        public UISceneReferenceContainer UIContainer => Container as UISceneReferenceContainer;
    }
}
