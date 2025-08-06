using System;
using EncryptionUtils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SaveSystem
{
    public class GenericDataSaveHandler<T> : MonoBehaviour where T : new()
    {
        #region FIELDS
        public T GameData { get; protected set; }

        [Header("File Storage Config")]
        [SerializeField] protected DataSaveFileHandler<T> _dataSaveFileHandler;

        [Header("InGame Parameters")]
        [SerializeField] protected bool _saveOnQuit;
        #endregion

        #region METHODS
        protected virtual void Awake()
        {
            LoadGame();
        }

#if UNITY_EDITOR
        protected virtual void Reset()
        {
            _saveOnQuit = true;
        }
#endif

#if UNITY_EDITOR || UNITY_STANDALONE
        protected virtual void OnApplicationQuit()
        {
            if (!_saveOnQuit) return;
            SaveGame();
        }
#endif

#if UNITY_ANDROID || UNITY_IOS
        protected virtual void OnApplicationFocus(bool focus)
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
        
        public void LoadGame()
        {
            if (!_dataSaveFileHandler.TryLoad(out T data))
            {
                Debug.LogWarning("No data was found. Initializing with defaults data.");
                GenerateNewGameData();
                return;
            }
            GameData = data;
        }

        public void GenerateNewGameData()
        {
            GameData = new T();
        }
        
        public void SaveGame()
        {
            _dataSaveFileHandler.Save(GameData);
        }
        #endregion
    }
}
