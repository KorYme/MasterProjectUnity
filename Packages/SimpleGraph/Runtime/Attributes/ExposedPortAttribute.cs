using System;
using UnityEditor.Experimental.GraphView;

namespace SimpleGraph
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class ExposedPortAttribute : Attribute
    {
        public readonly Direction PortDirection;

        public ExposedPortAttribute(Direction portDirection)
        {
            PortDirection = portDirection;
        }
    }
}
