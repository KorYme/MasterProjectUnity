using MasterProject.Utilities;
using UnityEngine;

namespace MasterProject.Tests
{
    public class TestComponentWithInjection : MonoBehaviour
    {
        [ServiceDepencency] public IMusicService MusicService;

        private void Start()
        {
            MusicService.PlayMusic();
        }
    }
}
