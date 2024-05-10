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
        public delegate string GetLocalizedText(string key);
        public static GetLocalizedText OnLocalizationKeyChanged;
        public delegate void GetLocalizedFont(string fontKey, FontStyle fontStyle);
        public static GetLocalizedFont OnLocalizationFontChanged;

        [SerializeField] private TMP_Text m_TextComponent;

        [SerializeField] private string m_LocalizationKey;
        public string LocalizationKey => m_LocalizationKey;

        private void Awake()
        {
            LocalizationHandler.RegisterLocalizedText(this);

        }

        private void OnDestroy()
        {
            LocalizationHandler.UnRegisterLocalizedText(this);
        }

        private void Reset()
        {
            m_TextComponent = GetComponent<TMP_Text>();
        }

        private void OnValidate()
        {
            if (m_TextComponent)
            {
                SetText(m_LocalizationKey); // A REVOIR
            }
        }

        public void SetText(string text)
        {
            m_TextComponent.SetText(text);
        }
    }
}
