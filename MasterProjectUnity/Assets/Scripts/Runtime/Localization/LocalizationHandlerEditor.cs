#if UNITY_EDITOR
namespace MasterProject.Localization
{
    /// <summary>
    /// Localization Handler only used in Editor, make sure you use this class non-built code
    /// </summary>
    public class LocalizationHandlerEditor : LocalizationHandler
    {
        private static LocalizationHandlerEditor m_Instance;
        public static LocalizationHandlerEditor Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    m_Instance = new LocalizationHandlerEditor();
                    m_Instance.Initialize();
                }
                return m_Instance;
            }
        }

        public static void Clear()
        {
            m_Instance = null;
        }
    }
}
#endif