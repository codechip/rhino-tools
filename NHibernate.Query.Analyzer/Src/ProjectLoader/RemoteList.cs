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
