using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Collections;

namespace Rhino.Commons
{
    public static class Collection
    {
        public static T First<T>(ICollection<T> collection)
        {
            IList<T> list = collection as IList<T>;
            if (list != null)
                return list[0];
            foreach (T item in collection)
            {
                return item;
            }
            throw new ElementNotfoundException();
        }
        
        public static T Last<T>(ICollection<T> collection)
        {
            IList<T> list = collection as IList<T>;
            if (list != null)
                return list[list.Count - 1];
            T last = default(T);
            bool set = false;
            foreach (T item in collection)
            {
                last = item;
                set = true;
            }
            if (set)
                return last;
            throw new ElementNotfoundException();
        }

        public static ICollection<T> SelectAll<T>(ICollection<T> collection, Predicate<T> predicate)
        {
            return SelectInternal(true, collection, predicate);
        }

        public static T Find<T>(ICollection<T> items, Predicate<T> pred)
        {
            foreach (T item in items)
            {
                if(pred(item))
                    return item;
            }
            return default(T);
        }
        
        private static ICollection<T> SelectInternal<T>(bool addIfTrue, ICollection<T> collection, Predicate<T> predicate)
        {
            ICollection<T> results = new List<T>();
            foreach (T item in collection)
            {
                if(predicate(item))
                {
                    if(addIfTrue)
                        results.Add(item);
                }
                else if (addIfTrue == false)
                {
                    results.Add(item);
                }
            }
            return results;
        }

        public static ICollection<T> SelectAllNot<T>(ICollection<T> collection, Predicate<T> predicate)
        {
            return SelectInternal(false, collection, predicate);
        }
        
        public static void ForEach<T>(ICollection<T> collection, Action<T> action)
        {
            foreach (T item in collection)
            {
                action(item);
            }
        }
        
        public static T[] ToArray<T>(IList list)
        {
            T[] arr = new T[list.Count];
            list.CopyTo(arr, 0);
            return arr;
        }

        public static BindingList<T> ToBindingList<T>(IList list)
        {
            return new BindingList<T>(ToArray<T>(list));
        }

		public static IDictionary<T, ICollection<K>> GroupBy<K, T>(ICollection<K> collection, Converter<K, T> converter)
		{
			Dictionary<T, ICollection<K>> dic = new Dictionary<T, ICollection<K>>();
			foreach (K k in collection)
			{
				T key = converter(k);
				if (dic.ContainsKey(key) == false)
				{
					dic[key] = new List<K>();
				}
				dic[key].Add(k);
			}
			return dic;
		}
    }
}
