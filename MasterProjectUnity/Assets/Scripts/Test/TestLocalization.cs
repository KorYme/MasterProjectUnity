using MasterProject.Localization;
using UnityEngine;

namespace MasterProject.Tests
{
    public class TestLocalization : MonoBehaviour
    {
        private LocalizationHandler m_LocalizationHandler;
        [SerializeField] private string m_LanguageId;

        private void Start()
        {
            m_LocalizationHandler = new LocalizationHandler();
            m_LocalizationHandler.Initialize();
        }

        private void OnDestroy()
        {
            m_LocalizationHandler?.Unload();
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(1))
            {
                m_LocalizationHandler.SetLocalizationData(m_LanguageId);
            }
        }
    }
}
