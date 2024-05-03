using System.Collections.Generic;

namespace MasterProject.Utilities
{
    public static class CollectionUtils
    {
        public static void Add<T>(this List<T> list, params T[] values)
        {
            for (int i = 0; i < list.Count; i++)
            {
                list.Add(values[i]);
            }
        }

        public static bool TryGet<T>(this List<T> list, int index, out T value)
        {
            if (index < list.Count)
            {
                value = list[index];
                return true;
            }
            value = default;
            return false;
        }

        public static bool TryAddUnique<T>(this List<T> list, T value)
        {
            if (!list.Contains(value))
            {
                list.Add(value);
                return true;
            }
            return false;
        }
    }
}