using AdvancedDebugTool;
using UnityEngine;

public class TestDebugTool : MonoBehaviour
{
    private enum TestEnum
    {
        One,
        Two,
    }
    
    private DebugTool<DebugMethodAttribute, DebugCategory> m_DebugTool;

    private float m_Timer;
    
    private bool m_TestToggle;
    private TestEnum m_EnumValue;
    
    private void Awake()
    {
        m_DebugTool = new DebugTool<DebugMethodAttribute, DebugCategory>();
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
        if (debugContext.DrawEnumDropdown("Test 1", ref m_EnumValue))
        {
            
        }
    }
    
    [DebugMethod("Button method", DebugCategory.Gameplay)]
    private void ButtonDisplay(DebugContext debugContext)
    {
        if (debugContext.DrawEnumDropdown("Test 1", ref m_EnumValue))
        {
            
        }
        if (debugContext.DrawEnumDropdown("Test 2", ref m_EnumValue))
        {
            
        }
        if (debugContext.DrawButton("DebugValue"))
        {
            Debug.Log(m_EnumValue.ToString());
        }
    }
}
