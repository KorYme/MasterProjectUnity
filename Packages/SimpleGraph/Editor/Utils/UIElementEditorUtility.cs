using System;
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
