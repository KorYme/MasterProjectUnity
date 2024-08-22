using TMPro;
using UnityEngine;

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

        [SerializeField] private TMP_Text m_textComponent;

        [SerializeField] private string m_localizationKey;

        [SerializeField] private string[] m_arguments;

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
            m_textComponent = GetComponent<TMP_Text>();
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
            m_localizationKey = key;
            Refresh();
        }

        public void SetArguments(params string[] args)
        {
            m_arguments = args;
            Refresh();
        }

        public void SetLocalizedKeyAndArgs(string key, params string[] ags)
        {
            m_localizationKey = key;
            m_arguments = ags;
            Refresh();
        }

        private void SetText(string text)
        {
            if (m_textComponent)
            {
                m_textComponent.SetText(text);
            }
            // A MODIFIER AVEC D'AUTRES COMPONENTS POSSIBLE
        }

        private string GetLocalizedString()
        {
            if (OnLocalizationKeyChanged != null)
            {
                return OnLocalizationKeyChanged(m_localizationKey, m_arguments);
            }
#if UNITY_EDITOR
            if (!EditorApplication.isPlaying)
            {
                return LocalizationHandlerEditor.Instance.GetTextFromKeyAndArgs(m_localizationKey, m_arguments);
            }
#endif
            return LOCALIZATION_HANDLER_NOT_INITIALIZED;
        }
    }
}
