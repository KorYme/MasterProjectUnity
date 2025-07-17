using System.Collections.Generic;
using UnityEngine;

namespace KorYmeLibrary.SaveSystem
{
    [System.Serializable]
    public class SerializableDictionnary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [SerializeField] private List<TKey> m_keys = new();
        [SerializeField] private List<TValue> m_values = new();

        public void OnAfterDeserialize()
        {
            Clear();
            if (m_keys.Count != m_values.Count)
            {
                Debug.LogWarning("There is a different number of keys and values in the save file");
                return;
            }
            for (int i = 0; i < m_keys.Count; i++)
            {
                Add(m_keys[i], m_values[i]);
            }
        }

        public void OnBeforeSerialize()
        {
            m_keys.Clear();
            m_values.Clear();
            foreach (KeyValuePair<TKey, TValue> kvp in this)
            {
                m_keys.Add(kvp.Key);
                m_values.Add(kvp.Value);
            }
        }
    }
}
