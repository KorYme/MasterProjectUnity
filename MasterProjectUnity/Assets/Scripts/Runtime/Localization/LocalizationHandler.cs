using MasterProject.Debugging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace MasterProject.Localization
{
    public class LocalizationHandler
    {
        private const string KEY_NOT_FOUND = "{0} NOT FOUND";
        private const string NO_LOCA_DATA_FOUND = "NO LOCALIZATION DATA FOUND";
        private const string KEY_EMPTY = "EMPTY KEY";

        private const string FILE_NAME_TEMPLATE = "localization_data_{0}.json";

        public static readonly string LocalizationAssetsDirectory = Path.Combine(Application.streamingAssetsPath, "LocalizationAssets");

        protected static HashSet<LocalizedText> s_LocalizedTexts = new HashSet<LocalizedText>();

        protected Dictionary<string, LocalizationData> m_LoadedLanguages;
        protected List<string> m_LanguagesIDs;
        public IReadOnlyList<string> LanguagesIDs => m_LanguagesIDs;

        protected LocalizationData m_CurrentLocaData;

        public virtual void Initialize()
        {
#if UNITY_EDITOR
            if (this is not LocalizationHandlerEditor)
            {
                LocalizationHandlerEditor.Clear();
            }
#endif
            m_LoadedLanguages = new Dictionary<string, LocalizationData>();
            m_LanguagesIDs = new List<string>();
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
                LocalizedText.OnLocalizationKeyChanged = GetTextFromKeyAndArgs;
                SetLocalizationData(locaData);
            }
        }

        public virtual void Unload()
        {
            LocalizedText.OnLocalizationKeyChanged = null;
        }

        public void SetLocalizationLanguage(string languageID)
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

        protected void SetLocalizationData(LocalizationData locaData)
        {
            m_CurrentLocaData = locaData;
            RefreshLocalizedTexts();
        }

        public IEnumerable<string> GetLocalization()
        {
            return m_LanguagesIDs;
        }

        public string GetTextFromKeyAndArgs(string key, params string[] args)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return KEY_EMPTY;
            }
            if (m_CurrentLocaData.LocalizationKeys == null)
            {
                return NO_LOCA_DATA_FOUND;
            }
            if (!m_CurrentLocaData.LocalizationKeys.TryGetValue(key, out string localizedText))
            {
                return string.Format(KEY_NOT_FOUND, key);
            }
            else
            {
                if (args == null || args.Length == 0)
                {
                    return localizedText;
                }
                string[] localizedArguments = new string[args.Length];
                for (int index = 0; index < args.Length; index++)
                {
                    if (m_CurrentLocaData.LocalizationKeys.TryGetValue(args[index], out string localizedArg))
                    {
                        localizedArguments[index] = localizedArg;
                    }
                    else
                    {
                        localizedArguments[index] = args[index];
                    }
                }
                return string.Format(localizedText, localizedArguments);        
            }
        }

        public virtual void RefreshLocalizedTexts()
        {
            foreach (LocalizedText locaText in s_LocalizedTexts)
            {
                locaText.Refresh();
            }
        }

        public static void RegisterLocalizedText(LocalizedText locaText)
        {
            s_LocalizedTexts.Add(locaText);
        }

        public static void UnRegisterLocalizedText(LocalizedText locaText)
        {
            s_LocalizedTexts.Remove(locaText);
        }

        public static string GetLocalizationAssetFullPath(string languageName)
        {
            return Path.Combine(LocalizationAssetsDirectory, string.Format(FILE_NAME_TEMPLATE, languageName.ToLower()));
        }
    }
}
