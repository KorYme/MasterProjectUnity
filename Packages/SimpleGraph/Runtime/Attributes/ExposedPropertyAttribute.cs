using System;

namespace SimpleGraph
{
    /// <summary>
    /// Expose the value in the graph node
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class ExposedPropertyAttribute : Attribute
    {
    }
}
