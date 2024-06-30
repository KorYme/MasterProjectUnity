using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MasterProject.Utilities
{
    public static class InjectionUtilities
    {
        public static void InjectDependencies<U>(object injectionObject, Type attributeType, Dictionary<Type, U> injectedTypes)
        {
            if (!attributeType.IsSubclassOf(typeof(InjectionAttribute)))
            {
                throw new ArgumentException($"The type {attributeType.GetType()} is not an attribute class !");
            }

            Type injectionObjectType = injectionObject.GetType();
            MemberInfo[] test = injectionObjectType.GetMembers();
            foreach (MemberInfo member in injectionObjectType.GetMembers()
                .Where(memberInfo => memberInfo.CustomAttributes.Any(attributeData => attributeData.AttributeType == attributeType)))
            {
                FieldInfo field = injectionObjectType.GetField(member.Name);
                if (!field.FieldType.IsAbstract && (field.FieldType.IsSubclassOf(typeof(U)) || Equals(field.FieldType, typeof(U))))
                {
                    if (injectedTypes.TryGetValue(field.FieldType, out U injectedType))
                    {
                        field.SetValue(injectionObject, injectedType);
                    }
                    else
                    {
                        throw new Exception($"No {field.FieldType.Name} has been found in the dictionnay");
                    }
                }
                else
                {
                    throw new Exception($"You can't use the attribute {attributeType.Name} on the field {field.Name} in {injectionObjectType.Name} class");
                }
            }
            foreach (MemberInfo member in injectionObjectType.GetMembers()
                .Where(memberInfo => memberInfo.CustomAttributes.Any(attributeData => attributeData.AttributeType == typeof(InjectDependencies))))
            {
                FieldInfo field = injectionObjectType.GetField(member.Name);
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
                            InjectDependencies(enumerator.Current, attributeType, injectedTypes);
                        }
                    }
                    while (enumerator.MoveNext());
                }
                else
                {
                    InjectDependencies(field.GetValue(injectionObject), attributeType, injectedTypes);
                }
            }
        }
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

    }
}
