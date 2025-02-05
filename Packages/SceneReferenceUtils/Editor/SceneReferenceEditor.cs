using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SceneReferenceUtils.Editor
{
    [CustomPropertyDrawer(typeof(SceneReference))]
    public class SceneReferenceEditor : PropertyDrawer
    {
        private const int MARGIN_HEIGHT = 3;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            SerializedProperty m_nameProperty = property.FindPropertyRelative("m_name");
            SerializedProperty m_sceneObjectProperty = property.FindPropertyRelative("m_sceneObject");
            Rect currentPosition = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            currentPosition = EditorGUI.PrefixLabel(currentPosition, label);
            if (m_sceneObjectProperty != null)
            {
                m_sceneObjectProperty.objectReferenceValue = EditorGUI.ObjectField(currentPosition, m_sceneObjectProperty.objectReferenceValue, typeof(SceneAsset), false);
                if (m_sceneObjectProperty.objectReferenceValue != null)
                {
                    m_nameProperty.stringValue = m_sceneObjectProperty.objectReferenceValue.name;
                    if (SceneUtility.GetBuildIndexByScenePath(AssetDatabase.GetAssetPath(m_sceneObjectProperty.objectReferenceValue)) < 0)
                    {
                        currentPosition = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + MARGIN_HEIGHT, position.width, EditorGUIUtility.singleLineHeight * 2);
                        EditorGUI.HelpBox(currentPosition, "This scene isn't in the build settings.", MessageType.Warning);
                    }
                }
            }
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty m_sceneObjectProperty = property.FindPropertyRelative("m_sceneObject");
            return base.GetPropertyHeight(property, label) + 
                (SceneUtility.GetBuildIndexByScenePath(AssetDatabase.GetAssetPath(m_sceneObjectProperty.objectReferenceValue)) < 0 
                ? EditorGUIUtility.singleLineHeight * 2 + MARGIN_HEIGHT : 0);
        }
    }
}
