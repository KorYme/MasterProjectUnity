using Discord;
using MasterProject.DiscordRPC;
using MasterProject.Services;

namespace MasterProject
{
    public interface IDiscordRichPresenceService : IService
    {
        void UpdateActivity(Activity activity);
        void UpdateActivity(DiscordActivityDescriptor activityData, bool resetTimestamp = false);
    }
}
