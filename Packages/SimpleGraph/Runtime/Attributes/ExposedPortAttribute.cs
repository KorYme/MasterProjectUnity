using System;

namespace SimpleGraph
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public abstract class ExposedPortAttribute : Attribute
    {
    }
    
    public class ExposedInputPortAttribute : ExposedPortAttribute
    {
    }
    
    public class ExposedOutputPortAttribute : ExposedPortAttribute
    {
    }
}
