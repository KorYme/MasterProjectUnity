using UnityEngine;

namespace AdvancedDebugTool
{
    public class DebugTool
    {
        private DebugToolHandler m_DebugToolHandler;
        private DebugToolView m_DebugToolView;
        
        public DebugTool(ILogger logger = null)
        {
            m_DebugToolHandler = new DebugToolHandler(logger);
            m_DebugToolView = new DebugToolView(m_DebugToolHandler);
        }

        #region CONTROLLER_PART

        public bool AddObjectToMenu(object objectToDebug) => m_DebugToolHandler.AddObjectToMenu(objectToDebug);
        
        public bool RemoveObjectFromMenu(object objectToDebug) => m_DebugToolHandler.RemoveObjectFromMenu(objectToDebug);

        #endregion

        #region VIEW_PART

        public void DrawOnGUI() => m_DebugToolView.OnGUI();
        
        public void ToggleMenuDisplay() => m_DebugToolView.ToggleMenuDisplay();
        
        #endregion
    }
}