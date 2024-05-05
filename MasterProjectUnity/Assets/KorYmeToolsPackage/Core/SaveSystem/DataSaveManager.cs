using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;

namespace KorYmeLibrary.SaveSystem
{
    public class DataSaveManager<T> : MonoBehaviour where T : GameDataTemplate, new()
    {
        #region FIELDS
        public static DataSaveManager<T> Instance { get; private set; }

        protected T _gameData = null;
        protected List<IDataSaveable<T>> AllSaveData
        {
            get => FindObjectsOfType<MonoBehaviour>().OfType<IDataSaveable<T>>().ToList();
        }
        protected SaveFileDataHandler<T> _saveFileDataHandler;
        protected bool _dataHasBeenLoaded;

        [Header("File Storage Config")]
        [SerializeField] protected string _fileName;
        [SerializeField] protected EncryptionUtilities.EncryptionType _encryptionType;

        [Header("InGame parameters")]
        [SerializeField] protected bool _saveOnQuit;
        #endregion

        #region METHODS
        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogWarning("There is more than one DataSaveManager of this type in the scene");
                return;
            }
            Instance = this;
            _saveFileDataHandler = new SaveFileDataHandler<T>(Application.persistentDataPath, _fileName, _encryptionType);
            _dataHasBeenLoaded = false;
            LoadGame();
        }

        private void Reset()
        {
            _fileName = "data.json";
            _saveOnQuit = true;
        }

#if UNITY_EDITOR || UNITY_STANDALONE
        private void OnApplicationQuit()
        {
            if (!_saveOnQuit) return;
            SaveGame();
        }
#endif

#if UNITY_ANDROID || UNITY_IOS
        private void OnApplicationFocus(bool focus)
        {
            if (!_saveOnQuit) return;
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
            _gameData = new T();
            AllSaveData.ForEach(x => x.InitializeData());
        }

        public void LoadGame(bool isLoadForced = false)
        {
            if (_dataHasBeenLoaded && !isLoadForced) return;
            _dataHasBeenLoaded = true;
            _gameData = _saveFileDataHandler.Load();
            if (_gameData == null)
            {
                Debug.LogWarning("No data was found. Initializing with defaults data.");
                NewGame();
                return;
            }
            AllSaveData.ForEach(x => x.LoadData(_gameData));
        }

        public void SaveGame()
        {
            AllSaveData.ForEach(x => x.SaveData(ref _gameData));
            _saveFileDataHandler.Save(_gameData);
            _dataHasBeenLoaded = false;
        }

        [MenuItem("CONTEXT/DataSaveManager/Delete saved data")]
        public void DestroySavedData()
        {
            SaveFileDataHandler<T>.DestroySavedData(Path.Combine(Application.persistentDataPath, _fileName));
        }
#endregion
    }
}
