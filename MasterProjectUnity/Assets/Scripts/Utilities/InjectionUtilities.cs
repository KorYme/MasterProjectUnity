using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MasterProject.Utilities
{
    public static class InjectionUtilities
    {
        public static void InjectDependencies<T,U>(T injectionObject, Type attributeType, Dictionary<Type, U> injectedTypes) where T : class
        {
            if (!attributeType.IsSubclassOf(typeof(Attribute)))
            {
                throw new ArgumentException($"The type {attributeType.GetType()} is not an attribute class !");
            }
            Type injectionObjectType = typeof(T);
            MemberInfo[] test = injectionObjectType.GetMembers();
            foreach (MemberInfo member in injectionObjectType.GetMembers()
                .Where(memberInfo => memberInfo.CustomAttributes.Any(y => y.AttributeType == typeof(InjectionDependency))))
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
        }
    }

    public class InjectionDependency : Attribute
    {

    }
}
