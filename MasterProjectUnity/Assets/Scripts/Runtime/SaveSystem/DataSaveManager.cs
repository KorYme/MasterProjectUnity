using MasterProject.Debugging;
using MasterProject.Utilities;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace MasterProject.SaveSystem
{
    public class DataSaveManager<T> : MonoBehaviour where T : GameDataTemplate, new()
    {
        private static readonly string TAG = "DataSaveManager";

        #region FIELDS
        public static DataSaveManager<T> Instance { get; private set; }

        protected T m_GameData = null;
        protected List<IDataSaveable<T>> m_AllSaveData
        {
            get => FindObjectsOfType<MonoBehaviour>().OfType<IDataSaveable<T>>().ToList();
        }
        protected SaveFileDataHandler<T> m_SaveFileDataHandler;
        protected bool m_DataHasBeenLoaded;

        [Header("File Storage Config")]
        [SerializeField] protected string m_FileName;
        [SerializeField] protected EncryptionUtilities.EncryptionType m_EncryptionType;

        [Header("InGame parameters")]
        [SerializeField] protected bool m_SaveOnQuit;
        #endregion

        #region METHODS
        private void Awake()
        {
            if (Instance != null)
            {
                DebugLogger.Warning(TAG, "There is more than one DataSaveManager of this type in the scene");
                return;
            }
            Instance = this;
            m_SaveFileDataHandler = new SaveFileDataHandler<T>(Application.persistentDataPath, m_FileName, m_EncryptionType);
            m_DataHasBeenLoaded = false;
            LoadGame();
        }

        private void Reset()
        {
            m_FileName = "data.json";
            m_SaveOnQuit = true;
        }

#if UNITY_EDITOR || UNITY_STANDALONE
        private void OnApplicationQuit()
        {
            if (!m_SaveOnQuit) return;
            SaveGame();
        }
#endif

#if UNITY_ANDROID || UNITY_IOS
        private void OnApplicationFocus(bool focus)
        {
            if (!m_SaveOnQuit) return;
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
            m_GameData = new T();
            m_AllSaveData.ForEach(x => x.InitializeData());
        }

        public void LoadGame(bool isLoadForced = false)
        {
            if (m_DataHasBeenLoaded && !isLoadForced) return;
            m_DataHasBeenLoaded = true;
            m_GameData = m_SaveFileDataHandler.Load();
            if (m_GameData == null)
            {
                DebugLogger.Warning(TAG, "No data was found. Initializing with defaults data.");
                NewGame();
                return;
            }
            m_AllSaveData.ForEach(x => x.LoadData(m_GameData));
        }

        public void SaveGame()
        {
            m_AllSaveData.ForEach(x => x.SaveData(ref m_GameData));
            m_SaveFileDataHandler.Save(m_GameData);
            m_DataHasBeenLoaded = false;
        }

        public virtual void DestroySavedData()
        {
            m_SaveFileDataHandler.DestroySavedData();
        }
#endregion
    }
}
