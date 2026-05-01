using UnityEngine;

namespace AdvancedDebugTool
{
    public class DebugContext
    {
        public void Label(string label)
        {
            GUILayout.Label(label);
        }
    }
    
    public class DebugToolView
    {
        private DebugContext m_DebugContext;
        private object[] m_Arguments;
        private IDebugInfoProvider m_DebugInfoProvider;

        private bool m_IsHidden = true;

        public DebugToolView(IDebugInfoProvider debugInfoProvider)
        {
            m_DebugInfoProvider = debugInfoProvider;
            
            m_DebugContext = new DebugContext();
            m_Arguments = new object[] { m_DebugContext };
        }
        
        public void OnGUI()
        {
            if (m_IsHidden) return;
            
            foreach (DebugTypeDefinition definition in m_DebugInfoProvider.GetDebugInfos())
            {
                definition.Instances.RemoveWhere(obj => obj == null);
                foreach (object obj in definition.Instances)
                {
                    foreach (DebugMethod method in definition.Methods)
                    {
                        GUILayout.BeginVertical();
                        method.Invoke(obj, m_Arguments);
                        GUILayout.EndVertical();
                    }
                }
            }
        }

        public void ToggleMenuDisplay()
        {
            m_IsHidden = !m_IsHidden;
        }
    }
}