using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;
using System.Reflection;
using UnityToolbarExtender;

using VisualElement = UnityEngine.UIElements.VisualElement;

namespace ASze.CustomPlayButton
{
    [InitializeOnLoad]
    public static class CustomPlayButton
    {
        const string FOLDER_PATH = "Assets/Scripts/Editor/ToolbarExtender/CustomPlayButton/";
        const string SETTING_PATH = FOLDER_PATH + "BookmarkSettings/BookmarkSetting.asset";
        const string ICONS_PATH = FOLDER_PATH + "Icons/";

        private static SceneBookmark bookmark = null;
        private static SceneAsset selectedScene = null;


        static GUIContent customSceneContent;
        static GUIContent gameSceneContent;

        static Rect buttonRect;
        static VisualElement toolbarElement;
        static SceneAsset lastScene = null;

        public static SceneBookmark Bookmark
        {
            get
            {
                if (bookmark == null)
                {
                    bookmark = ScriptableObject.CreateInstance<SceneBookmark>();
                    if (!Directory.Exists(FOLDER_PATH))
                        Directory.CreateDirectory(FOLDER_PATH);
                    AssetDatabase.CreateAsset(bookmark, SETTING_PATH);
                    AssetDatabase.Refresh();
                }
                return bookmark;
            }
        }

        public static SceneAsset SelectedScene
        {
            get { return selectedScene; }
            set
            {
                selectedScene = value;
                toolbarElement?.MarkDirtyRepaint();

                if (value != null)
                {
                    var path = AssetDatabase.GetAssetPath(value);
                    EditorPrefs.SetString(GetEditorPrefKey(), path);
                }
                else
                {
                    EditorPrefs.DeleteKey(GetEditorPrefKey());
                }
            }
        }

        static class ToolbarStyles
        {
            public static readonly GUIStyle commandButtonStyle;

            static ToolbarStyles()
            {
                EditorApplication.playModeStateChanged += HandleOnPlayModeChanged;
                commandButtonStyle = new GUIStyle("Command")
                {
                    fontSize = 16,
                    alignment = TextAnchor.MiddleCenter,
                    imagePosition = ImagePosition.ImageAbove,
                    fontStyle = FontStyle.Bold
                };
            }
        }

        static CustomPlayButton()
        {
            ToolbarExtender.LeftToolbarGUI.Add(OnToolbarLeftGUI);
            EditorApplication.update += OnUpdate;

            if (bookmark == null)
            {
                bookmark = AssetDatabase.LoadAssetAtPath<SceneBookmark>(SETTING_PATH);
                Bookmark?.RemoveNullValue();
            }

            string savedScenePath = EditorPrefs.GetString(GetEditorPrefKey(), "");
            selectedScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(savedScenePath);
            if (selectedScene == null && EditorBuildSettings.scenes.Length > 0)
            {
                string scenePath = EditorBuildSettings.scenes[0].path;
                SelectedScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);
            }

            customSceneContent = CreateIconContent("PlaySceneButton.png", "d_UnityEditor.Timeline.TimelineWindow@2x", "Play Custom Scene");
            gameSceneContent = CreateIconContent("PlayGameButton.png", "d_UnityEditor.GameView@2x", "Play Game Scene");
        }

