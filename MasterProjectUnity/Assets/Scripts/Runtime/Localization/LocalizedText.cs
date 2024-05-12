using TMPro;
using UnityEngine;

namespace MasterProject.Localization
{
    [AddComponentMenu("Localization/LocalizedText")]
    [RequireComponent(typeof(TMP_Text))]
    [DisallowMultipleComponent]
    [ExecuteAlways]
    public class LocalizedText : MonoBehaviour
    {
        private const string LOCALIZATION_HANDLER_NOT_INITIALIZED = "LOCA HANDLER NOT INITIALIZED";
        private const string KEY_NOT_FOUND = "{0} KEY NOT FOUND";
        private const string KEY_EMPTY = "EMPTY KEY";

        public delegate bool GetLocalizedText(string key, out string text);
        public static GetLocalizedText OnLocalizationKeyChanged;
        public delegate bool GetLocalizedFont(string fontKey, FontStyle fontStyle);
        public static GetLocalizedFont OnLocalizationFontChanged;

        [SerializeField] private TMP_Text m_TextComponent;

        [SerializeField] private string m_LocalizationKey;

        [SerializeField] private string[] m_Arguments;

        private void Awake()
        {
            LocalizationHandler.RegisterLocalizedText(this);
        }

        private void OnDestroy()
        {
            LocalizationHandler.UnRegisterLocalizedText(this);
        }

        private void Start()
        {
            Refresh();
        }

        private void Reset()
        {
            m_TextComponent = GetComponent<TMP_Text>();
        }

        private void OnValidate()
        {
            Refresh();
        }

        public void Refresh()
        {
            if (m_Arguments == null || m_Arguments.Length == 0)
            {
                SetText(GetLocalizedString(m_LocalizationKey, true));
            }
            else
            {
                for (int i = 0; i < m_Arguments.Length; i++)
                {
                    m_Arguments[i] = GetLocalizedString(m_Arguments[i], false);
                }
                SetText(string.Format(GetLocalizedString(m_LocalizationKey, true), m_Arguments));
            }
        }

        public void SetLocalizedKey(string key)
        {
            m_LocalizationKey = key;
            Refresh();
        }

        public void SetArguments(params string[] args)
        {
            m_Arguments = args;
            Refresh();
        }

        public void SetLocalizedKeyAndArgs(string key, params string[] ags)
        {
            m_LocalizationKey = key;
            m_Arguments = ags;
            Refresh();
        }

        private void SetText(string text)
        {
            if (m_TextComponent)
            {
                m_TextComponent.SetText(text);
            }
        }

        private string GetLocalizedString(string key, bool forceLocalization = false)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return KEY_EMPTY;
            }
            else if (OnLocalizationKeyChanged != null)
            {
                if (OnLocalizationKeyChanged(key, out string text))
                {
                    return text;
                }
                else
                {
                    return forceLocalization ? string.Format(KEY_NOT_FOUND, key) : key;
                }
            }
            else
            {
                return forceLocalization ? LOCALIZATION_HANDLER_NOT_INITIALIZED : key;
            }
        }
    }
}
