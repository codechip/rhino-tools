﻿#region license

// Copyright (c) 2005 - 2007 Ayende Rahien (ayende@ayende.com)
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//     * Neither the name of Ayende Rahien nor the names of its
//     contributors may be used to endorse or promote products derived from this
//     software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections;

namespace Rhino.Commons
{
	public static class Collection
	{
		public static IList<T> Containing<T>(params T[] items)
		{
			if (items == null) return null;
			return new List<T>(items);
		}

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
				if (pred(item))
					return item;
			}
			return default(T);
		}

		private static ICollection<T> SelectInternal<T>(bool addIfTrue, ICollection<T> collection, Predicate<T> predicate)
		{
			ICollection<T> results = new List<T>();
			foreach (T item in collection)
			{
				if (predicate(item))
				{
					if (addIfTrue)
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

		public static T[] ToArray<T>(object list)
		{
			return ToArray<T>((IList)list);
		}

		public static T[] ToArray<T>(IEnumerable list)
		{
			List<T> items = new List<T>();
			foreach (T item in list)
			{
				items.Add(item);
			}
			return items.ToArray();
		}

		public static BindingList<T> ToBindingList<T>(IEnumerable list)
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

		public static object Single(ICollection collection)
		{
			if (collection.Count == 0)
				return null;
			if (collection.Count > 1)
				throw new InvalidOperationException("Collection does not have exactly one item");
			IEnumerator enumerator = collection.GetEnumerator();
			enumerator.MoveNext();
			return enumerator.Current;
		}

		public static ICollection<T> ToUniqueCollection<T>(IEnumerable<T> collection)
		{
			return ToUniqueCollection<T>((IEnumerable) collection);
		}

		public static ICollection<T> ToUniqueCollection<T>(IEnumerable collection)
		{
			if (collection == null) return null;

			List<T> result = new List<T>();
			foreach (T item in collection)
			{
				if (!result.Contains(item))
					result.Add(item);
			}
			return result;
		}

		public static IList<TResult> Map<TSource, TResult>(
			IEnumerable<TSource> source,
			Converter<TSource, TResult> converter)
		{
			List<TResult> result = new List<TResult>();
			if (source != null)
			{
				foreach (TSource item in source)
					result.Add(converter(item));
			}
			return result;
		}

		public delegate TResult Accumelator<TSource, TResult>(TSource source, TResult result);

		public static TResult Reduce<TSource, TResult>(
			IEnumerable<TSource> source, TResult startValue,
			Accumelator<TSource, TResult> accumulator)
		{
			TResult result = startValue;
			if (source != null)
			{
				foreach (TSource item in source)
					result = accumulator(item, result);
			}
			return result;
		}
	}
}