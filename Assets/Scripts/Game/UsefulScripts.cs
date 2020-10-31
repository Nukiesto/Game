
using System.Collections.Generic;
using UnityEngine;

namespace UsefulScripts
{
    public static class RandomScripts
    {
        public static bool RandomBool()
        {
            return UnityEngine.Random.Range(0, 2) == 0;
        }

        public static bool RandomBoolChance(int n)
        {
            return UnityEngine.Random.Range(0, n) == 0;
        }

        public static T Choose<T>(ChanceItem[] vars)
        {
            var list = new List<T>();
            for (var i = 0; i < vars.Length; i++)
            {
                var item = vars[i];
                if (RandomBoolChance(item.Chance))
                {
                    list.Add(item.Item);
                }
            }

            if (list.Count > 0)
                return list[Random.Range(0, list.Count)];
            else
                return vars[0].Item;
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
}