using System;
using UnityEngine.UIElements;

namespace GraphTool.Utils
{
    public static class UIElementUtility
    {
        public static VisualElement Add(this VisualElement element, params VisualElement[] childs)
        {
            for (int i = 0; i < childs.Length; i++)
            {
                element.Add(childs[i]);
            }
            return element;
        }
        
        public static TextField CreateTextField(string initialValue = null, string labelValue = null, EventCallback<ChangeEvent<string>> onChangedCallback = null, bool isSelectable = true, bool multiLine = false)
        {
            TextField textField = new TextField()
            {
                value = initialValue,
                label = labelValue,
                isReadOnly = true,
                textSelection =
                {
                    isSelectable = isSelectable
                }, 
                multiline = multiLine,
            };
            if (onChangedCallback != null)
            {
                textField.RegisterValueChangedCallback(onChangedCallback);
            }
            return textField;
        }

        public static TextField CreateTextArea(string initialValue = null, string labelValue = null, EventCallback<ChangeEvent<string>> onChangedCallback = null)
        {
            return CreateTextField(initialValue, labelValue, onChangedCallback, multiLine: true);;
        }

        public static Foldout CreateFoldout(string title, bool collapsed = false)
        { 
            return new Foldout(){ text = title, value = !collapsed };
        } 

        public static Button CreateButton(string title, params Action[] onClickCallbacks)
        {
            Button button = new Button()
            {
                text = title
            };
            for (int i = 0; i < onClickCallbacks.Length; i++)
            {
                if (onClickCallbacks[i] == null) continue;
                button.clicked += onClickCallbacks[i];
            }
            return button;
        }

        public static Toggle CreateToggle(bool initialValue = false, string labelValue = null, EventCallback<ChangeEvent<bool>> callback = null)
        {
            Toggle toggle = new Toggle()
            {
                label = labelValue,
                value = initialValue,
                
            };
            toggle.RegisterValueChangedCallback(callback);
            return toggle;
        }
        
        public static VisualElement AddClasses(this VisualElement element, params string[] classNames)
        {
            foreach (string className in classNames)
            {
                element.AddToClassList(className);
            }
            return element;
        }

        public static VisualElement RemoveClasses(this VisualElement element, params string[] classNames)
        {
            foreach (string className in classNames)
            {
                element.RemoveFromClassList(className);
            }
            return element;
        }
    }
}

