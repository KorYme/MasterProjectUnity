using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Reflection;
using UnityToolbarExtender;

using VisualElement = UnityEngine.UIElements.VisualElement;

namespace CustomPlayButton
{
    [InitializeOnLoad]
    public static class CustomPlayButton
    {
        private const string FOLDER_PATH = "Assets/ToolbarExtender/Editor/";
        private const string SETTING_PATH = FOLDER_PATH + "BookmarkSetting.asset";
        private const string ICONS_PATH = "Packages/com.koryme.unity-toolbar-extender/Icons/";

        private static SceneAsset _selectedScene;

        private static GUIContent _customSceneContent;
        private static GUIContent _gameSceneContent;

        private static Rect _buttonRect;
        private static VisualElement _toolbarElement;
        private static SceneAsset _lastScene;

        public static SceneAsset SelectedScene
        {
            get => _selectedScene; 
            set
            {
                _selectedScene = value;
                _toolbarElement?.MarkDirtyRepaint();

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
            public static readonly GUIStyle CommandButtonStyle;

            static ToolbarStyles()
            {
                EditorApplication.playModeStateChanged += HandleOnPlayModeChanged;
                CommandButtonStyle = new GUIStyle("Command")
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
            ToolbarExtender.LeftToolbarGUI.Remove(OnToolbarLeftGUI);
            ToolbarExtender.LeftToolbarGUI.Add(OnToolbarLeftGUI);
            EditorApplication.update -= OnUpdate;
            EditorApplication.update += OnUpdate;
            
            SceneBookmarkSettings.instance.Refresh();
            string savedScenePath = EditorPrefs.GetString(GetEditorPrefKey(), "");
            _selectedScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(savedScenePath);
            if (_selectedScene == null && EditorBuildSettings.scenes.Length > 0)
            {
                string scenePath = EditorBuildSettings.scenes[0].path;
                SelectedScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);
            }

            _customSceneContent = CreateIconContent("PlaySceneButton.png", "d_UnityEditor.Timeline.TimelineWindow@2x", "Play Custom Scene");
            _gameSceneContent = CreateIconContent("PlayGameButton.png", "d_UnityEditor.GameView@2x", "Play Game Scene");
        }

        static void OnToolbarLeftGUI()
        {
            GUILayout.FlexibleSpace();

            string sceneName = _selectedScene != null ? _selectedScene.name : "Select Scene...";
            bool selected = EditorGUILayout.DropdownButton(new GUIContent(sceneName, "Setup selected scene and bookmarks"), FocusType.Passive);
            if (Event.current.type == EventType.Repaint)
            {
                _buttonRect = GUILayoutUtility.GetLastRect();
            }

            if (selected)
            {
                PopupWindow.Show(_buttonRect, new EditorSelectScenePopup());
            }

            if (GUILayout.Button(_customSceneContent, ToolbarStyles.CommandButtonStyle))
            {
                if (_selectedScene != null)
                {
                    StartScene(_selectedScene);
                }
                else
                {
                    EditorUtility.DisplayDialog(
                        "Cannot play custom scene",
                        "No scene is selected to play. Please select a scene from the dropdown list.",
                        "Ok");
                }
            }

            if (GUILayout.Button(_gameSceneContent, ToolbarStyles.CommandButtonStyle))
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
                _lastScene = scene;
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
            if (_toolbarElement == null)
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
                    _toolbarElement = (VisualElement)viewVisualTree.GetValue(windowBackend, null);
#else
                    PropertyInfo viewVisualTree = guiViewType.GetProperty("visualTree",
                        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    _toolbarElement = (VisualElement)viewVisualTree.GetValue(currentToolbar, null);
#endif
                }
            }

            if (_lastScene == null ||
                EditorApplication.isPlaying || EditorApplication.isPaused ||
                EditorApplication.isCompiling || EditorApplication.isPlayingOrWillChangePlaymode)
            {
                return;
            }

            ChangeScene(_lastScene);
            _lastScene = null;
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
