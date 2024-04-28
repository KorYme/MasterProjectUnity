using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace MasterProject.Editor.Utilities
{
    public static class UIElementUtils
    {
        /// <summary>
        /// Serialize an object in a parent visual element.
        /// Will always clear the parent container before doing so.
        /// </summary>
        /// <param name="obj">Object to serialize</param>
        /// <param name="parent">Parent to contain the newly serialized object</param>
        /// <param name="hideScript">Should hide or not the script ObjectField</param>
        public static void SerializeObjectInVisualElement(UnityEngine.Object obj, VisualElement parent, bool hideScript = true)
        {
            parent.Clear();
            if (obj != null)
            {
                // Create a SerializedObject for the ScriptableObject
                SerializedObject serializedObject = new SerializedObject(obj);
                SerializedProperty property = serializedObject.GetIterator();
                if (hideScript)
                {
                    // Skip "Script" property
                    property.NextVisible(true);
                }
                // Iterate through the properties and add them to the node
                while (property.NextVisible(false))
                {
                    var field = new PropertyField(property.Copy()) { bindingPath = property.propertyPath };
                    field.Bind(serializedObject);
                    parent.Add(field);
                }
            }
        }
    }
}
