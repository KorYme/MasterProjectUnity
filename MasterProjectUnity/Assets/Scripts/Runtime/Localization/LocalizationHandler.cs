using MasterProject.Debugging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace MasterProject.Localization
{
    public class LocalizationHandler
    {
        public static readonly string LocalizationAssetsDirectory = Path.Combine(Application.streamingAssetsPath, "LocalizationAssets");

        private static HashSet<LocalizedText> m_LocalizedTexts = new HashSet<LocalizedText>();

        private Dictionary<string, LocalizationData> m_LoadedLanguages = new Dictionary<string, LocalizationData>();
        private List<string> m_LanguagesIDs = new List<string>();
        private LocalizationData m_CurrentLocaData;

        public virtual void Initialize()
        {
            m_LoadedLanguages.Clear();
            m_LanguagesIDs.Clear();
            IEnumerable<string> localizationFilesPath = Directory.EnumerateFiles(LocalizationAssetsDirectory, "*.json", SearchOption.AllDirectories);
            foreach (string filePath in localizationFilesPath)
            {
                LocalizationData localizationData = JsonConvert.DeserializeObject<LocalizationData>(File.ReadAllText(filePath));
                if (m_LoadedLanguages.TryAdd(localizationData.LanguageID, localizationData))
                {
                    m_LanguagesIDs.Add(localizationData.LanguageID);
                }
                else
                {
                    DebugLogger.Error(this, $"The file {localizationFilesPath} has the same LangageID as another localization asset, it will not be considered.");
                }
            }
            if (m_LanguagesIDs.Count == 0)
            {
                throw new System.ArgumentOutOfRangeException($"The {LocalizationAssetsDirectory} folder doesn't countain any localization asset");
            }
            if (m_LoadedLanguages.TryGetValue(m_LanguagesIDs[0], out LocalizationData locaData))
            {
                m_CurrentLocaData = locaData;
                LocalizedText.OnLocalizationKeyChanged = GetTextFromKey;
                SetLocalizationData(locaData);
            }
        }

        public virtual void Unload()
        {
            LocalizedText.OnLocalizationKeyChanged = null;
        }

        public void SetLocalizationData(string languageID)
        {
            if (languageID == m_CurrentLocaData.LanguageID)
            {
                return;
            }
            if (!m_LoadedLanguages.TryGetValue(languageID, out LocalizationData locaData))
            {
                DebugLogger.Warning(this, $"The language with languageId {languageID} doesn't exist");
                return;
            }
            SetLocalizationData(locaData);
        }

        public void SetLocalizationData(LocalizationData locaData)
        {
            m_CurrentLocaData = locaData;
            Refresh();
        }

        public bool GetTextFromKey(string key, out string text)
        {
            return m_CurrentLocaData.LocalizationKeys.TryGetValue(key, out text);
        }

        public virtual void Refresh()
        {
            foreach (LocalizedText locaText in m_LocalizedTexts)
            {
                locaText.Refresh();
            }
        }

        public static void RegisterLocalizedText(LocalizedText locaText)
        {
            m_LocalizedTexts.Add(locaText);
        }

        public static void UnRegisterLocalizedText(LocalizedText locaText)
        {
            m_LocalizedTexts.Remove(locaText);
        }

        public static string GetLocalizationAssetFullPath(string languageName)
        {
            return Path.Combine(LocalizationAssetsDirectory, "localization_data_" + languageName.ToLower() + ".json");
        }
    }
}
