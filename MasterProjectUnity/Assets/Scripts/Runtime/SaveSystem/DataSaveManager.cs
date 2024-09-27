using MasterProject.Debugging;
using MasterProject.Utilities;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MasterProject.SaveSystem
{
    public class DataSaveManager<T> : MonoBehaviour where T : BaseGameData, new()
    {
        #region FIELDS
        public static DataSaveManager<T> Instance { get; private set; }

        protected T m_gameData = null;
        protected List<IDataSaveable<T>> m_allSaveData
        {
            get => FindObjectsOfType<MonoBehaviour>().OfType<IDataSaveable<T>>().ToList();
        }
        protected SaveFileDataHandler<T> m_saveFileDataHandler;
        protected bool m_dataHasBeenLoaded;

        [Header("File Storage Config")]
        [SerializeField] protected string m_fileName;
        [SerializeField] protected EncryptionUtilities.EncryptionType m_encryptionType;

        [Header("InGame parameters")]
        [SerializeField] protected bool m_saveOnQuit;
        #endregion

        #region METHODS
        private void Awake()
        {
            if (Instance != null)
            {
                DebugLogger.Warning(this, "There is more than one DataSaveManager of this type in the scene");
                return;
            }
            Instance = this;
            m_saveFileDataHandler = new SaveFileDataHandler<T>(m_fileName, m_encryptionType);
            m_dataHasBeenLoaded = false;
            LoadGame();
        }

        private void Reset()
        {
            m_fileName = "data.json";
            m_saveOnQuit = true;
        }

#if UNITY_EDITOR || UNITY_STANDALONE
        private void OnApplicationQuit()
        {
            if (!m_saveOnQuit) return;
            SaveGame();
        }
#endif

#if UNITY_ANDROID || UNITY_IOS
        private void OnApplicationFocus(bool focus)
        {
            if (!m_saveOnQuit) return;
            if (focus)
            {
                LoadGame();
            }
            else
            {
                SaveGame();
            }
        }
#endif

        public void NewGame()
        {
            m_gameData = new T();
        }

        public void LoadGame(bool isLoadForced = false)
        {
            if (m_dataHasBeenLoaded && !isLoadForced) return;
            m_dataHasBeenLoaded = true;
            m_gameData = m_saveFileDataHandler.Load();
            if (m_gameData == null)
            {
                DebugLogger.Warning(this, "No data was found. Initializing with defaults data.");
                NewGame();
                return;
            }
            m_allSaveData.ForEach(x => x.LoadData(m_gameData));
        }

        public void SaveGame()
        {
            m_allSaveData.ForEach(x => x.SaveData(ref m_gameData));
            m_saveFileDataHandler.Save(m_gameData);
            m_dataHasBeenLoaded = false;
        }

        public virtual void DestroySavedData()
        {
            m_saveFileDataHandler.DestroySavedData();
        }
#endregion
    }
}
