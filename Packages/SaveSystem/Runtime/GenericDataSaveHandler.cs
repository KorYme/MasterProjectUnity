using System;
using EncryptionUtils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SaveSystem
{
    [Flags]
    public enum SaveEvent
    {
        None = 0,
        OnApplicationQuit = 1 << 0,
        OnApplicationLostFocus = 1 << 1,
        OnDestroy = 1 << 2,
    }

    [Flags]
    public enum LoadEvent
    {
        None = 0,
        Awake = 1 << 0,
        Start = 1 << 1,
        OnEnable = 1 << 2,
        OnApplicationGainFocus = 1 << 3
    }
    
    public class GenericDataSaveHandler<T> : MonoBehaviour where T : new()
    {
        #region FIELDS
        public T GameData { get; protected set; }

        [Header("File Storage Config")]
        [SerializeField] protected DataSaveFileHandler<T> _dataSaveFileHandler;

        [Header("InGame Parameters")]
        [SerializeField] protected SaveEvent _saveEvent = SaveEvent.OnApplicationQuit;
        [SerializeField] protected LoadEvent _loadEvent = LoadEvent.Awake;
        #endregion

        #region METHODS
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
        
        public void SaveGame()
        {
            _dataSaveFileHandler.Save(GameData);
        }
        
        public void GenerateNewGameData()
        {
            GameData = new T();
        }
        
        protected virtual void Awake()
        {
            if (_loadEvent.HasFlag(LoadEvent.Awake))
            {
                LoadGame();
            }
        }

        protected virtual void Start()
        {
            if (_loadEvent.HasFlag(LoadEvent.Start))
            {
                LoadGame();
            }
        }

        protected virtual void OnEnable()
        {
            if (_loadEvent.HasFlag(LoadEvent.OnEnable))
            {
                LoadGame();
            }
        }

        protected virtual void OnDestroy()
        {
            if (_saveEvent.HasFlag(SaveEvent.OnDestroy))
            {
                SaveGame();
            }
        }

        protected virtual void OnApplicationQuit()
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            if (_saveEvent.HasFlag(SaveEvent.OnApplicationQuit))
            {
                SaveGame();
            }
#endif
        }

        protected virtual void OnApplicationFocus(bool focus)
        {
            if (focus && _loadEvent.HasFlag(LoadEvent.OnApplicationGainFocus))
            {
                LoadGame();
            }
#if !UNITY_ANDROID || UNITY_IOS
            // OnApplicationQuit is not called on mobile devices
            else if (!focus && (_saveEvent.HasFlag(SaveEvent.OnApplicationLostFocus) || _saveEvent.HasFlag(SaveEvent.OnApplicationQuit)))
            {
                SaveGame();
            }
#else
            else if (!focus && _saveEvent.HasFlag(SaveEvent.OnApplicationLostFocus))
            {
                SaveGame();
            }
#endif
        }
        #endregion
    }
}
