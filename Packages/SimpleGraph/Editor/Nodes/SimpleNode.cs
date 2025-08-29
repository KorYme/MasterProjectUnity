using GraphTool.Utils;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace SimpleGraph.Editor.Nodes
{
    [SimpleNodeInfo("Simple Graph/Simple Node")]
    public class SimpleNode : Node
    {
        protected SimpleGraphView _graphView;
        
        public SimpleNode()
        {
            mainContainer.AddClasses("node__main-container");
            extensionContainer.AddClasses("node__extension-container");
        }

        public virtual void Initialize(SimpleGraphView graphView, Vector2 position)
        {
            _graphView = graphView;
            SetPosition(new Rect(position, Vector2.zero));
        }
        
        public virtual void Draw()
        {
            // TITLE CONTAINER
            DrawTitleContainer();

            // MAIN CONTAINER
            DrawMainContainer();

            // INPUT CONTAINER
            DrawInputContainer();

            // OUTPUT CONTAINER
            DrawOutputContainer();

            // EXTENSION CONTAINER
            DrawExtensionContainer();

            // USEFUL CALL
            RefreshExpandedState();
        }

        protected virtual void DrawTitleContainer()
        {
            TextField simpleNodeTitle = UIElementUtility.CreateTextField(GetType().Name, null, 
                _ => {});
            titleContainer.Insert(0, simpleNodeTitle);
            simpleNodeTitle.AddClasses(
                "node__text-field",
                "node__text-field__hidden",
                "node__filename-text-field"
            );
        }
        
        protected virtual void DrawMainContainer() { }
        
        protected virtual void DrawInputContainer() { }

        protected virtual void DrawOutputContainer() { }

        protected virtual void DrawExtensionContainer() { }
    }
}
