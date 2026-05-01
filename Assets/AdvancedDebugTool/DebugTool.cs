using UnityEngine;

namespace AdvancedDebugTool
{
    public class DebugTool
    {
        private DebugToolReflector m_DebugToolReflector;
        private DebugToolView m_DebugToolView;
        private bool m_IsDisplayed;
        
        public DebugTool()
        {
            m_DebugToolReflector = new DebugToolReflector();
            m_DebugToolView = new DebugToolView(m_DebugToolReflector);
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