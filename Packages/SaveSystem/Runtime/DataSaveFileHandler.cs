using SerializationUtils;
using System;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace SaveSystem
{
    [Serializable]
    public class DataSaveFileHandler<T> where T : new()
    {
        #region CONSTS
        private const string DEFAULT_FILE_NAME = "data.json";
        #endregion
        
        #region FIELDS
        [SerializeField] private string _dataFileName;
        [SerializeField] private string _encryptionString;
        [SerializeField] private EncryptionUtilities.EncryptionType _encryptionType;
        #endregion

        #region PROPERTIES
        private string DataFileName => string.IsNullOrEmpty(_dataFileName) ? DEFAULT_FILE_NAME : _dataFileName;
        
        private string FullPath
        {
            get => Path.Combine(Application.persistentDataPath, nameof(SaveSystem), DataFileName);
        }
        #endregion
        
        #region METHODS
        public bool TryLoad(out T data)
        {
            if (!File.Exists(FullPath))
            {
                Debug.LogError("No data has been found, please ensure you've saved before loading.");
                data = default;
                return false;
            }
            try
            {
                using FileStream stream = new FileStream(FullPath, FileMode.Open);
                using StreamReader reader = new StreamReader(stream);
                string dataToLoad = EncryptionUtilities.Encrypt(reader.ReadToEnd(), _encryptionType, false, _encryptionString);
                data = JsonConvert.DeserializeObject<T>(dataToLoad);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogWarning("Error occured when trying to save data to file: " + FullPath + "\n" + e);
                data = default;
                return false;
            }
        }

        public void Save(T data)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(FullPath) ?? throw new InvalidOperationException());
                using FileStream stream = new FileStream(FullPath, FileMode.Create);
                using StreamWriter writer = new StreamWriter(stream);
                writer.Write(EncryptionUtilities.Encrypt(JsonConvert.SerializeObject(data, Formatting.Indented), _encryptionType, true, _encryptionString));
            }
            catch (Exception e)
            {
                Debug.LogError("Error occured when trying to save data to file: " + FullPath + "\n" + e);
            }
        }

        public void DestroySavedData()
        {
            if (!File.Exists(FullPath))
            {
                Debug.LogError($"The file you tried to destroy with path {FullPath} didn't exist");
            }
            File.Delete(FullPath);
        }
        #endregion
    }
}
