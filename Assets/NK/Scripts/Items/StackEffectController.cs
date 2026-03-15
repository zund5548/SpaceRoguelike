using UnityEngine;
using Ships;
using System.Collections.Generic;
using UniRx.Triggers;
using UniRx;
using Managers;
using Stats;
using System;
namespace Items
{
    public class StackEffectController
    {
        public Dictionary<Type,StackEffect> stackDic = new();
        public T GetStackEffect<T>() where T : StackEffect
        {
            stackDic.TryGetValue(typeof(T), out var status);
            return status as T;
        }
        public T AddStackEffect<T>(Ship ownerShip) where T : StackEffect, new()
        {
            var type = typeof(T);
            if (!stackDic.TryGetValue(type, out var status))
            {
                status = new T();
                status.ownerShip = ownerShip;
                stackDic.Add(type, status);
            }
            return (T)status;
        }
        public void AddStackNum<T>(Ship ownerShip,int value) where T : StackEffect, new()
        {
            var status = AddStackEffect<T>(ownerShip);
            status.AddStack(value);
            status.OnStackChanged();
        }
    }
}

