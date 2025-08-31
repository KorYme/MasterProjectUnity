using System;
using UnityEditor.Experimental.GraphView;

namespace SimpleGraph.Editor
{
    public class SimplePort : Port
    {
        // TODO
        protected SimplePort(Orientation portOrientation, Direction portDirection, Capacity portCapacity, Type type) : base(portOrientation, portDirection, portCapacity, type)
        {
        }
    }
}