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
			throw new NotImplementedException();
		}

		public int Count
		{
			get { throw new NotImplementedException(); }
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
