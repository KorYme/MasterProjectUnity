using MasterProject.Services;

namespace MasterProject.Tests
{
    public interface IMusicService : IService
    {
        void PlayMusic();
        void StopMusic();
    }

    public class MusicService : BaseService, IMusicService
    {
        void IMusicService.PlayMusic()
        {

        }

        void IMusicService.StopMusic()
        {

        }
    }
}
