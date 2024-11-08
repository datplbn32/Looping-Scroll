using System;
using System.Collections.Generic;
using UnityEngine;
using Object = System.Object;

namespace DatdevUlts
{
    [Serializable]
    public class PoolObjectManager<T>
    {
        private List<T> _stack = new List<T>();

        public T GetObject(Func<T> funcNewItem)
        {
            if (_stack.Count == 0)
            {
                _stack.Add(funcNewItem.Invoke());
            }

            T item = _stack[0];
            _stack.RemoveAll(obj => obj.Equals(item));

            return item;
        }

        private void Release(T poolObject)
        {
            _stack.Add(poolObject);
        }
    }
}