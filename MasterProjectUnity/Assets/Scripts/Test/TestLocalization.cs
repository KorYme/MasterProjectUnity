using MasterProject.Localization;
using UnityEngine;

namespace MasterProject.Tests
{
    public class TestLocalization : MonoBehaviour
    {
        public LocalizationHandler LocalizationHandler;
        [SerializeField] private string m_languageId;

        private void Start()
        {
            LocalizationHandler = new LocalizationHandler();
            LocalizationHandler.Initialize();
        }

        private void OnDestroy()
        {
            LocalizationHandler?.Unload();
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(1))
            {
                LocalizationHandler.SetLocalizationLanguage(m_languageId);
            }
        }
    }
}
