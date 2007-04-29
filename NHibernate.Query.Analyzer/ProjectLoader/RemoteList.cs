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
	
	public class RemoteList : RemoteObject, IList, IEnumerable
	{
		public bool IsReadOnly
		{
			get { return List.IsReadOnly; }
		}

		public bool IsFixedSize
		{
			get { return List.IsFixedSize; }
		}

		public int Add(object value)
		{
			throw new InvalidOperationException("Operation not allowed with Remote List");
		}

		public bool Contains(object value)
		{
			throw new InvalidOperationException("Operation not allowed with Remote List");
		}

		public void Clear()
		{
			throw new InvalidOperationException("Operation not allowed with Remote List");
		}

		public int IndexOf(object value)
		{
			throw new InvalidOperationException("Operation not allowed with Remote List");
		}

		public void Insert(int index, object value)
		{
			throw new InvalidOperationException("Operation not allowed with Remote List");
		}

		public void Remove(object value)
		{
			throw new InvalidOperationException("Operation not allowed with Remote List");
		}

		public void RemoveAt(int index)
		{
			throw new InvalidOperationException("Operation not allowed with Remote List");
		}

		public object this[int index]
		{
			get { return RemoteObject.Create(List[index]); }
			set { throw new InvalidOperationException("Operation not allowed with Remote List"); }
		}

		public RemoteList(IList list) : base(list)
		{
		}

		internal IList List
		{
			get { return (IList)obj;}
		}

		public IEnumerator GetEnumerator()
		{
			return new RemoteListEnumerator(List.GetEnumerator());
		}

		public void CopyTo(Array array, int index)
		{
			throw new NotImplementedException();
		}

		public int Count
		{
			get { return List.Count; }
		}

		public object SyncRoot
		{
			get { throw new NotImplementedException(); }
		}

		public bool IsSynchronized
		{
			get { throw new NotImplementedException(); }
		}

	}

	internal class RemoteListEnumerator : MarshalByRefObject, IEnumerator
	{
		private readonly IEnumerator enumerator;

		public RemoteListEnumerator(IEnumerator enumerator)
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
			get { return RemoteObject.Create(enumerator.Current); }
		}
	}
}
