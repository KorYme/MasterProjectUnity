using System.Reflection;

namespace SimpleGraph.Editor.Utils
{
    public static class ReflectionEditorUtility
    {
        private const string PROPERTY_BACKING_FIELD_NAME = "<{0}>k__BackingField";
            
        public static string GetRelativePropertyPath(this MemberInfo memberInfo)
        {
            return memberInfo.MemberType.HasFlag(MemberTypes.Property) ? GetPropertyPropertyPath(memberInfo.Name) : memberInfo.Name;
        }

        public static string GetPropertyPropertyPath(string propertyName)
        {
            return string.Format(PROPERTY_BACKING_FIELD_NAME, propertyName);
        }
    }
}
