using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Linq;
using MasterProject.Debugging;

namespace MasterProject.SaveSystem
{
    public class DataSaveSystemGenerator : MonoBehaviour
    {
        #region FIELDS
        [Header("Parameters")]
        [SerializeField, Tooltip("Name of the class which will countain all the data")] 
        private string m_DataClassName;
        [SerializeField, Tooltip("Path to the folder starting from the Assets/")]
        private string m_FolderName;
        #endregion

        #region PROPERTIES
        private string m_FolderPath
        {
            get => Path.Combine(Application.dataPath, m_FolderName);
        }

        private string m_SystemClassName
        {
            get => "DSM_" + m_DataClassName;
        }

        private string m_Path
        {
            get => Path.Combine(m_FolderPath, m_SystemClassName + ".cs");
        }

        private string m_ClassCode
        {
            get =>
                "using System.Collections;" + "\n" +
                "using System.Collections.Generic;" + "\n" +
                "using UnityEngine;" + "\n" +
                "\n" +
                "namespace MasterProject.SaveSystem " + "\n" +
                "{" + "\n" +
                "   public class " + m_SystemClassName + " : DataSaveManager<" + m_DataClassName + ">" + "\n" +
                "   {" + "\n" +
                "       // Modify if you're willing to add some behaviour to the component" + "\n" +
                "   }" + "\n" +
                "\n" +
                "   [System.Serializable]" + "\n" +
                "   public class " + m_DataClassName + " : " + "GameDataTemplate" + "\n" +
                "   {" + "\n" +
                "       // Create the values you want to save here" + "\n" +
                "   }" + "\n" +
                "}";
        }
        #endregion

        #region METHODS
        #if UNITY_EDITOR
        private void Reset()
        {
            m_FolderName = "SaveSystemClasses";
            m_DataClassName = "GameData";
        }

        //[Button]
        public void GenerateSaveSystemFolder()
        {
            if (Directory.Exists(m_FolderPath))
            {
                DebugLogger.Warning(this, "A folder named this way already exists in the project.");
                return;
            }
            Directory.CreateDirectory(m_FolderPath);
            AssetDatabase.Refresh();
        }

        //[Button]
        public void GenerateGameDataClass()
        {
            if (!Directory.Exists(m_FolderPath))
            {
                DebugLogger.Warning(this, "No folder named this way has been found in the project. \n" +
                                    "Try creating one with the button above");
                return;
            }
            if (File.Exists(m_FolderPath + "/" + m_SystemClassName + ".cs"))
            {
                DebugLogger.Warning(this, "There is already one class named this way in " + m_FolderName);
                return;
            }
            // Écriture du code généré dans un fichier
            File.WriteAllText(m_Path, m_ClassCode);
            AssetDatabase.Refresh();
        }

        //[Button]
        public void AttachDataSaveManager()
        {
            if (!Directory.Exists(m_FolderPath))
            {
                DebugLogger.Warning(this, "No folder SaveSystemClasses has been found");
                return;
            }
            if (!File.Exists(m_FolderPath + "/" + m_SystemClassName + ".cs"))
            {
                DebugLogger.Warning(this, "No game data class has been found in the folder : " + m_FolderPath);
                return;
            }
            Type type = AppDomain.CurrentDomain.GetAssemblies()
                                .SelectMany(a => a.GetTypes())
                                .FirstOrDefault(t => t.Name == m_SystemClassName);
            if (type == null)
            {
                DebugLogger.Warning(this, "No type has been found");
                return;
            }
            if (GetComponent(type) != null)
            {
                DebugLogger.Warning(this, "A component already exists on this gameObject");
                return;
            }
            gameObject.AddComponent(type);
        }
        #endif
        #endregion
    }
}