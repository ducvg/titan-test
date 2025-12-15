using System.Collections.Generic;
using UnityEngine;

namespace DVG.Common.ObjectPool
{
    public sealed class UnityPool<T> : IObjectPool<T> where T : Component
    {
        private T prefab;
        private Stack<T> inactiveStack;
        private int maxSize;

        public void Init(T prefab, int defaultCapacity, int maxSize)
        {
            this.prefab = prefab;
            inactiveStack = new(defaultCapacity);
            this.maxSize = maxSize;
        }

        public void Preload(int amount, Transform parent = null)
        {
            for(int i=0; i<amount; i++)
            {
                T instance = Create();
                if(parent) instance.transform.SetParent(parent);
                instance.gameObject.SetActive(false);
                inactiveStack.Push(instance);
            }
        }

        private T Create()
        {
            return Object.Instantiate(prefab);
        }

        public T Get()
        {
            T instance;
            
            if (inactiveStack.Count == 0) instance = Create();
            else instance = inactiveStack.Pop();
            instance.gameObject.SetActive(true);
            return instance;
        }

        public void Release(T instance)
        {
            if(inactiveStack.Count > maxSize)
            {
                Object.Destroy(instance);
                return;
            }
            instance.gameObject.SetActive(false);
            inactiveStack.Push(instance);
        }

        public void Clear()
        {
            while(inactiveStack.Count > 0)
            {
                Object.Destroy(inactiveStack.Pop());
            }
            inactiveStack.Clear();
        }
    }

    public interface IObjectPool<T>
    {
        public void Init(T prefab, int defaultCapacity, int maxSize);
        public void Preload(int amount, Transform parent = null);
        public T Get();
        public void Release(T instance);
        public void Clear();
    }
}

