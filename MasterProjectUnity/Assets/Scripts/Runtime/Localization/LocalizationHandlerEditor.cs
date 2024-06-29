#if UNITY_EDITOR
namespace MasterProject.Localization
{
    /// <summary>
    /// Localization Handler only used in Editor, make sure you use this class non-built code
    /// </summary>
    public class LocalizationHandlerEditor : LocalizationHandler
    {
        private static LocalizationHandlerEditor s_instance;
        public static LocalizationHandlerEditor Instance
        {
            get
            {
                if (s_instance == null)
                {
                    s_instance = new LocalizationHandlerEditor();
                    s_instance.Initialize();
                }
                return s_instance;
            }
        }

        public static void Clear()
        {
            s_instance = null;
        }
    }
}
#endif