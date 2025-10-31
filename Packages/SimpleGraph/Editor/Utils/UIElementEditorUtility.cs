using System;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace SimpleGraph.Editor.Utils
{
    public static class UIElementEditorUtility
    {
        public static ObjectField CreateObjectField(string title = null, Type type = null, UnityEngine.Object initialValue = null, EventCallback<ChangeEvent<UnityEngine.Object>> callback = null)
        {
            ObjectField objectField = new ObjectField()
            {
                label = title,
                objectType = type,
                value = initialValue,
            };
            if (callback != null)
            {
                objectField.RegisterValueChangedCallback(callback);
            }
            return objectField;
        }
        
                
        /// <summary>
        /// Serialize an object in a parent visual element.
        /// </summary>
        /// <param name="obj">Object to serialize</param>
        /// <param name="parent">Parent to contain the newly serialized object</param>
        /// <param name="hideScript">Should hide or not the script ObjectField</param>
        public static void SerializeObjectInVisualElement(UnityEngine.Object obj, VisualElement parent, bool hideScript = true)
        {
            if (obj != null)
            {
                // Create a SerializedObject for the ScriptableObject
                SerializedObject serializedObject = new SerializedObject(obj);
                SerializedProperty property = serializedObject.GetIterator();
                bool isFirstDepth = true;
                if (hideScript)
                {
                    // Skip "Script" property
                    property.NextVisible(true);
                    isFirstDepth = false;
                }
                // Iterate through the properties and add them to the node
                while (property.NextVisible(isFirstDepth))
                {
                    isFirstDepth = false;
                    var field = new PropertyField(property.Copy()) { bindingPath = property.propertyPath };
                    field.Bind(serializedObject);
                    parent.Add(field);
                }
            }
        }
    }

    public static class UIElementEditorUtilityExtensions
    {
        public static Port CreatePort(this Node node, string name = null, string portName = null,
            Orientation orientation = Orientation.Horizontal, Direction direction = Direction.Output,
            Port.Capacity capacity = Port.Capacity.Single)
        {
            Port port = node.InstantiatePort(orientation, direction, capacity, typeof(bool));
            port.name = name;
            port.portName = portName;
            return port;
        }
    }
}
