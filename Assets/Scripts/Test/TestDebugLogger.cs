using MasterProject.Debugging;
using System.Collections.Generic;
using UnityEngine;

namespace MasterProject.Tests
{
    public class TestDebugLogger : MonoBehaviour
    {
        [SerializeField] private List<string> m_values;

        private int m_Count = 0;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                DebugLogger.Info(this, m_values[m_Count % m_values.Count]);
                m_Count++;
            }
        }
    }
}