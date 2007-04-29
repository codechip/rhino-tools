#region license
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
using System.Collections;

namespace Ayende.NHibernateQueryAnalyzer.ProjectLoader
{
	public class RemoteDictionary : RemoteObject, IDictionary , IEnumerable 
	{
		public ICollection Keys
		{
			get {throw new InvalidOperationException("Operation not allowed with Remote List");}
		}

		public ICollection Values
		{
			get { throw new InvalidOperationException("Operation not allowed with Remote List"); }
		}

		public bool IsReadOnly
		{
			get { return Dictionary.IsReadOnly; }
		}

		public bool IsFixedSize
		{
			get { return Dictionary.IsFixedSize; }
		}

		public bool Contains(object key)
		{
			throw new InvalidOperationException("Operation not allowed with Remote List");
		}

		public void Add(object key, object value)
		{
			throw new InvalidOperationException("Operation not allowed with Remote List");
		}

		public void Clear()
		{
			throw new InvalidOperationException("Operation not allowed with Remote List");
		}

		public IDictionaryEnumerator GetEnumerator()
		{
			return new RemoteDictionaryEnumerator(Dictionary.GetEnumerator());
		}

		public void Remove(object key)
		{
			throw new InvalidOperationException("Operation not allowed with Remote List");
		}

		public object this[object key]
		{
			get { return RemoteObject.Create(Dictionary[key]); }
			set {throw new InvalidOperationException("Operation not allowed with Remote List");}
		}

		public RemoteDictionary(IDictionary dic) : base(dic)
		{
		}

		internal IDictionary Dictionary
		{
			get{ return (IDictionary)obj;}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public void CopyTo(Array array, int index)
		{
		}

		public int Count
		{
			get { return Dictionary.Count; }
		}

		public object SyncRoot
		{
			get { return Dictionary.SyncRoot; }
		}

		public bool IsSynchronized
		{
			get { return Dictionary.IsSynchronized; }
		}
	}

	internal class RemoteDictionaryEnumerator : MarshalByRefObject, IDictionaryEnumerator
	{
		private readonly IDictionaryEnumerator enumerator;

		public RemoteDictionaryEnumerator(IDictionaryEnumerator enumerator)
		{
			this.enumerator = enumerator;

		}

		public bool MoveNext()
		{
			return enumerator.MoveNext();
		}

		public void Reset()
		{
			enumerator.Reset();
		}

		public object Current
		{
			get 
			{ 
				return Entry;
			}
		}

		public object Key
		{
			get { return RemoteObject.Create(enumerator.Key); }
		}

		public object Value
		{
			get { return RemoteObject.Create(enumerator.Value); }
		}

		public DictionaryEntry Entry
		{
			get { return new DictionaryEntry(Key,Value); }
		}
	}
}
