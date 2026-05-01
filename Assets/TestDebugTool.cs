using AdvancedDebugTool;
using UnityEngine;

public class TestDebugTool : MonoBehaviour
{
    private DebugTool m_DebugTool;

    private float m_Timer;

    private void Awake()
    {
        m_DebugTool = new DebugTool(Debug.unityLogger);
        m_DebugTool.AddObjectToMenu(this);
    }

    private void OnGUI()
    {
        m_DebugTool.DrawOnGUI();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.D) && Input.GetKey(KeyCode.LeftControl))
        {
            m_DebugTool.ToggleMenuDisplay();
        }
        m_Timer += Time.deltaTime;
    }
    
    [DebugMethod("Timer method")]
    private void TimerDisplay(DebugContext debugContext)
    {
        debugContext.Label($"{nameof(m_Timer)} : {m_Timer}");
    }
}
