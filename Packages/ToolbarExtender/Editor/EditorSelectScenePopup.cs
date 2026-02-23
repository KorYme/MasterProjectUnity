using System;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine.SceneManagement;

namespace CustomPlayButton
{
    public class EditorSelectScenePopup : PopupWindowContent
    {
        private const float COLLUMN_WIDTH = 300.0f;
        private const float COLLUMN_HEIGHT = 28f;
        private readonly GUILayoutOption[] _iconLayout = new[] {
            GUILayout.Width(20.0f), GUILayout.Height(20.0f)
        };

        private GUIStyle _titleButtonStyle;
        private GUIStyle _buttonStyle;
        private GUIStyle _selectedButtonStyle;
        private GUIContent _bookmarkContent;
        private List<SceneAsset> _buildScenes;
        private SceneAsset _currentScene;

        private Vector2 _scrollPosBuild;
        private Vector2 _scrollPosBookmark;

        public EditorSelectScenePopup()
        {
            InitStyles();

            _bookmarkContent = EditorGUIUtility.IconContent("blendKeySelected", "Bookmark ScriptableObject");

            GetBuildScenes();
            _currentScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(SceneManager.GetActiveScene().path);
            SceneBookmarkSettings.instance.Refresh();
        }

        private void InitStyles()
        {
            Texture2D blankTex = MakeTex(new Color(0f, 0f, 0f, 0f));
            Texture2D selectedTex = MakeTex(new Color(0f, 0f, 0f, 0.3f));

            GUIStyleState hoverState = new GUIStyleState()
            {
                background = selectedTex,
                textColor = GUI.skin.button.onHover.textColor,
            };
            _buttonStyle = new GUIStyle(GUI.skin.label)
            {
                onHover = hoverState,
                hover = hoverState,
            };
            _buttonStyle.normal.background = blankTex;

            _selectedButtonStyle = new GUIStyle(_buttonStyle);
            _selectedButtonStyle.normal.background = selectedTex;

            _titleButtonStyle = new GUIStyle(EditorStyles.boldLabel);
            _titleButtonStyle.onHover = _buttonStyle.onHover;
            _titleButtonStyle.hover = _buttonStyle.hover;
            _titleButtonStyle.normal.background = blankTex;
        }

        public static Texture2D MakeTex(Color col)
        {
            Texture2D texture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
            texture.SetPixel(0, 0, col);
            texture.Apply();
            return texture;
        }

        public override Vector2 GetWindowSize()
        {
            float width = COLLUMN_WIDTH * (SceneBookmarkSettings.instance.ScenesCount > 0 ? 2 : 1);
            int maxRow = Mathf.Max(_buildScenes.Count, SceneBookmarkSettings.instance.ScenesCount, 1);
            float height = Mathf.Min(COLLUMN_HEIGHT * (maxRow + 1), Screen.currentResolution.height * 0.5f);
            return new Vector2(width, height);
        }

        public override void OnGUI(Rect rect)
        {
            EditorGUILayout.BeginHorizontal();
            DrawBuildScenes();
            DrawBookmarkScenes();
            EditorGUILayout.EndHorizontal();

            if (Event.current.type == EventType.MouseMove && EditorWindow.mouseOverWindow == editorWindow)
                editorWindow?.Repaint();
        }

