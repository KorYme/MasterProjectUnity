using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine.SceneManagement;

namespace ASze.CustomPlayButton
{
    public class EditorSelectScenePopup : PopupWindowContent
    {
        const float COLLUMN_WIDTH = 200.0f;
        const float COLLUMN_HEIGHT = 28f;
        readonly GUILayoutOption[] ICON_LAYOUT = new GUILayoutOption[] {
            GUILayout.Width(20.0f), GUILayout.Height(20.0f)
        };


        GUIStyle titleButtonStyle;
        GUIStyle buttonStyle;
        GUIStyle selectedButtonStyle;
        GUIContent bookmarkContent;
        SceneAsset[] buildScenes;
        SceneAsset currentScene;

        Vector2 scrollPosBuild;
        Vector2 scrollPosBookmark;

        public EditorSelectScenePopup()
        {
            InitStyles();

            bookmarkContent = EditorGUIUtility.IconContent("blendKeySelected", "Bookmark ScriptableObject");

            GetBuildScenes();
            currentScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(SceneManager.GetActiveScene().path);
            CustomPlayButton.Bookmark.RemoveNullValue();
        }

        void InitStyles()
        {
            var blankTex = MakeTex(new Color(0f, 0f, 0f, 0f));
            var selectedTex = MakeTex(new Color(0f, 0f, 0f, 0.3f));

            var hoverState = new GUIStyleState()
            {
                background = selectedTex,
                textColor = GUI.skin.button.onHover.textColor,
            };
            buttonStyle = new GUIStyle(GUI.skin.label)
            {
                onHover = hoverState,
                hover = hoverState,
            };
            buttonStyle.normal.background = blankTex;

            selectedButtonStyle = new GUIStyle(buttonStyle);
            selectedButtonStyle.normal.background = selectedTex;

            titleButtonStyle = new GUIStyle(EditorStyles.boldLabel);
            titleButtonStyle.onHover = buttonStyle.onHover;
            titleButtonStyle.hover = buttonStyle.hover;
            titleButtonStyle.normal.background = blankTex;
        }

        public static Texture2D MakeTex(Color col)
        {
            var texture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
            texture.SetPixel(0, 0, col);
            texture.Apply();
            return texture;
        }

        public override Vector2 GetWindowSize()
        {
            var width = COLLUMN_WIDTH * (CustomPlayButton.Bookmark.HasBookmark() ? 2 : 1);
            var maxRow = Mathf.Max(buildScenes.Length, CustomPlayButton.Bookmark.bookmarks.Count, 1);
            var height = Mathf.Min(COLLUMN_HEIGHT * (maxRow + 1), Screen.currentResolution.height * 0.5f);
            return new Vector2(width, height);
        }

