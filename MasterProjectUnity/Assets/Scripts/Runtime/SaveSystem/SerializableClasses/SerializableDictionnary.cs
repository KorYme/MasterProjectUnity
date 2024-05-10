using MasterProject.Debugging;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace KorYmeLibrary.SaveSystem
{
    [Serializable]
    public class SerializableDictionnary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [SerializeField] private List<TKey> m_Keys = new();
        [SerializeField] private List<TValue> m_Values = new();

        public void OnAfterDeserialize()
        {
            Clear();
            if (m_Keys.Count != m_Values.Count)
            {
                DebugLogger.Warning(this, "There is a different number of keys and values in the save file");
                return;
            }
            for (int i = 0; i < m_Keys.Count; i++)
            {
                Add(m_Keys[i], m_Values[i]);
            }
        }

        public void OnBeforeSerialize()
        {
            m_Keys.Clear();
            m_Values.Clear();
            foreach (KeyValuePair<TKey, TValue> kvp in this)
            {
                m_Keys.Add(kvp.Key);
                m_Values.Add(kvp.Value);
            }
        }
    }
}
