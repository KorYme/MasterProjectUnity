using System;
using System.IO;
using SerializationUtils;
using UnityEditor;
using UnityEngine;

namespace GraphTool.Editor
{
    public class GraphSaveHandler
    {
        #region CONSTANTS
        readonly static string GRAPH_MAIN_FOLDER_PATH = Path.Combine("Assets", "GraphToolSaved");
        readonly static string GRAPH_WINDOW_SETTINGS_FOLDER_PATH = Path.Combine(GRAPH_MAIN_FOLDER_PATH, $"_{nameof(GraphWindowSettings)}");
        readonly static string GRAPH_WINDOW_SETTINGS_DATA_PATH = Path.Combine(GRAPH_WINDOW_SETTINGS_FOLDER_PATH, $"{nameof(GraphWindowSettings)}.asset");
        #endregion

        #region ASSET_CREATION_METHODS
        public void GenerateDSRootFolder()
        {
            if (!Directory.Exists(GRAPH_MAIN_FOLDER_PATH))
            {
                Directory.CreateDirectory(Path.Combine(GRAPH_MAIN_FOLDER_PATH));
            }
            if (!Directory.Exists(GRAPH_WINDOW_SETTINGS_FOLDER_PATH))
            {
                Directory.CreateDirectory(Path.Combine(GRAPH_WINDOW_SETTINGS_FOLDER_PATH));
                AssetDatabase.Refresh();
            }
        }

        public GraphWindowSettings GetOrGenerateNewWindowData()
        {
            GenerateDSRootFolder();
            if (!File.Exists(GRAPH_WINDOW_SETTINGS_DATA_PATH))
            {
                GraphWindowSettings graphWindowSettings = ScriptableObject.CreateInstance<GraphWindowSettings>();
                AssetDatabase.CreateAsset(graphWindowSettings, GRAPH_WINDOW_SETTINGS_DATA_PATH);
                return graphWindowSettings;
            }
            else
            {
                return AssetDatabase.LoadAssetAtPath<GraphWindowSettings>(GRAPH_WINDOW_SETTINGS_DATA_PATH);
            }
        }

        public GraphData GenerateGraphDataFile(string fileName)
        {
            if (fileName.IsSerializableFriendly())
            {
                Debug.LogWarning("Please choose a valid name before generating a new graph file");
                return null;
            }
            GenerateDSRootFolder();
            if (!File.Exists(Path.Combine(GRAPH_MAIN_FOLDER_PATH, fileName) + ".asset"))
            {
                GraphData graphData = ScriptableObject.CreateInstance<GraphData>();
                AssetDatabase.CreateAsset(graphData, Path.Combine(GRAPH_MAIN_FOLDER_PATH, fileName) + ".asset");
                AssetDatabase.SaveAssets();
                if (!Directory.Exists(Path.Combine(GRAPH_MAIN_FOLDER_PATH, fileName)))
                {
                    Directory.CreateDirectory(Path.Combine(GRAPH_MAIN_FOLDER_PATH, fileName));
                    AssetDatabase.Refresh();
                }
                return graphData;
            }
            Debug.LogWarning("A file named the same way already exist, please rename it before generating a new graph");
            return null;
        }
        #endregion

        #region SAVE_AND_LOAD_UTILITIES
        public bool SaveDataInProject<T>(T elementData, string graphName) where T : ElementData
        {
            Type tmpType = elementData.GetType();
            string path = "";
            while (tmpType != typeof(ElementData))
            {
                path = Path.Combine(tmpType.Name, path);
                tmpType = tmpType.BaseType;
            }
            path = Path.Combine(GRAPH_MAIN_FOLDER_PATH, graphName, path);
            if (!Directory.Exists(path)) 
            {
                Directory.CreateDirectory(Path.Combine(path));
                AssetDatabase.Refresh();
            }
            path = Path.Combine(path, elementData.ID) + ".asset";
            if (!File.Exists(path))
            {
                AssetDatabase.CreateAsset(elementData, path);
                AssetDatabase.SaveAssets();
                return true;
            }
            return false;
        }

        public void RemoveDataFromProject<T>(T elementData) where T : ElementData
        {
            Type tmpType = typeof(T);
            string path = "";
            while (tmpType != typeof(ElementData))
            {
                path = Path.Combine(tmpType.Name, path);
                tmpType = tmpType.BaseType;
            }
            path = Path.Combine(GRAPH_MAIN_FOLDER_PATH, path);
            if (!Directory.Exists(path))
            {
                Debug.Log("The file in which the data should have been saved has been destroyed");
            }
        }
        #endregion
    }
}
