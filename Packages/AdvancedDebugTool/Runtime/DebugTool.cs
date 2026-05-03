using System;

namespace AdvancedDebugTool
{
    public class DebugTool<TAttribute, TEnum> where TAttribute : DebugMethodBaseAttribute where TEnum : Enum
    {
        private DebugToolReflector<TAttribute, TEnum> m_DebugToolReflector;
        private DebugToolView<TEnum> m_DebugToolView;
        private bool m_IsDisplayed;
        
        public DebugTool()
        {
            m_DebugToolReflector = new DebugToolReflector<TAttribute, TEnum>();
            m_DebugToolView = new DebugToolView<TEnum>(m_DebugToolReflector);
            m_DebugToolView.OnCloseRequest += Hide;
        }

        #region CONTROLLER_PART

        public bool AddObjectToMenu(object objectToDebug) => m_DebugToolReflector.AddObjectToMenu(objectToDebug);
        
        public bool RemoveObjectFromMenu(object objectToDebug) => m_DebugToolReflector.RemoveObjectFromMenu(objectToDebug);

        #endregion

        #region VIEW_PART

        public void DrawOnGUI()
        {
            if (m_IsDisplayed)
            {
                m_DebugToolView.OnGUI();
            }
        } 
        
        public void ToggleDisplay() => DisplayMenu(!m_IsDisplayed);

        public void Show() => DisplayMenu(true);
        public void Hide() =>  DisplayMenu(false);
        
        private void DisplayMenu(bool display)
        {
            m_IsDisplayed = display;
        }
        
        #endregion
    }
}