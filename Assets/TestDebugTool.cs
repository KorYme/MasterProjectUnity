using System;
using AdvancedDebugTool;
using UnityEngine;

public class TestDebugTool : MonoBehaviour
{
    private DebugTool m_DebugTool;

    private float m_Timer;
    
    private bool m_TestToggle;

    private void Awake()
    {
        m_DebugTool = new DebugTool();
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
            m_DebugTool.ToggleDisplay();
        }
        m_Timer += Time.deltaTime;
    }

    [DebugMethod("Timer method")]
    private void TimerDisplay(DebugContext debugContext)
    {
        debugContext.DrawLabelBlock("Test Label", $"Timer: {m_Timer}");
    }
    
    [DebugMethod("Toggle method")]
    private void ToggleDisplay(DebugContext debugContext)
    {
        m_TestToggle = debugContext.DrawToggle("Test Toggle", m_TestToggle);
    }
}
