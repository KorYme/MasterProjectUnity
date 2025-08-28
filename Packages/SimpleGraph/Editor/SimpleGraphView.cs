using System;
using System.Collections.Generic;
using System.Linq;
using SimpleGraph.Editor.Utils;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace SimpleGraph.Editor
{
    public class SimpleGraphView : GraphView
    {
        protected readonly SimpleGraphEditorWindow _graphEditorWindow;
        protected MiniMap _miniMap;
        protected SimpleSearchWindow _searchWindow;
        
        public event Action<bool> OnMiniMapVisibilityChanged;

        public bool IsMinimapVisible
        {
            get => _miniMap?.visible ?? false;
            set
            {
                if (_miniMap == null || IsMinimapVisible == value) return;
                _miniMap.visible = value;
                OnMiniMapVisibilityChanged?.Invoke(value);
            }
        }
        
        protected virtual string[] StyleSheets => new[]
        {
            "GraphViewStyles",
            "NodeStyles"
        };
        
        public SimpleGraphView(SimpleGraphEditorWindow graphEditorWindow)
        {
            _graphEditorWindow =  graphEditorWindow;
        }

        public virtual void Initialize()
        {
            AddManipulators();
            AddSearchWindow();
            AddMinimap();
            AddStyles();
            AddMiniMapStyles();
            AddGridBackground();
        }

        protected virtual void AddManipulators()
        {
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
        }
        
        protected virtual void AddSearchWindow()
        {
            if (_searchWindow) return;
            _searchWindow = ScriptableObject.CreateInstance<SimpleSearchWindow>();
            nodeCreationRequest = context => SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), _searchWindow);
        }
        
        protected virtual void AddMinimap()
        {
            _miniMap = new MiniMap()
            {
                anchored = true,
                visible = false,
                graphView = this,
                maxWidth = 200,
                maxHeight = 125,
            };
            _miniMap.SetPosition(new Rect(15, 50, 200, 125));
            Add(_miniMap);
        }
        
        protected void AddStyles() => this.LoadAndAddStyleSheets(StyleSheets);
        
        protected virtual void AddMiniMapStyles()
        {
            // In the future it should be added in the .uss file
            _miniMap.style.backgroundColor = new StyleColor(new Color32(29,29,29,255));
            _miniMap.style.borderBottomColor = new StyleColor(new Color32(51,51,51,255));
            _miniMap.style.borderTopColor = new StyleColor(new Color32(51,51,51,255));
            _miniMap.style.borderLeftColor = new StyleColor(new Color32(51,51,51,255));
            _miniMap.style.borderRightColor = new StyleColor(new Color32(51,51,51,255));
        }
        
        protected void AddGridBackground()
        {
            GridBackground gridBackground = new GridBackground()
            {
                name = "Background"
            };
            gridBackground.StretchToParentSize();
            Insert(0, gridBackground);
        }
        
        public void ToggleMinimapVisibility()
        {
            IsMinimapVisible = !IsMinimapVisible;
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            return ports.Where(port => port.node != startPort.node && port.direction != startPort.direction).ToList();
        }
        
        #region UTILITIES
        public Vector2 GetLocalMousePosition(Vector2 mousePosition, bool isSearchWindow = false)
        {
            Vector2 worldMousePosition = mousePosition;
            if (isSearchWindow)
            {
                worldMousePosition = _graphEditorWindow.rootVisualElement.ChangeCoordinatesTo(
                    _graphEditorWindow.rootVisualElement.parent, mousePosition - _graphEditorWindow.position.position) + Vector2.down * 20;
            }
            return contentViewContainer.WorldToLocal(worldMousePosition);
        }
        #endregion
    }
}
