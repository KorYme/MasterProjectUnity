using System;

namespace SimpleGraph.Editor
{
    public class SimpleNodeInfoAttribute : Attribute
    {
        public string MenuItem { get; private set; }
        
        public SimpleNodeInfoAttribute(string menuItem)
        {
            MenuItem = menuItem;
        }
    }
}
