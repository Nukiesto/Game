
using System.Collections.Generic;
using UnityEngine;

namespace UsefulScripts
{
    public static class RandomScripts
    {
        public static bool RandomBool()
        {
            return Random.Range(0, 2) == 0;
        }

        public static bool RandomBoolChance(int n)
        {
            return Random.Range(0, n) == 0;
        }

        public static T Choose<T>(ChanceItem[] vars)
        {
            var list = new List<T>();
            foreach (var item in vars)
            {
                if (RandomBoolChance(item.Chance))
                {
                    list.Add(item.Item);
                }
            }

            return list.Count > 0 ? list[Random.Range(0, list.Count)] : (T) vars[0].Item;
        }
        
        public struct ChanceItem
        {
            public dynamic Item;
            public int Chance;

            public ChanceItem(dynamic item, int chance)
            {
                Item = item;
                Chance = chance;
            }
        }
    }

    public static class OtherScripts
    {
        public static bool IsNull(dynamic value)
        {
            return ReferenceEquals(value, null);
        }
        
        public static T[] AppendToStart<T>(T[] array, T value) {
            var result = new T[array.Length + 1];
            result[0] = value;
            for (var i = 0; i < array.Length; i++)
                result[i + 1] = array[i];
 
            return result;
        }
    }
}