using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

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

        public static bool TryAddKvp<TKey, TValue>(this Dictionary<TKey, TValue> dico, KeyValuePair<TKey, TValue> kvp)
        {
            if (dico.TryAdd(kvp.Key, kvp.Value))
            {
                return true;
            }
            return false;
        }

        public static Dictionary<TKey, TValue> MapListsIntoDictionary<TKey, TValue>(IEnumerable<TKey> keys, IEnumerable<TValue> values)
        {
            return keys.Zip(values, (k, v) => new { k, v }).ToDictionary(x => x.k, x => x.v);
        }
    }
}