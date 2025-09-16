using System;
using System.Reflection;
using GraphTool.Utils;
using SimpleGraph.Editor.Utils;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace SimpleGraph.Editor
{
    public class SimpleNode : Node
    {
        public readonly SimpleNodeData NodeData;
        private readonly SerializedObject _graphSerializedObject;
        private SerializedProperty _serializedProperty;

        public event Action OnNodeModified;

        public SimpleNode(SimpleNodeData nodeData, SerializedObject graphSerializedObject)
        {
            NodeData = nodeData;
            _graphSerializedObject = graphSerializedObject;
            InitializeNode();
        }

        private void InitializeNode()
        {
            mainContainer.AddClasses("node__main-container");
            extensionContainer.AddClasses("node__extension-container");
            
            SetPosition(NodeData.Position);

            Type typeInfo = NodeData.GetType();
            SimpleNodeInfoAttribute info = typeInfo.GetCustomAttribute<SimpleNodeInfoAttribute>();
            title = info.NodeName;
            name = typeInfo.Name;

            string[] depths = info.MenuItem.Split('/');
            foreach (string depth in depths)
            {
                this.AddToClassList(depth.ToLower().Replace(" ", "-"));
            }

            Draw();
        }

        public void SavePosition()
        {
            NodeData.Position = GetPosition();
        }
        
        #region CONTAINERS_DRAWERS
        private void Draw()
        {
            DrawTitleContainer(titleContainer);

            DrawMainContainer(mainContainer);

            DrawInputContainer(inputContainer);

            DrawOutputContainer(outputContainer);

            DrawExtensionContainer(extensionContainer);

            // USEFUL CALL
            RefreshExpandedState();
        }

        private void DrawTitleContainer(VisualElement container) { }

        private void DrawMainContainer(VisualElement container) { }

        private void DrawInputContainer(VisualElement container)
        {
            Type typeInfo = NodeData.GetType();
            MemberInfo[] memberInfos = typeInfo.GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (MemberInfo memberInfo in memberInfos)
            {
                if (memberInfo.GetCustomAttribute<ExposedPortAttribute>() is not ExposedInputPortAttribute) continue;
                
                Type portDataType = null;
                
                switch (memberInfo.MemberType)
                {
                    case MemberTypes.Field:
                        
                        if (memberInfo is not FieldInfo fieldInfo) continue;
                        
                        if (fieldInfo.FieldType.IsGenericType
                            && fieldInfo.FieldType.GetGenericTypeDefinition() == typeof(SimplePortData<>))
                        {
                            portDataType = fieldInfo.FieldType.GenericTypeArguments[0];
                        }
                        
                        break;
                    case MemberTypes.Property:
                        
                        if (memberInfo is not PropertyInfo propertyInfo) continue;
                        
                        if (propertyInfo.PropertyType.IsGenericType
                            && propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(SimplePortData<>))
                        {
                            portDataType = propertyInfo.PropertyType.GenericTypeArguments[0];
                        }
                        
                        break;
                    default:
                        continue;
                }
                
                if (portDataType == null) continue;
                    
                Port port = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, portDataType);
                container.Add(port);
            }
        }

        private void DrawOutputContainer(VisualElement container)
        {
            Type typeInfo = NodeData.GetType();
            MemberInfo[] memberInfos = typeInfo.GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (MemberInfo memberInfo in memberInfos)
            {
                if (memberInfo.GetCustomAttribute<ExposedPortAttribute>() is not ExposedOutputPortAttribute) continue;
                
                Type portDataType = null;
                
                switch (memberInfo.MemberType)
                {
                    case MemberTypes.Field:
                        
                        FieldInfo fieldInfo = memberInfo as FieldInfo;
                        if (fieldInfo == null) continue;
                        
                        if (fieldInfo.FieldType.IsGenericType
                            && fieldInfo.FieldType.GetGenericTypeDefinition() == typeof(SimplePortData<>))
                        {
                            portDataType = fieldInfo.FieldType.GenericTypeArguments[0];
                        }
                        
                        break;
                    case MemberTypes.Property:
                        
                        PropertyInfo propertyInfo = memberInfo as PropertyInfo;
                        if (propertyInfo == null) continue;
                        
                        if (propertyInfo.PropertyType.IsGenericType
                            && propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(SimplePortData<>))
                        {
                            portDataType = propertyInfo.PropertyType.GenericTypeArguments[0];
                        }
                        
                        break;
                    default:
                        continue;
                }
                
                if (portDataType == null) continue;
                
                Port port = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, portDataType);
                container.Add(port);
            }
        }

        private void DrawExtensionContainer(VisualElement container)
        {
            Type typeInfo = NodeData.GetType();
            MemberInfo[] memberInfos = typeInfo.GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (MemberInfo memberInfo in memberInfos)
            {
                if (memberInfo.GetCustomAttribute<ExposedPropertyAttribute>() is { } exposedPropertyAttribute)
                {
                    PropertyField propertyField = DrawProperty(memberInfo, container);
                    if (propertyField == null)
                    {
                        Debug.LogError($"The Property {memberInfo.Name} could not be found.");
                        continue;
                    }
                    propertyField.RegisterValueChangeCallback(_ => OnNodeModified?.Invoke());
                }
            }
        }
        #endregion
        
        #region PROPERTIES_DRAWERS
        private PropertyField DrawProperty(MemberInfo memberInfo, VisualElement container)
        {
            _serializedProperty = GetSerializedProperty();
            if (_serializedProperty == null) return null;
            
            SerializedProperty property = _serializedProperty.FindPropertyRelative(memberInfo.GetRelativePropertyPath());
            PropertyField propertyField = new PropertyField(property)
            {
                bindingPath = property.propertyPath,
            };
            container.Add(propertyField);
            return propertyField;
        }

        private SerializedProperty GetSerializedProperty()
        {
            if (_serializedProperty != null) return _serializedProperty;
            SerializedProperty nodes = _graphSerializedObject.FindProperty(ReflectionEditorUtility.GetPropertyPropertyPath("Nodes"));
            if (nodes.isArray)
            {
                int size = nodes.arraySize;
                for (int i = 0; i < size; i++)
                {
                    SerializedProperty element = nodes.GetArrayElementAtIndex(i);
                    SerializedProperty elementId = element.FindPropertyRelative(ReflectionEditorUtility.GetPropertyPropertyPath("Id"));
                    if (elementId.stringValue.Equals(NodeData.Id))
                    {
                        return element;
                    }
                }
            }
            return null;
        }
        #endregion
    }
}
