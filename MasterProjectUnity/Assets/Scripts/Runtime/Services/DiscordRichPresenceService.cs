using MasterProject.Debugging;
using MasterProject.DiscordRPC;
using System;
using UnityEngine;

namespace MasterProject.Services
{
    public class DiscordRichPresenceService : BaseService, IDiscordRichPresenceService
    {
        private Discord.Discord m_discord;
        private Discord.LobbyManager m_lobbyManager;
        private Discord.ActivityManager m_activityManager;

        [SerializeField] private DiscordActivityDescriptor m_activity;
        public DiscordActivityDescriptor Activity => m_activity;

        public override void Initialize()
        {
            base.Initialize();
            m_discord = new Discord.Discord(m_activity.ApplicationId, (UInt64)Discord.CreateFlags.Default);
            m_activityManager = m_discord.GetActivityManager();
            m_lobbyManager = m_discord.GetLobbyManager();
            UpdateActivity(m_activity, true);
        }

        public override void BaseUpdate(float deltaTime)
        {
            m_discord.RunCallbacks();
            m_lobbyManager.FlushNetwork();
        }

        public override void Unload()
        {
            m_discord.Dispose();
        }

        public void UpdateActivity(Discord.Activity activity)
        {
            m_activityManager.UpdateActivity(activity, OnActivityUpdated);

            void OnActivityUpdated(Discord.Result result)
            {
                DebugLogger.Info(this, $"Update Activity : {result}");
            }
        }

        public void UpdateActivity(DiscordActivityDescriptor activityData, bool resetTimestamp = false)
        {
            Discord.ActivityTimestamps timestamps = new Discord.ActivityTimestamps();
            if (resetTimestamp)
            {
                timestamps.Start = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
            }
            UpdateActivity(new Discord.Activity()
            {
                ApplicationId = m_activity.ApplicationId,
                Name = m_activity.Name,
                State = m_activity.State,
                Details = m_activity.Details,
                Assets = new Discord.ActivityAssets()
                {
                    LargeImage = m_activity.LargeImage,
                    LargeText = m_activity.LargeText,
                    SmallImage = m_activity.SmallImage,
                    SmallText = m_activity.SmallText,
                },
                Timestamps = timestamps,
            });
        }
    }
}