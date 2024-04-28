using MasterProject.Debugging;
using System.Collections.Generic;
using UnityEngine;

public class TestDebugLogger : MonoBehaviour
{
    [SerializeField] private List<string> Values;

    private int m_Count = 0;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            DebugLogger.Info(Values[m_Count % Values.Count], "Ceci est un test !");
            m_Count++;
        }
    }
}
