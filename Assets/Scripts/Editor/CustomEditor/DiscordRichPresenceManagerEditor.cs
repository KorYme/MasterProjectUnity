using MasterProject.Services;
using UnityEditor;
using UnityEngine;

namespace MasterProject.Editor
{
    [CustomEditor(typeof(DiscordRichPresenceService)), CanEditMultipleObjects]
    public class DiscordRichPresenceManagerEditor : UnityEditor.Editor
    {
        private DiscordRichPresenceService m_discordManager;

        private void OnEnable()
        {
            m_discordManager = (DiscordRichPresenceService)target;
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
}
