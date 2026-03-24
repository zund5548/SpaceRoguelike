using UnityEngine;
using Ships;
using System;
using System.Collections.Generic;
namespace Stats
{
    public class UniqueStatController
    {
        public Dictionary<Type,UniqueStatSet> statDic = new();
        //statの情報を読む
        public T GetUniqueStat<T>() where T : UniqueStatSet
        {
            statDic.TryGetValue(typeof(T), out var stat);
            return stat as T;
        }
        //新しいstatを作る
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
    }
}

