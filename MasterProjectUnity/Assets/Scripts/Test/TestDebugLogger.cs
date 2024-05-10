using MasterProject.Debugging;
using System.Collections.Generic;
using UnityEngine;

namespace MasterProject.Tests
{
    public class TestDebugLogger : MonoBehaviour
    {
        [SerializeField] LayerMask debugLogLevel;
        [SerializeField] private List<string> Values;
        List<int> test = new List<int>();

        private int m_Count = 0;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                DebugLogger.Info(/*Values[m_Count % Values.Count],*/ this, "Ceci est un test !");
                m_Count++;
            }
        }
    }
}