        static void OnToolbarLeftGUI()
        {
            GUILayout.FlexibleSpace();

            string sceneName = selectedScene != null ? selectedScene.name : "Select Scene...";
            bool selected = EditorGUILayout.DropdownButton(new GUIContent(sceneName, "Setup selected scene and bookmarks"), FocusType.Passive);
            if (Event.current.type == EventType.Repaint)
            {
                buttonRect = GUILayoutUtility.GetLastRect();
            }

            if (selected)
            {
                PopupWindow.Show(buttonRect, new EditorSelectScenePopup());
            }

            if (GUILayout.Button(customSceneContent, ToolbarStyles.commandButtonStyle))
            {
                if (selectedScene != null)
                {
                    StartScene(selectedScene);
                }
                else
                {
                    EditorUtility.DisplayDialog(
                        "Cannot play custom scene",
                        "No scene is selected to play. Please select a scene from the dropdown list.",
                        "Ok");
                }
            }

            if (GUILayout.Button(gameSceneContent, ToolbarStyles.commandButtonStyle))
            {
                if (EditorBuildSettings.scenes.Length > 0)
                {
                    string scenePath = EditorBuildSettings.scenes[0].path;
                    SceneAsset scene = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);
                    StartScene(scene);
                }
                else
                {
                    if (!EditorUtility.DisplayDialog(
                        "Cannot play the game",
                        "Please add the first scene in build setting in order to play the game.",
                        "Ok", "Open build setting"))
                    {
                        EditorWindow.GetWindow(System.Type.GetType("UnityEditor.BuildPlayerWindow,UnityEditor"));
                    }
                    // Avoid error from GUILayout.EndHorizontal()
                    GUILayout.BeginHorizontal();
                }
            }
        }

        static void StartScene(SceneAsset scene)
        {
            if (EditorApplication.isPlaying)
            {
                lastScene = scene;
                EditorApplication.isPlaying = false;
            }
            else
            {
                ChangeScene(scene);
            }
        }

        static void OnUpdate()
        {
            // Get toolbar element for repainting
            if (toolbarElement == null)
            {
                System.Type toolbarType = typeof(Editor).Assembly.GetType("UnityEditor.Toolbar");
                Object[] toolbars = Resources.FindObjectsOfTypeAll(toolbarType);
                ScriptableObject currentToolbar = toolbars.Length > 0 ? (ScriptableObject)toolbars[0] : null;
                if (currentToolbar != null)
                {
                    System.Type guiViewType = typeof(Editor).Assembly.GetType("UnityEditor.GUIView");
#if UNITY_2020_1_OR_NEWER
                    System.Type iWindowBackendType = typeof(Editor).Assembly.GetType("UnityEditor.IWindowBackend");
                    PropertyInfo guiBackend = guiViewType.GetProperty("windowBackend",
                        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    PropertyInfo viewVisualTree = iWindowBackendType.GetProperty("visualTree",
                        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    object windowBackend = guiBackend.GetValue(currentToolbar);
                    toolbarElement = (VisualElement)viewVisualTree.GetValue(windowBackend, null);
#else
                    PropertyInfo viewVisualTree = guiViewType.GetProperty("visualTree",
                        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    toolbarElement = (VisualElement)viewVisualTree.GetValue(currentToolbar, null);
#endif
                }
            }

            if (lastScene == null ||
                EditorApplication.isPlaying || EditorApplication.isPaused ||
                EditorApplication.isCompiling || EditorApplication.isPlayingOrWillChangePlaymode)
            {
                return;
            }

            ChangeScene(lastScene);
            lastScene = null;
        }

        static void ChangeScene(SceneAsset scene)
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                EditorSceneManager.playModeStartScene = scene;
                EditorApplication.isPlaying = true;
            }
        }

        static void HandleOnPlayModeChanged(PlayModeStateChange playMode)
        {
            if (playMode == PlayModeStateChange.ExitingPlayMode)
            {
                EditorSceneManager.playModeStartScene = null;
            }
        }

        public static string GetEditorPrefKey()
        {
            string projectPrefix = PlayerSettings.companyName + "." + PlayerSettings.productName;
            return projectPrefix + "_CustomPlayButton_SelectedScenePath";
        }

        public static GUIContent CreateIconContent(string localTex, string builtInTex, string tooltip)
        {
            Texture2D tex = LoadTexture(localTex);
            if (tex != null) return new GUIContent(tex, tooltip);
            else return EditorGUIUtility.IconContent(builtInTex, tooltip);
        }

        public static Texture2D LoadTexture(string path)
        {
            return (Texture2D)EditorGUIUtility.Load(ICONS_PATH + path);
        }
    }
}
