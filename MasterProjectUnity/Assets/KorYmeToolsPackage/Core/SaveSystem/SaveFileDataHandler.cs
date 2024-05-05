using System;
using System.IO;
using Unity.Plastic.Newtonsoft.Json;
using UnityEngine;

namespace KorYmeLibrary.SaveSystem
{
    public class SaveFileDataHandler<T>
    {
        #region FIELDS
        private string _dataDirPath;
        private string _dataFileName;
        private EncryptionUtilities.EncryptionType _encryptionType;
        private string _encryptionString;
        #endregion

        #region PROPERTIES
        string _fullPath
        {
            get => Path.Combine(_dataDirPath, _dataFileName);
        }
        #endregion

        #region CONSTRUCTORS
        public SaveFileDataHandler(string dataDirPath = "", string dataFileName = "", EncryptionUtilities.EncryptionType encryptionType = EncryptionUtilities.EncryptionType.None, string encryptionString = "")
        {
            _dataDirPath = dataDirPath;
            _dataFileName = dataFileName;
            _encryptionType = encryptionType;
            _encryptionString = encryptionString;
        }
        #endregion

        #region METHODS
        public T Load()
        {
            if (!File.Exists(_fullPath))
            {
                // DEBUG HERE
                return default;
            }
            try
            {
                string dataToLoad;
                using FileStream stream = new FileStream(_fullPath, FileMode.Open);
                using StreamReader reader = new StreamReader(stream);
                dataToLoad = EncryptionUtilities.Encrypt(reader.ReadToEnd(), _encryptionType, false, _encryptionString);
                return JsonConvert.DeserializeObject<T>(dataToLoad);
            }
            catch (Exception e)
            {
                Debug.LogWarning("Error occured when trying to save data to file: " + _fullPath + "\n" + e);
                return default;
            }
        }

        public void Save(T data)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(_fullPath));
                using FileStream stream = new FileStream(_fullPath, FileMode.Create);
                using StreamWriter writer = new StreamWriter(stream);
                writer.Write(EncryptionUtilities.Encrypt(JsonConvert.SerializeObject(data, Formatting.Indented), _encryptionType, true, _encryptionString));
            }
            catch (Exception e)
            {
                Debug.LogError("Error occured when trying to save data to file: " + _fullPath + "\n" + e);
            }
        }

        public static void DestroySavedData(string filePath)
        {
            if (!File.Exists(filePath))
            {
                // DEBUG HERE
            }
            File.Delete(filePath);
        }
        #endregion
    }
}
