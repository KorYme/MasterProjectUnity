using System;

namespace SimpleGraph
{
    [AttributeUsage(AttributeTargets.Class, Inherited=true, AllowMultiple=false)]
    public class SimpleNodeInfoAttribute : Attribute
    {
        public string NodeName { get; private set; }
        public string MenuItem { get; private set; }
        
        public SimpleNodeInfoAttribute(string nodeName, string menuItem)
        {
            NodeName = nodeName;
            MenuItem = menuItem;
        }
    }
}