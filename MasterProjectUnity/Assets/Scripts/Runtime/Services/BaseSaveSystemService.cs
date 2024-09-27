using MasterProject.Debugging;
using MasterProject.SaveSystem;
using MasterProject.Utilities;
using System.Collections.Generic;
using UnityEngine;

namespace MasterProject.Services
{
    public abstract class BaseSaveSystemService<T> : BaseService, ISaveSystemService<T> where T : BaseGameData, new()
    {
        protected T m_savedData;

        protected HashSet<IDataSaveable<T>> m_dataSaveables;

        protected SaveFileDataHandler<T> m_saveFileDataHandler;
        protected bool m_dataHasBeenLoaded;

        [Header("File Storage Config")]
        [SerializeField] protected string m_fileName;
        [SerializeField] protected EncryptionUtilities.EncryptionType m_encryptionType;

        [Header("InGame parameters")]
        [SerializeField] protected bool m_saveOnQuit;

        public override void Initialize()
        {
            base.Initialize();
            m_dataSaveables = new HashSet<IDataSaveable<T>>();
            m_saveFileDataHandler = new SaveFileDataHandler<T>(m_fileName, m_encryptionType);
            m_dataHasBeenLoaded = false;
            LoadData();
        }

        public virtual void Register(IDataSaveable<T> dataSaveable)
        {
            m_dataSaveables.Add(dataSaveable);
            if (m_dataHasBeenLoaded)
            {
                dataSaveable.LoadData(m_savedData);
            }
        }

        public virtual void Unregister(IDataSaveable<T> dataSaveable)
        {
            if (m_dataHasBeenLoaded)
            {
                dataSaveable.SaveData(ref m_savedData);
            }
            m_dataSaveables.Remove(dataSaveable);
        }

        public abstract T InitializeData();

        public virtual void LoadData(bool isLoadForced = false)
        {
            if (m_dataHasBeenLoaded && !isLoadForced) return;
            m_dataHasBeenLoaded = true;
            m_savedData = m_saveFileDataHandler.Load();
            if (m_savedData == null)
            {
                DebugLogger.Info(this, "No data was found. Initializing with defaults data.");
                m_savedData = InitializeData();
                return;
            }
            foreach (IDataSaveable<T> dataSaveable in m_dataSaveables)
            {
                dataSaveable?.LoadData(m_savedData);
            }
        }

        public virtual void SaveData()
        {
            foreach (IDataSaveable<T> dataSaveable in m_dataSaveables)
            {
                dataSaveable?.SaveData(ref m_savedData);
            }
            m_saveFileDataHandler.Save(m_savedData);
        }

#if UNITY_EDITOR || UNITY_STANDALONE
        protected virtual void OnApplicationQuit()
        {
            if (!m_saveOnQuit) return;
            SaveData();
        }
#endif

#if UNITY_ANDROID || UNITY_IOS
        protected virtual void OnApplicationFocus(bool focus)
        {
            if (!m_saveOnQuit || focus) return;
            SaveData();
        }
#endif
    }
}
