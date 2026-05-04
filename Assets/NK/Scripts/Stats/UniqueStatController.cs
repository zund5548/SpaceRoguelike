using UnityEngine;
using Ships;
using System;
using System.Collections.Generic;
using Items;
namespace Stats
{
    public class UniqueStatController
    {
        public Dictionary<Type,UniqueStatSet> statDic = new();
        //public Dictionary<Type,UniqueStatCollection> statDicByCollection = new();
        public T GetUniqueStat<T>() where T : UniqueStatSet, new()
        {
            statDic.TryGetValue(typeof(T), out var stat);
            return stat as T;
        }
        public T AddUniqueStat<T>(T addedStat) where T : UniqueStatSet
        {
            var type = typeof(T);
            if (!statDic.TryGetValue(type, out var stat))
            {
                stat = addedStat;
                statDic.Add(type, stat);
            }
            return (T)stat;
        }
        // public T AddUniqueStat<T>() where T : UniqueStatSet, new()
        // {
        //     var type = typeof(T);
        //     if (!statDic.TryGetValue(type, out var stat))
        //     {
        //         stat = new T();
        //         statDic.Add(type, stat);
        //     }
        //     return (T)stat;
        // }

        //collcectionによる
        // public T GetUniqueStatByCollection<T>() where T : UniqueStatCollection, new()
        // {
        //     statDic.TryGetValue(typeof(T), out var stat);
        //     return stat as T;
        // }
        // public T AddUniqueStatByCollection<T>(T addedStat) where T : UniqueStatCollection
        // {
        //     var type = typeof(T);
        //     if (!statDic.TryGetValue(type, out var stat))
        //     {
        //         stat = addedStat;
        //         statDicByCollection.Add(type, stat);
        //     }
        //     return (T)stat;
        // }
        // public T AddUniqueStatByCollection<T>() where T : UniqueStatCollection, new()
        // {
        //     var type = typeof(T);
        //     if (!statDic.TryGetValue(type, out var stat))
        //     {
        //         stat = new T();
        //         statDic.Add(type, stat);
        //     }
        //     return (T)stat;
        // }

        // public void ChangeEachType(UniqueStatCollection collection)
        // {
        //     switch collection.
        // }
    }
}