        public override void OnClose()
        {
            if (EditorUtility.IsDirty(CustomPlayButton.Bookmark))
            {
                AssetDatabase.SaveAssets();
            }
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

        void DrawBuildScenes()
        {
            EditorGUILayout.BeginVertical();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Scenes in Project", EditorStyles.boldLabel, GUILayout.Height(20.0f));
            if (!CustomPlayButton.Bookmark.HasBookmark())
            {
                GUILayout.FlexibleSpace();
                if (GUILayout.Button(bookmarkContent, buttonStyle, ICON_LAYOUT))
                {
                    Selection.activeObject = CustomPlayButton.Bookmark;
                }
            }
            EditorGUILayout.EndHorizontal();

            if (buildScenes.Length > 0)
            {
                scrollPosBuild = EditorGUILayout.BeginScrollView(scrollPosBuild);
                for (int i = 0; i < buildScenes.Length; i++)
                {
                    DrawSelection(buildScenes[i], i, true);
                }
                EditorGUILayout.EndScrollView();
            }
            else
            {
                GUILayout.Label("No scene in build setting");
            }
            EditorGUILayout.EndVertical();
        }

        void DrawBookmarkScenes()
        {
            var bookmarkSetting = CustomPlayButton.Bookmark;
            if (!bookmarkSetting.HasBookmark()) return;

            EditorGUILayout.BeginVertical(GUILayout.MinWidth(COLLUMN_WIDTH));

            var content = new GUIContent(bookmarkContent);
            content.text = "Bookmark";
            if (GUILayout.Button(content, titleButtonStyle, GUILayout.Height(20.0f)))
            {
                Selection.activeObject = CustomPlayButton.Bookmark;
            }


            scrollPosBookmark = EditorGUILayout.BeginScrollView(scrollPosBookmark);
            var bookmarks = new List<SceneAsset>(bookmarkSetting.bookmarks);
            foreach (var bookmark in bookmarks)
            {
                DrawSelection(bookmark);
            }
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        void DrawSelection(SceneAsset scene, int index = -1, bool bookmarkButton = false)
        {
            if (scene == null) return;

            GUILayout.BeginHorizontal();

            if (bookmarkButton)
            {
                var bookmarks = CustomPlayButton.Bookmark.bookmarks;
                var inBookmark = bookmarks.Contains(scene);
                GUIContent content;
                if (inBookmark)
                    content = EditorGUIUtility.IconContent("blendKeySelected", "Unbookmark");
                else
                    content = EditorGUIUtility.IconContent("blendKeyOverlay", "Bookmark");
                if (GUILayout.Button(content, buttonStyle, ICON_LAYOUT))
                {
                    if (inBookmark)
                    {
                        bookmarks.Remove(scene);
                    }
                    else
                    {
                        bookmarks.Add(scene);
                    }
                    bookmarks.RemoveAll(item => item == null);
                    EditorUtility.SetDirty(CustomPlayButton.Bookmark);
                }
            }
            else
            {
                if (GUILayout.Button(EditorGUIUtility.IconContent("d_winbtn_win_close", "Unbookmark"), buttonStyle, ICON_LAYOUT))
                {
                    var bookmarks = CustomPlayButton.Bookmark.bookmarks;
                    bookmarks.Remove(scene);
                    bookmarks.RemoveAll(item => item == null);
                    EditorUtility.SetDirty(CustomPlayButton.Bookmark);
                }
            }

            GUIStyle style = currentScene == scene ? selectedButtonStyle : buttonStyle;
            if (GUILayout.Button(EditorGUIUtility.IconContent("d_BuildSettings.SelectedIcon", "Open Scene"), style, ICON_LAYOUT))
            {
                OpenScene(scene);
            }

            style = CustomPlayButton.SelectedScene == scene ? selectedButtonStyle : buttonStyle;
            if (GUILayout.Button(index >= 0 ? $"{index}   {scene.name}" : scene.name, style))
            {
                SelectScene(scene);
            }
            GUILayout.EndHorizontal();
        }

        void SelectScene(SceneAsset scene)
        {
            CustomPlayButton.SelectedScene = scene;
            editorWindow.Close();
        }

        void OpenScene(SceneAsset scene)
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                var scenePath = AssetDatabase.GetAssetPath(scene);
                EditorSceneManager.OpenScene(scenePath);
                currentScene = scene;
                // Recreate textures which are destoryed by OpenScene
                InitStyles();
            }
        }

        void GetBuildScenes()
        {
            ToolbarExtenderSettings asset = AssetDatabase.LoadAssetAtPath<ToolbarExtenderSettings>(ToolbarExtenderSettings.toolbarExtenderSettingsPath);
            buildScenes = LoadAssetsOfType<SceneAsset>(Path.Combine("Assets", asset?.FolderToFocus ?? string.Empty));
            if (buildScenes.Length == 0)
            {
                buildScenes = LoadAssetsOfType<SceneAsset>("Assets/");
                Debug.LogWarning("Using \"Assets/\" path instead.");
            }
        }
        
        public static T[] LoadAssetsOfType<T>(params string[] searchInFolders) where T : Object
        {
            return AssetDatabase
                .FindAssets($"t:{typeof(T).Name}", searchInFolders)
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<T>)
                .ToArray();
        }
    }
}
