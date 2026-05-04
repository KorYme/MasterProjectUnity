using AdvancedDebugTool;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class SampleDebugToolBehaviour : MonoBehaviour
{
    private enum ExampleEnum
    {
        A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W, X, Y, Z
    }
    
    private DebugTool<SampleDebugMethodAttribute, SampleCategory> m_DebugTool;

    private float m_Timer;
    private bool m_ExampleToggle;
    private string m_ExampleText;
    private ExampleEnum m_ExampleEnum;
    private int m_ExampleInt;
    private float m_ExampleFloat;
    private Vector2 m_ExampleVector2;
    private Vector3 m_ExampleVector3;
    
    private void Awake()
    {
        m_DebugTool = new DebugTool<SampleDebugMethodAttribute, SampleCategory>();
        m_DebugTool.AddObjectToMenu(this);
    }

    private void OnGUI()
    {
        m_DebugTool.DrawOnGUI();
    }

    private void Update()
    {
        m_Timer += Time.deltaTime;
        
        #if ENABLE_INPUT_SYSTEM
        if (Keyboard.current.leftCtrlKey.isPressed && Keyboard.current[Key.D].wasPressedThisFrame)
        {
            m_DebugTool.ToggleDisplay();
        }
        #else
        if (Input.GetKeyDown(KeyCode.D) && Input.GetKey(KeyCode.LeftControl))
        {
            m_DebugTool.ToggleDisplay();
        }
        #endif
    }
    
    [SampleDebugMethod("Introduction")]
    private void WelcomeDisplay(DebugContext debugContext)
    {
        debugContext.DrawLabelBlock("Hey !", $"Welcome to the debug tool!");
    }

    [SampleDebugMethod("Simple label with updating timer")]
    private void TimerDisplay(DebugContext debugContext)
    {
        debugContext.DrawLabelBlock("Timer :", $"{m_Timer}");
    }
    
    [SampleDebugMethod("Toggle example", SampleCategory.TextValues)]
    private void ToggleDebug(DebugContext debugContext)
    {
        if (debugContext.DrawToggle("GodMode", ref m_ExampleToggle))
        {
            Debug.Log("GodMode is " + m_ExampleToggle);
        }
    }
    
    [SampleDebugMethod("Enum dropdown", SampleCategory.TextValues)]
    private void EnumDebug(DebugContext debugContext)
    {
        if (debugContext.DrawEnumDropdown("Enum value", ref m_ExampleEnum))
        {
            Debug.Log(m_ExampleEnum.ToString());
        }
    }
    
    [SampleDebugMethod("TextField + Button", SampleCategory.TextValues)]
    private void TextFieldDebug(DebugContext debugContext)
    {
        debugContext.DrawTextField("Text", ref m_ExampleText);
        if (debugContext.DrawButton("DebugValue"))
        {
            Debug.Log(m_ExampleText);
        }
    }
    
    [SampleDebugMethod("Numeric debugs", SampleCategory.NumericValues)]
    private void NumericDebugs(DebugContext debugContext)
    {
        if (debugContext.DrawIntField("Int Field", ref m_ExampleInt))
        {
        }
        if (debugContext.DrawFloatField("Float Field", ref m_ExampleFloat))
        {
        }
        if (debugContext.DrawVector2Field("Vector2 Field", ref m_ExampleVector2))
        {
        }
        if (debugContext.DrawVector3Field("Vector3 Field", ref m_ExampleVector3))
        {
        }
    }
}
