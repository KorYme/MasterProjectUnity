using System;

namespace RequiredAttributeRoslynAnalyzer
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct, AllowMultiple = true, Inherited = false)]
    public sealed class RequiredAttribute : Attribute
    {
        public Type RequiredAttributeType { get; }

        public RequiredAttribute(Type requiredAttributeType)
        {
            RequiredAttributeType = requiredAttributeType;
        }
    }
}