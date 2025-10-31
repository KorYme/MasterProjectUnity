using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace SimpleGraph.Editor
{
    public struct SearchContextElement
    {
        public readonly Type TargetType;
        public readonly string Title;

        public SearchContextElement(Type targetType, string title)
        {
            TargetType = targetType;
            Title = title;
        }

        public SearchContextElement(SearchContextElement copy)
        {
            TargetType  = copy.TargetType;
            Title = copy.Title;
        }
    }
    
    public class SimpleSearchWindow : ScriptableObject, ISearchWindowProvider
    {
        #region FIELDS_AND_CONSTANTS
        const bool IS_WINDOW_CLOSED_ON_ENTRY_SELECTED = true;

        private SimpleGraphView _graphView;
        private Texture2D _spacingIcon;
        public VisualElement Target { get; set; }

        private static List<SearchContextElement> _elements;
        #endregion
        
        #region METHODS
        public SimpleSearchWindow Initialize(SimpleGraphView graphView)
        {
            _graphView = graphView;
            _spacingIcon = new Texture2D(1, 1);
            _spacingIcon.SetPixel(0, 0, Color.clear);
            _spacingIcon.Apply();
            return this;
        }
        
        List<SearchTreeEntry> ISearchWindowProvider.CreateSearchTree(SearchWindowContext context)
        {
            // TODO : Implement Attribute with reflection to enable some nodes only
            List<SearchTreeEntry> tree =  new List<SearchTreeEntry>();
            tree.Add(new SearchTreeGroupEntry(new GUIContent("Create Element")));
            
            _elements = new List<SearchContextElement>();
            
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            string[] usedAssembliesNames = _graphView.GetNodeDataAssembliesForGraphEditor();

            foreach (Assembly assembly in assemblies)
            {
                if (!usedAssembliesNames.Contains(assembly.FullName)) continue;
                foreach (Type type in assembly.GetTypes())
                {
                    if (type.IsAbstract || !typeof(SimpleNodeData).IsAssignableFrom(type) || !type.CustomAttributes.Any()) continue;
                    
                    Attribute attribute = type.GetCustomAttribute(typeof(SimpleNodeInfoAttribute));
                    if (attribute is not SimpleNodeInfoAttribute simpleAttribute) continue;
                    if (string.IsNullOrEmpty(simpleAttribute.MenuItem)) continue;

                    _elements.Add(new SearchContextElement(type, simpleAttribute.MenuItem));
                }
            }
            
            // Sort by name
            _elements.Sort((entry1, entry2) =>
            {
                string[] splits1 = entry1.Title.Split('/');
                string[] splits2 = entry2.Title.Split('/');
                for (int i = 0; i < splits1.Length; i++)
                {
                    if (i >= splits2.Length) return 1;
                    
                    int value = string.Compare(splits1[i], splits2[i], StringComparison.Ordinal);
                    if (value == 0) continue;
                    
                    // Make sure that leaves go before nodes
                    if (splits1[i].Length != splits2[i].Length && (i == splits1.Length - 1 || i == splits2.Length - 1))
                        return splits1.Length < splits2.Length ? 1 : -1;
                    return value;
                }
                return 0;
            });
            
            List<string> groups = new List<string>();

            foreach (SearchContextElement element in _elements)
            {
                string[] entryTitle = element.Title.Split('/');

                string groupName = string.Empty;

                for (int i = 0; i < entryTitle.Length - 1; i++)
                {
                    groupName += entryTitle[i];
                    if (!groups.Contains(groupName))
                    {
                        tree.Add(new SearchTreeGroupEntry(new GUIContent(entryTitle[i]), i + 1));
                        groups.Add(groupName);
                    }
                    groupName += "/";
                }
                
                SearchTreeEntry entry = new SearchTreeEntry(new GUIContent(entryTitle.Last(), _spacingIcon));
                entry.level = entryTitle.Length;
                entry.userData = new SearchContextElement(element);
                tree.Add(entry);
            }
            // TODO: Create Group Entry
            
            return tree;
        }

        bool ISearchWindowProvider.OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
        {
            if (searchTreeEntry.userData is SearchContextElement contextElement)
            {
                _graphView.CreateNewNode(contextElement.TargetType, _graphView.GetGraphMousePosition(context.screenMousePosition));
            }
            return IS_WINDOW_CLOSED_ON_ENTRY_SELECTED;
        }
        #endregion
    }
}
