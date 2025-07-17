using EncryptionUtils;
using Newtonsoft.Json;
using System;
using System.IO;
using UnityEngine;

namespace MasterProject.SaveSystem
{
    public class SaveFileDataHandler<T> where T : BaseGameData, new()
    {
        #region FIELDS
        private string m_dataFileName;
        private string m_encryptionString;
        private EncryptionUtilities.EncryptionType m_encryptionType;
        #endregion

        #region CONSTS
        private const string FILE_NAME = "data.json";

        private const string ENCRYPTION_STRING = "Yq3t6w9z$C&F)J@NcRfUjWnZr4u7x!A%D*G-KaPdSgVkYp2s5v8y/B?E(H+MbQe" +
            "ThWmZq4t6w9z$C&F)J@NcRfUjXn2r5u8x!A%D*G-KaPdSgVkYp3s6v9y$B?E(H+MbQeThWmZq4t7w!z%C*F)J@NcRfUjXn2r5u8x" +
            "/A?D(G+KaPdSgVkYp3s6v9y$B&E)H@McQeThWmZq4t7w!z%C*F-JaNdRgUjXn2r5u8x/A?D(G+KbPeShVmYp3s6v9y$B&E)H@McQ" +
            "fTjWnZr4t7w!z%C*F-JaNdRgUkXp2s5v8x/A?D(G+KbPeShVmYq3t6w9z$B&E)H@McQfTjWnZr4u7x!A%D*F-JaNdRgUkXp2s5v8" +
            "y/B?E(H+KbPeShVmYq3t6w9z$C&F)J@NcQfTjWnZr4u7x!A%D*G-KaPdSgUkXp2s5v8y/B?E(H+MbQeThWmYq3t6w9z$C&F)J@Nc" +
            "RfUjXn2r4u7x!A%D*G-KaPdSgVkYp3s6v8y/B?E(H+MbQeThW";
        #endregion

        #region PROPERTIES
        string m_fullPath
        {
            get => Path.Combine(Application.persistentDataPath, "SavingDataSystem", m_dataFileName);
        }
        #endregion

        #region CONSTRUCTORS
        public SaveFileDataHandler(string dataFileName = "", EncryptionUtilities.EncryptionType encryptionType = EncryptionUtilities.EncryptionType.None, string encryptionString = "")
        {
            m_dataFileName = string.IsNullOrEmpty(dataFileName) ? FILE_NAME : dataFileName;
            m_encryptionString = string.IsNullOrEmpty(encryptionString) ? ENCRYPTION_STRING : encryptionString;
            m_encryptionType = encryptionType;
        }
        #endregion

        #region METHODS
        public T Load()
        {
            if (!File.Exists(m_fullPath))
            {
                Debug.LogError("No data has been found, please ensure you've saved before loading.");
                return default;
            }
            try
            {
                using FileStream stream = new FileStream(m_fullPath, FileMode.Open);
                using StreamReader reader = new StreamReader(stream);
                string dataToLoad = EncryptionUtilities.Encrypt(reader.ReadToEnd(), m_encryptionType, false, m_encryptionString);
                return JsonConvert.DeserializeObject<T>(dataToLoad);
            }
            catch (Exception e)
            {
                Debug.LogWarning("Error occured when trying to save data to file: " + m_fullPath + "\n" + e);
                return default;
            }
        }

        public void Save(T data)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(m_fullPath));
                using FileStream stream = new FileStream(m_fullPath, FileMode.Create);
                using StreamWriter writer = new StreamWriter(stream);
                writer.Write(EncryptionUtilities.Encrypt(JsonConvert.SerializeObject(data, Formatting.Indented), m_encryptionType, true, m_encryptionString));
            }
            catch (Exception e)
            {
                Debug.LogError("Error occured when trying to save data to file: " + m_fullPath + "\n" + e);
            }
        }

        public void DestroySavedData()
        {
            if (!File.Exists(m_fullPath))
            {
                Debug.LogError($"The file you tried to destroy with path {m_fullPath} didn't exist");
            }
            File.Delete(m_fullPath);
        }
        #endregion
    }
}
