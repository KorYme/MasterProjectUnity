using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Linq;

namespace MasterProject.SaveSystem
{
    public class DataSaveSystemGenerator : MonoBehaviour
    {
        #region FIELDS
        [Header("Parameters")]
        [SerializeField, Tooltip("Name of the class which will countain all the data")] 
        private string m_dataClassName;
        [SerializeField, Tooltip("Path to the folder starting from the Assets/")]
        private string m_FolderName;
        #endregion

        #region PROPERTIES
        private string FolderPath
        {
            get => System.IO.Path.Combine(Application.dataPath, m_FolderName);
        }

        private string SystemClassName
        {
            get => "DSM_" + m_dataClassName;
        }

        private string Path
        {
            get => System.IO.Path.Combine(FolderPath, SystemClassName + ".cs");
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
                "   public class " + SystemClassName + " : DataSaveManager<" + m_dataClassName + ">" + "\n" +
                "   {" + "\n" +
                "       // Modify if you're willing to add some behaviour to the component" + "\n" +
                "   }" + "\n" +
                "\n" +
                "   [System.Serializable]" + "\n" +
                "   public class " + m_dataClassName + " : " + "GameDataTemplate" + "\n" +
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
            m_dataClassName = "GameData";
        }

        public void GenerateSaveSystemFolder()
        {
            if (Directory.Exists(FolderPath))
            {
                Debug.LogWarning("A folder named this way already exists in the project.");
                return;
            }
            Directory.CreateDirectory(FolderPath);
            AssetDatabase.Refresh();
        }

        public void GenerateGameDataClass()
        {
            if (!Directory.Exists(FolderPath))
            {
                Debug.LogWarning("No folder named this way has been found in the project. \n" +
                                    "Try creating one with the button above");
                return;
            }
            if (File.Exists(FolderPath + "/" + SystemClassName + ".cs"))
            {
                Debug.LogWarning("There is already one class named this way in " + m_FolderName);
                return;
            }
            // �criture du code g�n�r� dans un fichier
            File.WriteAllText(Path, m_ClassCode);
            AssetDatabase.Refresh();
        }

        public void AttachDataSaveManager()
        {
            if (!Directory.Exists(FolderPath))
            {
                Debug.LogWarning("No folder SaveSystemClasses has been found");
                return;
            }
            if (!File.Exists(FolderPath + "/" + SystemClassName + ".cs"))
            {
                Debug.LogWarning("No game data class has been found in the folder : " + FolderPath);
                return;
            }
            Type type = AppDomain.CurrentDomain.GetAssemblies()
                                .SelectMany(a => a.GetTypes())
                                .FirstOrDefault(t => t.Name == SystemClassName);
            if (type == null)
            {
                Debug.LogWarning("No type has been found");
                return;
            }
            if (GetComponent(type) != null)
            {
                Debug.LogWarning("A component already exists on this gameObject");
                return;
            }
            gameObject.AddComponent(type);
        }
        #endif
        #endregion
    }
}