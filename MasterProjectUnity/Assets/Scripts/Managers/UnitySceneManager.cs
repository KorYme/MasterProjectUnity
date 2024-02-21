using System.Linq;
using UnityEngine;

namespace MasterProject.Managers
{
    public abstract class UnitySceneManager : BaseManager
	{
		public abstract string SceneName { get; }
		public SceneReferenceContainer Container { get; private set; }

        public virtual void LinkSceneContainer()
        {
			var container = Object.FindObjectsOfType<SceneReferenceContainer>();
            Container = Object.FindObjectsOfType<SceneReferenceContainer>()
				.FirstOrDefault(/*goContainer => goContainer.gameObject.scene.name == SceneName*/);
			if (Container == null)
			{
				throw new System.Exception($"No {typeof(SceneReferenceContainer).Name} has been found in {SceneName}");
			}
        }
    }
}