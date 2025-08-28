using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace SimpleGraph.Editor
{
    public class SimpleSearchWindow : ScriptableObject, ISearchWindowProvider
    {
        #region FIELDS_AND_CONSTANTS
        const bool IS_WINDOW_CLOSED_ON_ENTRY_SELECTED = true;

        private SimpleGraphView _graphView;
        private Texture2D _spacingIcon;
        #endregion
        
        #region METHODS

        public void Initialize(SimpleGraphView graphView)
        {
            _graphView = graphView;
            _spacingIcon = new Texture2D(1, 1);
            _spacingIcon.SetPixel(0, 0, Color.clear);
            _spacingIcon.Apply();
        }
        
        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            // TODO : Implement Attribute with reflection to enable some nodes only
            return new List<SearchTreeEntry>()
            {
                new SearchTreeGroupEntry(new GUIContent("Create Element"), 0),
            };
        }

        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
        {
            // TODO : Search through all classes and generate node with reflection
            return IS_WINDOW_CLOSED_ON_ENTRY_SELECTED;
        }
        #endregion
    }
}
