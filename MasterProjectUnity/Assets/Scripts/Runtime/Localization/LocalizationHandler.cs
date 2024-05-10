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

        private const string KEY_NOT_FOUND = "{0} KEY NOT FOUND";
        private const string KEY_EMPTY = "EMPTY KEY";

        private static HashSet<LocalizedText> m_LocalizedTexts = new HashSet<LocalizedText>();

        private Dictionary<string, LanguageData> m_LoadedLanguages = new Dictionary<string, LanguageData>();
        private List<string> m_LanguagesIDs = new List<string>();
        private LanguageData m_CurrentLanguageData;

        public virtual void Initialize()
        {
            m_LoadedLanguages.Clear();
            m_LanguagesIDs.Clear();
            IEnumerable<string> localizationFilesPath = Directory.EnumerateFiles(LocalizationAssetsDirectory, "*.json", SearchOption.AllDirectories);
            foreach (string filePath in localizationFilesPath)
            {
                LanguageData languageData = JsonConvert.DeserializeObject<LanguageData>(File.ReadAllText(filePath));
                if (m_LoadedLanguages.TryAdd(languageData.LanguageID, languageData))
                {
                    m_LanguagesIDs.Add(languageData.LanguageID);
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
            if (m_LoadedLanguages.TryGetValue(m_LanguagesIDs[0], out LanguageData locaData))
            {
                m_CurrentLanguageData = locaData;
            }
            LocalizedText.OnLocalizationKeyChanged += GetTextFromKey;
            Refresh();
        }

        public virtual void Unload()
        {
            LocalizedText.OnLocalizationKeyChanged -= GetTextFromKey;
        }

        public void SetLangage(string languageID)
        {
            if (languageID == m_CurrentLanguageData.LanguageID)
            {
                return;
            }
            if (!m_LoadedLanguages.TryGetValue(languageID, out LanguageData languageData))
            {
                DebugLogger.Warning(this, $"The language with languageId {languageID} doesn't exist");
                return;
            }
            m_CurrentLanguageData = languageData;
            Refresh();
        }

        public string GetTextFromKey(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return KEY_EMPTY;
            }
            if (!m_CurrentLanguageData.LocalizationKeys.TryGetValue(key, out string text))
            {
                return string.Format(KEY_NOT_FOUND, key);
            }
            return text;
        }

        public virtual void Refresh()
        {
            foreach (LocalizedText locaText in m_LocalizedTexts)
            {
                locaText.SetText(GetTextFromKey(locaText.LocalizationKey));
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
