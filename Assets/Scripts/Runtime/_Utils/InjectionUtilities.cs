using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MasterProject.Utilities
{
    public static class InjectionUtilities
    {
        public static void InjectDependencies<U>(Type attributeType, IReadOnlyDictionary<Type, U> injectedTypes, params object[] injectionObjects)
        {
            if (!attributeType.IsSubclassOf(typeof(InjectionAttribute)))
            {
                throw new ArgumentException($"The type {attributeType.GetType()} is not an attribute class !");
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
                        else if (field.GetCustomAttribute<InjectionAttribute>() is ServiceDepencency serviceDepencencyAttribute
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

                matchingMembers = injectionObjectType.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                    .Where(fieldInfo => fieldInfo.CustomAttributes.Any(attributeData => attributeData.AttributeType == typeof(InjectDependencies)));
                foreach (FieldInfo field in matchingMembers)
                {
                    if (injectionObjectType.GetCustomAttribute(typeof(InjectDependencies)) is InjectDependencies injectDependenciesAttribute
                        && !injectDependenciesAttribute.IsAlreadyInjected)
                    {
                        injectDependenciesAttribute.IsAlreadyInjected = true;
                    }
                    else
                    {
                        throw new Exception($"The code has been stopped due to infite injection loop in your code in {injectionObjectType.Name} object");
                    }
                    if (field.GetValue(injectionObject) is IEnumerable enumerable)
                    {
                        IEnumerator enumerator = enumerable.GetEnumerator();
                        do
                        {
                            if (enumerator.Current != null)
                            {
                                InjectDependencies(attributeType, injectedTypes, enumerator.Current);
                            }
                        }
                        while (enumerator.MoveNext());
                    }
                    else
                    {
                        InjectDependencies(attributeType, injectedTypes, field.GetValue(injectionObject));
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

    public class ServiceDepencency : InjectionAttribute
    {
        public DependencyNecessity DependencyNecessity { get; set; }

        public ServiceDepencency(DependencyNecessity dependencyNecessity = DependencyNecessity.Mandatory)
        {
            DependencyNecessity = dependencyNecessity;
        }
    }
}
