using System.Linq;
using UnityEngine;
using MasterProject.Utilities;

namespace MasterProject.Managers
{
    public abstract class UnitySceneManager : BaseManager
	{
		public abstract string SceneName { get; }
		public SceneReferenceContainer Container { get; private set; }

		[ManagerDepencency] public UnitySceneManager a;

        public virtual void LinkSceneContainer()
        {
			var container = Object.FindObjectsOfType<SceneReferenceContainer>();
            Container = Object.FindObjectsOfType<SceneReferenceContainer>()
				.FirstOrDefault(/*goContainer => goContainer.gameObject.scene.name == SceneName*/);
			if (Container == null)
			{
				throw new System.Exception($"No {GetType().Name} has been found in {SceneName}");
			}
        }
    }
}