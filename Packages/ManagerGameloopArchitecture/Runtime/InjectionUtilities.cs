using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ManagerInjection.Utilities
{
    public static class InjectionUtilities
    {
        public static void InjectDependencies<U>(Type attributeType, IReadOnlyDictionary<Type, U> injectedTypes, params object[] injectionObjects)
        {
            if (!attributeType.IsSubclassOf(typeof(InjectionAttribute)))
            {
                throw new ArgumentException($"The type {attributeType} is not an attribute class !");
            }

            Type injectedBaseType = typeof(U);
            foreach (object injectionObject in injectionObjects)
            {
                Type injectionObjectType = injectionObject.GetType();

                IEnumerable<FieldInfo> matchingMembers = injectionObjectType.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                    .Where(fieldInfo => fieldInfo.CustomAttributes.Any(attributeData => attributeData.AttributeType == attributeType));
                foreach (FieldInfo field in matchingMembers)
                {
                    if (injectedBaseType.IsAssignableFrom(field.FieldType))
                    {
                        if (injectedTypes.TryGetValue(field.FieldType, out U injectedType))
                        {
                            field.SetValue(injectionObject, injectedType);
                        }
                        else if (field.GetCustomAttribute<InjectionAttribute>() is InjectManager serviceDepencencyAttribute
                            && serviceDepencencyAttribute.DependencyNecessity == DependencyNecessity.Mandatory)
                        {
                            throw new Exception($"No {field.FieldType.Name} has been found in the dictionnay");
                        }
                    }
                    else
                    {
                        throw new Exception($"You can't use the attribute {attributeType.Name} on the field {field.Name} in {injectionObjectType.Name} class");
                    }
                }
            }
        }
    }

    public enum DependencyNecessity
    {
        Mandatory,
        Optional,
    }

    public class InjectionAttribute : Attribute
    {

    }

    public class InjectDependencies : InjectionAttribute
    {
        public bool IsAlreadyInjected { get; set; } = false;
    }

    public class InjectManager : InjectionAttribute
    {
        public DependencyNecessity DependencyNecessity { get; set; }

        public InjectManager(DependencyNecessity dependencyNecessity = DependencyNecessity.Mandatory)
        {
            DependencyNecessity = dependencyNecessity;
        }
    }
}