        private void DrawBuildScenes()
        {
            EditorGUILayout.BeginVertical();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Scenes in Project", EditorStyles.boldLabel, GUILayout.Height(20.0f));
            EditorGUILayout.EndHorizontal();

            if (_buildScenes.Count > 0)
            {
                _scrollPosBuild = EditorGUILayout.BeginScrollView(_scrollPosBuild);
                for (int i = 0; i < _buildScenes.Count; i++)
                {
                    DrawSelection(_buildScenes[i], true);
                }
                SceneBookmarkSettings.instance.Refresh();
                EditorGUILayout.EndScrollView();
            }
            else
            {
                GUILayout.Label("No scene in build setting");
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawBookmarkScenes()
        {
            if (SceneBookmarkSettings.instance.ScenesCount <= 0) return;

            EditorGUILayout.BeginVertical(GUILayout.MinWidth(COLLUMN_WIDTH));

            GUIContent content = new GUIContent(_bookmarkContent);
            content.text = "Bookmarks";
            GUILayout.Label(content, EditorStyles.boldLabel, GUILayout.Height(20.0f));

            _scrollPosBookmark = EditorGUILayout.BeginScrollView(_scrollPosBookmark);
            foreach (SceneAsset bookmark in SceneBookmarkSettings.instance.Scenes)
            {
                DrawSelection(bookmark);
            }
            SceneBookmarkSettings.instance.Refresh();
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        private void DrawSelection(SceneAsset scene, bool bookmarkButton = false)
        {
            if (scene == null) return;

            GUILayout.BeginHorizontal();

            if (bookmarkButton)
            {
                bool isInBookmark = SceneBookmarkSettings.instance.Contains(scene);
                GUIContent content;
                if (isInBookmark)
                {
                    content = EditorGUIUtility.IconContent("blendKeySelected", "Unbookmark");
                }
                else
                {
                    content = EditorGUIUtility.IconContent("blendKeyOverlay", "Bookmark");
                }
                if (GUILayout.Button(content, _buttonStyle, _iconLayout))
                {
                    if (isInBookmark)
                    {
                        SceneBookmarkSettings.instance.RemoveScene(scene);
                    }
                    else
                    {
                        SceneBookmarkSettings.instance.AddScene(scene);
                    }
                }
            }
            else
            {
                if (GUILayout.Button(EditorGUIUtility.IconContent("CrossIcon", "Unbookmark"), _buttonStyle, _iconLayout))
                {
                    SceneBookmarkSettings.instance.RemoveScene(scene);
                }
            }

            GUIStyle style = _currentScene == scene ? _selectedButtonStyle : _buttonStyle;
            if (GUILayout.Button(EditorGUIUtility.IconContent("d_Folder Icon", "Ping Scene"), _buttonStyle, _iconLayout))
            {
                EditorGUIUtility.PingObject(scene);
            }
            if (GUILayout.Button(EditorGUIUtility.IconContent("d_BuildSettings.SelectedIcon", "Open Scene"), style, _iconLayout))
            {
                OpenScene(scene);
            }
            
            style = CustomPlayButton.SelectedScene == scene ? _selectedButtonStyle : _buttonStyle;
            if (GUILayout.Button(scene.name, style))
            {
                SelectScene(scene);
            }
            GUILayout.EndHorizontal();
        }

        private void SelectScene(SceneAsset scene)
        {
            CustomPlayButton.SelectedScene = scene;
            editorWindow.Close();
        }

        private void OpenScene(SceneAsset scene)
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                string scenePath = AssetDatabase.GetAssetPath(scene);
                EditorSceneManager.OpenScene(scenePath);
                _currentScene = scene;
                // Recreate textures which are destoryed by OpenScene
                InitStyles();
            }
        }

        private void GetBuildScenes()
        {
            _buildScenes = LoadAssetsOfType<SceneAsset>(Path.Combine("Assets", ToolbarExtenderSettings.instance?.FolderToFocus ?? string.Empty)).ToList();
            _buildScenes.Sort((x, y) => String.Compare(x.name, y.name, StringComparison.CurrentCulture));
            if (_buildScenes.Count == 0)
            {
                _buildScenes = LoadAssetsOfType<SceneAsset>("Assets/").ToList();
                Debug.LogWarning("Using \"Assets/\" path instead.");
            }
        }
        
        private static T[] LoadAssetsOfType<T>(params string[] searchInFolders) where T : UnityEngine.Object
        {
            return AssetDatabase
                .FindAssets($"t:{typeof(T).Name}", searchInFolders)
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<T>)
                .ToArray();
        }
    }
}
