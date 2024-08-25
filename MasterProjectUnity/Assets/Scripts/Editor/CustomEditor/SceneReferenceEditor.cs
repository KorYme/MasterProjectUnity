using UnityEditor;
using UnityEngine;

namespace MasterProject.Editor
{
    [CustomPropertyDrawer(typeof(SceneReference))]
    public class SceneReferenceEditor : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            SerializedProperty m_nameProperty = property.FindPropertyRelative("m_name");
            SerializedProperty m_sceneObjectProperty = property.FindPropertyRelative("m_sceneObject");
            position = EditorGUI.PrefixLabel(position, label);
            if (m_sceneObjectProperty != null)
            {
                m_sceneObjectProperty.objectReferenceValue = EditorGUI.ObjectField(position, m_sceneObjectProperty.objectReferenceValue, typeof(SceneAsset), false);
                if (m_sceneObjectProperty.objectReferenceValue != null)
                {
                    m_nameProperty.stringValue = m_sceneObjectProperty.objectReferenceValue.name;
                }
            }
            EditorGUI.EndProperty();
        }
    }
}
