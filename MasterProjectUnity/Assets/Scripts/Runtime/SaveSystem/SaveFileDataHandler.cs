using MasterProject.Debugging;
using MasterProject.Utilities;
using Newtonsoft.Json;
using System;
using System.IO;

namespace MasterProject.SaveSystem
{
    public class SaveFileDataHandler<T>
    {
        #region FIELDS
        private string m_DataDirPath;
        private string m_DataFileName;
        private EncryptionUtilities.EncryptionType m_EncryptionType;
        private string m_EncryptionString;
        #endregion

        #region PROPERTIES
        string m_FullPath
        {
            get => Path.Combine(m_DataDirPath, "SavingDataSystem", m_DataFileName);
        }
        #endregion

        #region CONSTRUCTORS
        public SaveFileDataHandler(string dataDirPath = "", string dataFileName = "", EncryptionUtilities.EncryptionType encryptionType = EncryptionUtilities.EncryptionType.None, string encryptionString = "")
        {
            m_DataDirPath = dataDirPath;
            m_DataFileName = dataFileName;
            m_EncryptionType = encryptionType;
            m_EncryptionString = encryptionString;
        }
        #endregion

        #region METHODS
        public T Load()
        {
            if (!File.Exists(m_FullPath))
            {
                DebugLogger.Error(this, "No data has been found, please ensure you've saved before loading.");
                return default;
            }
            try
            {
                string dataToLoad;
                using FileStream stream = new FileStream(m_FullPath, FileMode.Open);
                using StreamReader reader = new StreamReader(stream);
                dataToLoad = EncryptionUtilities.Encrypt(reader.ReadToEnd(), m_EncryptionType, false, m_EncryptionString);
                return JsonConvert.DeserializeObject<T>(dataToLoad);
            }
            catch (Exception e)
            {
                DebugLogger.Warning(this, "Error occured when trying to save data to file: " + m_FullPath + "\n" + e);
                return default;
            }
        }

        public void Save(T data)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(m_FullPath));
                using FileStream stream = new FileStream(m_FullPath, FileMode.Create);
                using StreamWriter writer = new StreamWriter(stream);
                writer.Write(EncryptionUtilities.Encrypt(JsonConvert.SerializeObject(data, Formatting.Indented), m_EncryptionType, true, m_EncryptionString));
            }
            catch (Exception e)
            {
                DebugLogger.Error(this, "Error occured when trying to save data to file: " + m_FullPath + "\n" + e);
            }
        }

        public void DestroySavedData()
        {
            if (!File.Exists(m_FullPath))
            {
                DebugLogger.Error(this, $"The file you tried to destroy with path {m_FullPath} didn't exist");
            }
            File.Delete(m_FullPath);
        }
        #endregion
    }
}
