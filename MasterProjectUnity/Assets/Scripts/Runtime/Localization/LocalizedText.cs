using TMPro;
using UnityEngine;
using MasterProject.Debugging;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MasterProject.Localization
{
    [AddComponentMenu("Localization/LocalizedText")]
    [RequireComponent(typeof(TMP_Text))]
    [DisallowMultipleComponent]
    [ExecuteAlways]
    public class LocalizedText : MonoBehaviour
    {
        private const string LOCALIZATION_HANDLER_NOT_INITIALIZED = "LOCA HANDLER MISSING";

        public delegate string GetLocalizedText(string key, params string[] args);
        public static GetLocalizedText OnLocalizationKeyChanged;
        public delegate bool GetLocalizedFont(string fontKey, FontStyle fontStyle);
        public static GetLocalizedFont OnLocalizationFontChanged;

        [SerializeField] private TMP_Text m_TextComponent;

        [SerializeField] private string m_LocalizationKey;

        [SerializeField] private string[] m_Arguments;

        private void OnEnable()
        {
            LocalizationHandler.RegisterLocalizedText(this);
#if UNITY_EDITOR
            if (!EditorApplication.isPlaying)
            {
                Refresh();
            }
#endif
        }

        private void OnDisable()
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
            SetText(GetLocalizedString());
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
            // A MODIFIER AVEC D'AUTRES COMPONENTS POSSIBLE
        }

        private string GetLocalizedString()
        {
            if (OnLocalizationKeyChanged != null)
            {
                return OnLocalizationKeyChanged(m_LocalizationKey, m_Arguments);
            }
#if UNITY_EDITOR
            if (!EditorApplication.isPlaying)
            {
                return LocalizationHandlerEditor.Instance.GetTextFromKeyAndArgs(m_LocalizationKey, m_Arguments);
            }
#endif
            return LOCALIZATION_HANDLER_NOT_INITIALIZED;
        }
    }
}
