using System;
using System.Collections.Generic;

namespace WorldStrategy
{
    public class UniqueList<T> : IEnumerable<T>
    {
        private HashSet<T> hashSet;
        private List<T> list;

        public UniqueList()
        {
            hashSet = new HashSet<T>();
            list = new List<T>();
        }

        public int Count
        {
            get
            {
                return hashSet.Count;
            }
        }

        public void Clear()
        {
            hashSet.Clear();
            list.Clear();
        }

        public bool Contains(T item)
        {
            return hashSet.Contains(item);
        }

        public void Add(T item)
        {
            if (hashSet.Add(item))
            {
                list.Add(item);
            }
        }

        public void Remove(T item)
        {
            list.Remove(item);
            hashSet.Remove(item);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return list.GetEnumerator();
        }
    }
}