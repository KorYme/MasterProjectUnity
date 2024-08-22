using System.Runtime.InteropServices;
using UnityEngine;

namespace MasterProject.DiscordManager
{
    [CreateAssetMenu(fileName = "discord_activity_setup", menuName = "ScriptableObjects/Discord Activity Setup")]
    public class DiscordActivityDescriptor : ScriptableObject
    {
        public long ApplicationId;

        [Header("Text")]
        public string Name;
        public string State;
        public string Details;

        [Header("Image")]
        public string LargeImage;
        public string LargeText;
        public string SmallImage;
        public string SmallText;
    }
}
