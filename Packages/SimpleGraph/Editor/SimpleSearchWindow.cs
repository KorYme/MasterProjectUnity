using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SimpleGraph.Editor.Nodes;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

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
        
        public static List<SearchContextElement> Elements;
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
            
            Elements = new List<SearchContextElement>();
            
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (Assembly assembly in assemblies)
            {
                if (!_graphView.UsedNodesAssemblies().Contains(assembly.FullName)) continue;
                foreach (Type type in assembly.GetTypes())
                {
                    if (!type.IsAbstract && typeof(SimpleNode).IsAssignableFrom(type) && type.CustomAttributes.Count() > 0)
                    {
                        Attribute attribute = type.GetCustomAttribute(typeof(SimpleNodeInfoAttribute));
                        if (attribute != null && attribute is SimpleNodeInfoAttribute simpleAttribute)
                        {
                            if (string.IsNullOrEmpty(simpleAttribute.MenuItem)) continue;

                            Elements.Add(new SearchContextElement(type, simpleAttribute.MenuItem));
                        }
                    }
                }
            }
            
            // Sort by name
            Elements.Sort((entry1, entry2) =>
            {
                string[] splits1 = entry1.Title.Split('/');
                string[] splits2 = entry2.Title.Split('/');
                for (int i = 0; i < splits1.Length; i++)
                {
                    if (i >= splits2.Length) return 1;
                    
                    int value = String.Compare(splits1[i], splits2[i], StringComparison.Ordinal);
                    if (value != 0)
                    {
                        // Make sure that leaves go before nodes
                        if (splits1[i].Length != splits2[i].Length && (i == splits1.Length - 1 || i == splits2.Length - 1))
                            return splits1.Length < splits2.Length ? 1 : -1;
                        return value;
                    }
                }
                return 0;
            });
            
            List<string> groups = new List<string>();

            foreach (SearchContextElement element in Elements)
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

        bool ISearchWindowProvider.OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
        {
            // TODO : Search through all classes and generate node with reflection
            if (SearchTreeEntry.userData is SearchContextElement contextElement)
            {
                _graphView.CreateAndAddNode(contextElement.TargetType, _graphView.GetLocalMousePosition(context.screenMousePosition));
                Debug.Log($"You tried to create a node of type {contextElement.TargetType} and title {contextElement.Title}.");
            }
            return IS_WINDOW_CLOSED_ON_ENTRY_SELECTED;
        }
        #endregion
    }
}
