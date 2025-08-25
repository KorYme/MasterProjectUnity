using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;

namespace GraphTool.Editor
{
    public class SearchWindow : ScriptableObject, ISearchWindowProvider
    {
        #region FIELDS_AND_CONSTANTS
        const bool IS_WINDOW_CLOSED_ON_ENTRY_SELECTED = true;

        GraphView _graphView;
        Texture2D _spacingIcon;
        #endregion

        #region METHODS
        public void Initialize(GraphView graphView)
        {
            _graphView = graphView;
            _spacingIcon = new Texture2D(1, 1);
            _spacingIcon.SetPixel(0, 0, Color.clear);
            _spacingIcon.Apply();
        }
        #endregion

        #region INTERFACE_METHODS
        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            return new List<SearchTreeEntry>()
            {
                // ReSharper disable once RedundantArgumentDefaultValue
                new SearchTreeGroupEntry(new GUIContent("Create Element"), 0),
                new SearchTreeGroupEntry(new GUIContent("Dialogue Node"), 1),
                new SearchTreeEntry(new GUIContent("Choice Node", _spacingIcon))
                {
                    level = 2,
                    userData = nameof(ChoiceNode),
                },
                new SearchTreeEntry(new GUIContent("Create Group", _spacingIcon))
                {
                    level = 1,
                    userData = nameof(GraphGroup),
                },
            };
        }

        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
        {
            switch (SearchTreeEntry.userData)
            {
                case nameof(ChoiceNode): _graphView.CreateAndAddNode<ChoiceNode>(_graphView.GetLocalMousePosition(context.screenMousePosition, true)); break;
                case nameof(GraphGroup): _graphView.CreateAndAddGroup<GraphGroup>(_graphView.GetLocalMousePosition(context.screenMousePosition, true)); break;
                default: return false;
            }
            return IS_WINDOW_CLOSED_ON_ENTRY_SELECTED;
        }
        #endregion
    }
}
