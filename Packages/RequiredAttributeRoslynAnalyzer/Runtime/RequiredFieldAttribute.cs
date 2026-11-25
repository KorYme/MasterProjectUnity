using System;

namespace RequiredAttributeRoslynAnalyzer
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct, AllowMultiple = true)]
    public sealed class RequiredFieldAttribute : Attribute
    {
        public Type RequiredAttributeType { get; }

        public RequiredFieldAttribute(Type requiredAttributeType)
        {
            RequiredAttributeType = requiredAttributeType;
        }
    }
}