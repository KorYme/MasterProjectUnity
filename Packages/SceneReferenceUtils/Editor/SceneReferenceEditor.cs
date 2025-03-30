using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SceneReferenceUtils.Editor
{
    [CustomPropertyDrawer(typeof(SceneReference))]
    public class SceneReferenceEditor : PropertyDrawer
    {
        private const int MARGIN_HEIGHT = 3;

        private float size;
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            SerializedProperty m_nameProperty = property.FindPropertyRelative("m_name");
            SerializedProperty m_sceneObjectProperty = property.FindPropertyRelative("m_sceneObject");
            Rect currentPosition = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            currentPosition = EditorGUI.PrefixLabel(currentPosition, label);
            size = 0;
            if (m_sceneObjectProperty != null)
            {
                m_sceneObjectProperty.objectReferenceValue = EditorGUI.ObjectField(currentPosition, m_sceneObjectProperty.objectReferenceValue, typeof(SceneAsset), false);
                if (m_sceneObjectProperty.objectReferenceValue != null)
                {
                    m_nameProperty.stringValue = m_sceneObjectProperty.objectReferenceValue.name;
                    currentPosition = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + MARGIN_HEIGHT, position.width, EditorGUIUtility.singleLineHeight * 2);
                    size = EditorGUIUtility.singleLineHeight * 2 + MARGIN_HEIGHT;
                    if (SceneUtility.GetBuildIndexByScenePath(AssetDatabase.GetAssetPath(m_sceneObjectProperty.objectReferenceValue)) < 0)
                    {
                        // size += EditorGUIUtility.singleLineHeight;
                        EditorGUI.HelpBox(currentPosition, "This scene isn't in the build settings.", MessageType.Warning);
                        if (GUILayout.Button("Add scene to build settings"))
                        {
                            List<EditorBuildSettingsScene> editorBuildSettingsScenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
                            editorBuildSettingsScenes.Add(new EditorBuildSettingsScene(AssetDatabase.GetAssetPath(m_sceneObjectProperty.objectReferenceValue) ,true));
                            EditorBuildSettings.scenes = editorBuildSettingsScenes.ToArray();
                        }
                    }
                    else
                    {
                        EditorGUI.HelpBox(currentPosition, "This scene is in the build settings.", MessageType.Info);
                    }
                }
            }
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return size + base.GetPropertyHeight(property, label); 
        }
    }
}
