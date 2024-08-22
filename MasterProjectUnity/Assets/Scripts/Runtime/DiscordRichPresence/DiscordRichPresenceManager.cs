using MasterProject.Debugging;
using System;
using UnityEngine;

namespace MasterProject.DiscordManager
{
    public class DiscordRichPresenceManager : MonoBehaviour
    {
        private Discord.Discord m_discord;
        private Discord.LobbyManager m_lobbyManager;
        private Discord.ActivityManager m_activityManager;

        [SerializeField] private DiscordActivityDescriptor m_activity;
        public DiscordActivityDescriptor Activity => m_activity;

        private void Awake()
        {
            m_discord = new Discord.Discord(m_activity.ApplicationId, (UInt64)Discord.CreateFlags.Default);
            m_activityManager = m_discord.GetActivityManager();
            m_lobbyManager = m_discord.GetLobbyManager();
            UpdateActivity(m_activity, true);
        }

        public void UpdateActivity(Discord.Activity activity)
        {
            m_activityManager.UpdateActivity(activity, OnActivityUpdated);
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

        private void OnActivityUpdated(Discord.Result result)
        {
            DebugLogger.Info(this, $"Update Activity : {result}");
        }

        private void Update()
        {
            m_discord.RunCallbacks();
            m_lobbyManager.FlushNetwork();
        }

        private void OnDestroy()
        {
            m_discord.Dispose();
        }
    }

    #if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(DiscordRichPresenceManager)), UnityEditor.CanEditMultipleObjects]
    public class DiscordRichPresenceManagerEditor : UnityEditor.Editor
    {
        private DiscordRichPresenceManager m_discordManager;

        private void OnEnable()
        {
            m_discordManager = (DiscordRichPresenceManager)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (Application.isPlaying && GUILayout.Button("Change Activity"))
            {
                m_discordManager.UpdateActivity(m_discordManager.Activity);
            }
        }
    }
    #endif
}