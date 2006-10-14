using System;
using System.Collections;

namespace Ayende.NHibernateQueryAnalyzer.SchemaEditing
{
	#region Interface INodeFieldReferenceEnumerator

	/// <summary>
	/// Supports type-safe iteration over a collection that contains 
	/// <see cref="NodeFieldReference"/> elements.
	/// </summary>
	/// <remarks>
	/// <b>INodeFieldReferenceEnumerator</b> 
	/// provides an <see cref="IEnumerator"/> that is strongly typed for 
	/// <see cref="NodeFieldReference"/> elements.
	/// </remarks>
	public interface INodeFieldReferenceEnumerator
	{
		#region Properties

		#region Current

		/// <summary>
		/// Gets the current <see cref="NodeFieldReference"/> 
		/// element in the collection.
		/// </summary>
		/// <value>
		/// The current <see cref="NodeFieldReference"/> 
		/// element in the collection.</value>
		/// <exception cref="InvalidOperationException"><para>
		/// The enumerator is positioned before the first element 
		/// of the collection or after the last element.
		/// </para><para>-or-</para><para>
		/// The collection was modified after the enumerator was created.
		/// </para></exception>
		/// <remarks>
		/// Please refer to <see cref="IEnumerator.Current"/> for details, 
		/// but note that <b>Current</b> fails if the collection was modified 
		/// since the last successful call to <see cref="MoveNext"/> or 
		/// <see cref="Reset"/>.
		/// </remarks>
		NodeFieldReference Current { get; }

		#endregion

		#endregion

		#region Methods

		#region MoveNext

		/// <summary>
		/// Advances the enumerator to the next element of the collection.
		/// </summary>
		/// <returns>
		/// <c>true</c> if the enumerator was successfully advanced 
		/// to the next element; <c>false</c> if the enumerator has 
		/// passed the end of the collection.</returns>
		/// <exception cref="InvalidOperationException">
		/// The collection was modified after the enumerator was created.
		/// </exception>
		/// <remarks>
		/// Please refer to <see cref="IEnumerator.MoveNext"/> for details.
		/// </remarks>
		bool MoveNext();

		#endregion

		#region Reset

		/// <summary>
		/// Sets the enumerator to its initial position,
		/// which is before the first element in the collection.
		/// </summary>
		/// <exception cref="InvalidOperationException">
		/// The collection was modified after the enumerator was created.
		/// </exception>
		/// <remarks>
		/// Please refer to <see cref="IEnumerator.Reset"/> for details.
		/// </remarks>
		void Reset();

		#endregion

		#endregion
	}

	#endregion

	#region Class NodeFieldReferenceCollection

	/// <summary>
	/// Implements a strongly typed collection
	/// of <see cref="NodeFieldReference"/> elements.
	/// </summary>
	/// <remarks><para>
	/// <b>NodeFieldReferenceCollection</b>
	/// provides an <see cref="ArrayList"/> that is strongly typed for
	/// <see cref="NodeFieldReference"/> elements.
	/// </para><para>
	/// The <see cref="NodeFieldReference.Names"/> property
	/// of the <see cref="NodeFieldReference"/> class can be used as a key
	/// to locate elements in the <b>NodeFieldReferenceCollection</b>.
	/// </para><para>
	/// The collection may contain multiple identical keys. All key access
	/// methods return the first occurrence of the specified key, if found.
	/// Access by key is an O(<em>N</em>) operation, where <em>N</em> is the
	/// current value of the <see cref="NodeFieldReferenceCollection.Count"/> property.
	/// </para></remarks>
	[Serializable]
	public class NodeFieldReferenceCollection : IList, ICloneable
	{
		private readonly Data _data;

		#region Private Constructor

		/// <summary>
		/// Initializes a new instance of the
		/// <see cref="NodeFieldReferenceCollection"/> class
		/// with the specified data container.
		/// </summary>
		/// <param name="data">
		/// The <see cref="Data"/> object to share with another instance.
		/// </param>
		/// <remarks>
		/// This constructor is used to create read-only wrappers.
		/// </remarks>
		private NodeFieldReferenceCollection(Data data)
		{
			_data = data;
		}

		#endregion

		#region Public Constructors

		#region NodeFieldReferenceCollection()

		/// <overloads>
		/// Initializes a new instance of the
		/// <see cref="NodeFieldReferenceCollection"/> class.
		/// </overloads>
		/// <summary>
		/// Initializes a new instance of the
		/// <see cref="NodeFieldReferenceCollection"/> class
		/// that is empty and has the default initial capacity.
		/// </summary>
		/// <remarks>
		/// Please refer to <see cref="ArrayList()"/> for details.
		/// </remarks>
		public NodeFieldReferenceCollection()
		{
			_data = new Data();
			_data.Items = new NodeFieldReference[Data.DefaultCapacity];
		}

		#endregion

		#region NodeFieldReferenceCollection(Int32)

		/// <summary>
		/// Initializes a new instance of the
		/// <see cref="NodeFieldReferenceCollection"/> class
		/// that is empty and has the specified initial capacity.
		/// </summary>
		/// <param name="capacity">
		/// The initial number of elements that the new
		/// <see cref="NodeFieldReferenceCollection"/> can contain.</param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="capacity"/> is less than zero.</exception>
		/// <remarks>
		/// Please refer to <see cref="ArrayList(Int32)"/> for details.
		/// </remarks>
		public NodeFieldReferenceCollection(int capacity)
		{
			if (capacity < 0)
				throw new ArgumentOutOfRangeException("capacity", capacity, "Argument cannot be negative.");

			_data = new Data();
			_data.Items = new NodeFieldReference[capacity];
		}

		#endregion

		#region NodeFieldReferenceCollection(NodeFieldReferenceCollection)

		/// <summary>
		/// Initializes a new instance of the
		/// <see cref="NodeFieldReferenceCollection"/> class that
		/// contains elements copied from the specified collection and that
		/// has the same initial capacity as the number of elements copied.
		/// </summary>
		/// <param name="collection">
		/// The <see cref="NodeFieldReferenceCollection"/>
		/// whose elements are copied to the new collection.</param>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="collection"/> is a null reference.</exception>
		/// <remarks>
		/// Please refer to <see cref="ArrayList(ICollection)"/> for details.
		/// </remarks>
		public NodeFieldReferenceCollection(NodeFieldReferenceCollection collection)
		{
			if (collection == null)
				throw new ArgumentNullException("collection");

			_data = new Data();
			_data.Items = new NodeFieldReference[collection.Count];
			AddRange(collection._data.Items, collection.Count);
		}

		#endregion

		#region NodeFieldReferenceCollection(NodeFieldReference[])

		/// <summary>
		/// Initializes a new instance of the
		/// <see cref="NodeFieldReferenceCollection"/> class
		/// that contains elements copied from the specified
		/// <see cref="NodeFieldReference"/> array and that has the
		/// same initial capacity as the number of elements copied.
		/// </summary>
		/// <param name="array">
		/// An <see cref="Array"/> of <see cref="NodeFieldReference"/>
		/// elements that are copied to the new collection.</param>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="array"/> is a null reference.</exception>
		/// <remarks>
		/// Please refer to <see cref="ArrayList(ICollection)"/> for details.
		/// </remarks>
		public NodeFieldReferenceCollection(NodeFieldReference[] array)
		{
			if (array == null)
				throw new ArgumentNullException("array");

			_data = new Data();
			_data.Items = new NodeFieldReference[array.Length];
			AddRange(array, array.Length);
		}

		#endregion

		#endregion

		#region Public Properties

		#region Capacity

		/// <summary>
		/// Gets or sets the capacity of the <see cref="NodeFieldReferenceCollection"/>.
		/// </summary>
		/// <value>
		/// The number of elements that the
		/// <see cref="NodeFieldReferenceCollection"/> can contain.
		/// </value>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <b>Capacity</b> is set to a value that is
		/// less than <see cref="Count"/>.</exception>
		/// <exception cref="NotSupportedException">
		/// The property is set, and the <see cref="NodeFieldReferenceCollection"/> 
		/// is read-only or has a fixed size.</exception>
		/// <remarks>
		/// Please refer to <see cref="ArrayList.Capacity"/> for details.
		/// </remarks>
		public virtual int Capacity
		{
			get { return _data.Items.Length; }
			set
			{
				if (value < _data.Count)
					throw new ArgumentOutOfRangeException("Capacity", value, "Value cannot be less than Count.");

				_data.SetCapacity(value);
			}
		}

		#endregion

		#region Count

		/// <summary>
		/// Gets the number of elements contained in the
		/// <see cref="NodeFieldReferenceCollection"/>.
		/// </summary>
		/// <value>
		/// The number of elements contained in the
		/// <see cref="NodeFieldReferenceCollection"/>.
		/// </value>
		/// <remarks>
		/// Please refer to <see cref="ArrayList.Count"/> for details.
		/// </remarks>
		public int Count
		{
			get { return _data.Count; }
		}

		#endregion

		#region IsFixedSize

		/// <summary>
		/// Gets a value indicating whether the
		/// <see cref="NodeFieldReferenceCollection"/> has a fixed size.
		/// </summary>
		/// <value>
		/// <c>true</c> if the <see cref="NodeFieldReferenceCollection"/>
		/// has a fixed size; otherwise, <c>false</c>.
		/// The default is <c>false</c>.</value>
		/// <remarks>
		/// Please refer to <see cref="ArrayList.IsFixedSize"/> for details.
		/// </remarks>
		public virtual bool IsFixedSize
		{
			get { return false; }
		}

		#endregion

		#region IsReadOnly

		/// <summary>
		/// Gets a value indicating whether the
		/// <see cref="NodeFieldReferenceCollection"/> is read-only.
		/// </summary>
		/// <value>
		/// <c>true</c> if the <see cref="NodeFieldReferenceCollection"/>
		/// is read-only; otherwise, <c>false</c>.
		/// The default is <c>false</c>.</value>
		/// <remarks>
		/// Please refer to <see cref="ArrayList.IsReadOnly"/> for details.
		/// </remarks>
		public virtual bool IsReadOnly
		{
			get { return false; }
		}

		#endregion

		#region IsSynchronized

		/// <summary>
		/// Gets a value indicating whether access to the
		/// <see cref="NodeFieldReferenceCollection"/> is synchronized (thread-safe).
		/// </summary>
		/// <value>
		/// <c>true</c> if access to the <see cref="NodeFieldReferenceCollection"/>
		/// is synchronized (thread-safe); otherwise, <c>false</c>.
		/// The default is <c>false</c>.</value>
		/// <remarks>
		/// Please refer to <see cref="ArrayList.IsSynchronized"/> for details.
		/// </remarks>
		public bool IsSynchronized
		{
			get { return false; }
		}

		#endregion

		#region IsUnique

		/// <summary>
		/// Gets or sets a value indicating whether the
		/// <see cref="NodeFieldReferenceCollection"/>
		/// ensures that all elements are unique.
		/// </summary>
		/// <value>
		/// <c>true</c> if the <see cref="NodeFieldReferenceCollection"/>
		/// ensures that all elements are unique; otherwise,
		/// <c>false</c>. The default is <c>false</c>.</value>
		/// <exception cref="InvalidOperationException">
		/// The property is set to <c>true</c>, and the
		/// <see cref="NodeFieldReferenceCollection"/>
		/// already contains duplicate elements.</exception>
		/// <exception cref="NotSupportedException">
		/// The property is set, and the
		/// <see cref="NodeFieldReferenceCollection"/> is read-only.</exception>
		/// <remarks><para>
		/// <b>IsUnique</b> provides a set-like collection by ensuring that
		/// all elements in the <see cref="NodeFieldReferenceCollection"/> 
		/// are unique.
		/// </para><para>
		/// When changed to <c>true</c>, this property throws an 
		/// <see cref="InvalidOperationException"/> if the 
		/// <b>NodeFieldReferenceCollection</b> already contains duplicate
		/// elements. Any subsequent attempt to add an element that is
		/// already contained in the <b>NodeFieldReferenceCollection</b> 
		/// will cause a <see cref="NotSupportedException"/>.
		/// </para></remarks>
		public virtual bool IsUnique
		{
			get { return _data.IsUnique; }
			set
			{
				if (value == _data.IsUnique) return;
				if (value) CheckUnique();
				_data.IsUnique = value;
			}
		}

		#endregion

		#region Item[string]: NodeFieldReference

		/// <overloads>
		/// Gets or sets a specific <see cref="NodeFieldReference"/> element.
		/// </overloads>
		/// <summary>
		/// Gets the <see cref="NodeFieldReference"/> element
		/// associated with the first occurrence of the specified
		/// <see cref="NodeFieldReference.Type"/> value.
		/// </summary>
		/// <param name="key">
		/// The <see cref="NodeFieldReference.Name"/>
		/// value whose element to get.
		/// This argument may be a null reference.
		/// </param>
		/// <value>
		/// The <see cref="NodeFieldReference"/> element associated with the
		/// first occurrence of <paramref name="key"/>, if found; otherwise,
		/// a null reference.
		/// </value>
		/// <remarks>
		/// This indexer has the same effect as the
		/// <see cref="GetByKey"/> method.
		/// </remarks>
		public NodeFieldReference this[Type key]
		{
			get { return GetByKey(key); }
		}

		#endregion

		#region Item: NodeFieldReference

		/// <summary>
		/// Gets or sets the <see cref="NodeFieldReference"/>
		/// element at the specified index.
		/// </summary>
		/// <param name="index">
		/// The zero-based index of the <see cref="NodeFieldReference"/>
		/// element to get or set.</param>
		/// <value>
		/// The <see cref="NodeFieldReference"/> element
		/// at the specified <paramref name="index"/>.
		/// </value>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <para><paramref name="index"/> is less than zero.</para>
		/// <para>-or-</para>
		/// <para><paramref name="index"/> is equal to or
		/// greater than <see cref="Count"/>.</para></exception>
		/// <exception cref="NotSupportedException"><para>
		/// The property is set, and the
		/// <see cref="NodeFieldReferenceCollection"/> is read-only.
		/// </para><para>-or-</para><para>
		/// The property is set, the <b>NodeFieldReferenceCollection</b>
		/// already contains the specified element at a different index,
		/// and the <b>NodeFieldReferenceCollection</b> ensures
		/// that all elements are unique.</para></exception>
		/// <remarks>
		/// Please refer to <see cref="ArrayList.this"/> for details.
		/// </remarks>
		public virtual NodeFieldReference this[int index]
		{
			get
			{
				if (index < 0)
					throw new ArgumentOutOfRangeException("index", index, "Argument cannot be negative.");

				if (index >= _data.Count)
					throw new ArgumentOutOfRangeException("index", index, "Argument must be less than Count.");

				return _data.Items[index];
			}
			set
			{
				if (index < 0)
					throw new ArgumentOutOfRangeException("index", index, "Argument cannot be negative.");

				if (index >= _data.Count)
					throw new ArgumentOutOfRangeException("index", index, "Argument must be less than Count.");

				if (_data.IsUnique) CheckUnique(value, index);

				_data.Items[index] = value;
			}
		}

		#endregion

		#region IList.Item: Object

		/// <summary>
		/// Gets or sets the element at the specified index.
		/// </summary>
		/// <param name="index">
		/// The zero-based index of the element to get or set.</param>
		/// <value>
		/// The element at the specified <paramref name="index"/>.
		/// When the property is set, this value must be compatible
		/// with <see cref="NodeFieldReference"/>.
		/// </value>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <para><paramref name="index"/> is less than zero.</para>
		/// <para>-or-</para>
		/// <para><paramref name="index"/> is equal to
		/// or greater than <see cref="Count"/>.</para></exception>
		/// <exception cref="InvalidCastException">
		/// The property is set to a value that is not compatible
		/// with <see cref="NodeFieldReference"/>.</exception>
		/// <exception cref="NotSupportedException"><para>
		/// The property is set, and the
		/// <see cref="NodeFieldReferenceCollection"/> is read-only.
		/// </para><para>-or-</para><para>
		/// The property is set, the <b>NodeFieldReferenceCollection</b>
		/// already contains the specified element at a different index,
		/// and the <b>NodeFieldReferenceCollection</b> ensures
		/// that all elements are unique.</para></exception>
		/// <remarks>
		/// Please refer to <see cref="ArrayList.this"/> for details.
		/// </remarks>
		object IList.this[int index]
		{
			get { return this[index]; }
			set { this[index] = (NodeFieldReference) value; }
		}

		#endregion

		#region SyncRoot

		/// <summary>
		/// Gets an object that can be used to synchronize access to the
		/// <see cref="NodeFieldReferenceCollection"/>.
		/// </summary>
		/// <value>
		/// An object that can be used to synchronize access to the
		/// <see cref="NodeFieldReferenceCollection"/>.
		/// </value>
		/// <remarks>
		/// Please refer to <see cref="ArrayList.SyncRoot"/> for details.
		/// </remarks>
		public object SyncRoot
		{
			get { return _data; }
		}

		#endregion

		#endregion

		#region Public Methods

		#region Add(NodeFieldReference)

		/// <summary>
		/// Adds a <see cref="NodeFieldReference"/> to the end
		/// of the <see cref="NodeFieldReferenceCollection"/>.
		/// </summary>
		/// <param name="value">
		/// The <see cref="NodeFieldReference"/> object to be added
		/// to the end of the <see cref="NodeFieldReferenceCollection"/>.
		/// This argument may be a null reference.
		/// </param>
		/// <returns>
		/// The <see cref="NodeFieldReferenceCollection"/>
		/// index at which the <paramref name="value"/> has been added.
		/// </returns>
		/// <exception cref="NotSupportedException"><para>
		/// The <see cref="NodeFieldReferenceCollection"/> 
		/// is read-only or has a fixed size.
		/// </para><para>-or-</para><para>
		/// The <b>NodeFieldReferenceCollection</b>
		/// already contains <paramref name="value"/>,
		/// and the <b>NodeFieldReferenceCollection</b>
		/// ensures that all elements are unique.</para></exception>
		/// <remarks>
		/// Please refer to <see cref="ArrayList.Add"/> for details.
		/// </remarks>
		public virtual int Add(NodeFieldReference value)
		{
			if (_data.IsUnique) CheckUnique(value);

			int count = _data.Count;
			if (count == _data.Items.Length)
				_data.EnsureCapacity(count + 1);

			_data.Items[count] = value;
			return _data.Count++;
		}

		#endregion

		#region IList.Add(Object)

		/// <summary>
		/// Adds an <see cref="Object"/> to the end of the
		/// <see cref="NodeFieldReferenceCollection"/>.
		/// </summary>
		/// <param name="value">
		/// The object to be added to the end of the
		/// <see cref="NodeFieldReferenceCollection"/>. This argument
		/// must be compatible with <see cref="NodeFieldReference"/>.
		/// This argument may be a null reference.
		/// </param>
		/// <returns>
		/// The <see cref="NodeFieldReferenceCollection"/>
		/// index at which the <paramref name="value"/> has been added.
		/// </returns>
		/// <exception cref="InvalidCastException">
		/// <paramref name="value"/> is not compatible with
		/// <see cref="NodeFieldReference"/>.</exception>
		/// <exception cref="NotSupportedException"><para>
		/// The <see cref="NodeFieldReferenceCollection"/> 
		/// is read-only or has a fixed size.
		/// </para><para>-or-</para><para>
		/// The <b>NodeFieldReferenceCollection</b>
		/// already contains <paramref name="value"/>,
		/// and the <b>NodeFieldReferenceCollection</b>
		/// ensures that all elements are unique.
		/// </para></exception>
		/// <remarks>
		/// Please refer to <see cref="ArrayList.Add"/> for details.
		/// </remarks>
		int IList.Add(object value)
		{
			return Add((NodeFieldReference) value);
		}

		#endregion

		#region AddRange(NodeFieldReferenceCollection)

		/// <overloads>
		/// Adds a range of elements to the end of the
		/// <see cref="NodeFieldReferenceCollection"/>.
		/// </overloads>
		/// <summary>
		/// Adds the elements of another collection to the end of the
		/// <see cref="NodeFieldReferenceCollection"/>.
		/// </summary>
		/// <param name="collection">
		/// The <see cref="NodeFieldReferenceCollection"/> whose elements
		/// should be added to the end of the current collection.</param>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="collection"/> is a null reference.</exception>
		/// <exception cref="NotSupportedException"><para>
		/// The <see cref="NodeFieldReferenceCollection"/> 
		/// is read-only or has a fixed size.
		/// </para><para>-or-</para><para>
		/// The <b>NodeFieldReferenceCollection</b> already contains one
		/// or more elements in <paramref name="collection"/>,
		/// and the <b>NodeFieldReferenceCollection</b>
		/// ensures that all elements are unique.
		/// </para></exception>
		/// <remarks>
		/// Please refer to <see cref="ArrayList.AddRange"/> for details.
		/// </remarks>
		public virtual void AddRange(NodeFieldReferenceCollection collection)
		{
			if (collection == null)
				throw new ArgumentNullException("collection");

			AddRange(collection._data.Items, collection.Count);
		}

		#endregion

		#region AddRange(NodeFieldReference[])

		/// <summary>
		/// Adds the elements of a <see cref="NodeFieldReference"/> array
		/// to the end of the <see cref="NodeFieldReferenceCollection"/>.
		/// </summary>
		/// <param name="array">
		/// An <see cref="Array"/> of <see cref="NodeFieldReference"/>
		/// elements that should be added to the end of the
		/// <see cref="NodeFieldReferenceCollection"/>.</param>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="array"/> is a null reference.</exception>
		/// <exception cref="NotSupportedException"><para>
		/// The <see cref="NodeFieldReferenceCollection"/> 
		/// is read-only or has a fixed size.
		/// </para><para>-or-</para><para>
		/// The <b>NodeFieldReferenceCollection</b> already contains
		/// one or more elements in <paramref name="array"/>,
		/// and the <b>NodeFieldReferenceCollection</b>
		/// ensures that all elements are unique.
		/// </para></exception>
		/// <remarks>
		/// Please refer to <see cref="ArrayList.AddRange"/> for details.
		/// </remarks>
		public virtual void AddRange(NodeFieldReference[] array)
		{
			if (array == null)
				throw new ArgumentNullException("array");

			AddRange(array, array.Length);
		}

		#endregion

		#region BinarySearch(NodeFieldReference, IComparer)

		/// <overloads>
		/// Uses a binary search algorithm to locate a specific element
		/// in the sorted <see cref="NodeFieldReferenceCollection"/>
		/// or a portion of it.
		/// </overloads>
		/// <summary>
		/// Searches the entire sorted <see cref="NodeFieldReferenceCollection"/>
		/// for an <see cref="NodeFieldReference"/> element using the
		/// specified comparer and returns the zero-based index of the element.
		/// </summary>
		/// <param name="value">
		/// The <see cref="NodeFieldReference"/> object to locate
		/// in the <see cref="NodeFieldReferenceCollection"/>.
		/// This argument may be a null reference.
		/// </param>
		/// <param name="comparer">
		/// <para>The <see cref="IComparer"/> implementation
		/// to use when comparing elements.</para>
		/// <para>-or-</para>
		/// <para>A null reference to use the <see cref="IComparable"/>
		/// implementation of each element.</para></param>
		/// <returns>
		/// The zero-based index of <paramref name="value"/> in the sorted
		/// <see cref="NodeFieldReferenceCollection"/>, if <paramref name="value"/>
		/// is found; otherwise, a negative number, which is the bitwise
		/// complement of the index of the next element that is larger than
		/// <paramref name="value"/> or, if there is no larger element, the
		/// bitwise complement of <see cref="Count"/>.</returns>
		/// <exception cref="ArgumentException">
		/// <paramref name="comparer"/> is a null reference,
		/// and NodeFieldReference does not implement
		/// the <see cref="IComparable"/> interface.</exception>
		/// <remarks>
		/// Please refer to
		/// <see cref="ArrayList.BinarySearch(Object, IComparer)"/>
		/// for details.
		/// </remarks>
		public int BinarySearch(NodeFieldReference value, IComparer comparer)
		{
			return Array.BinarySearch(_data.Items, 0, _data.Count, value, comparer);
		}

		#endregion

		#region BinarySearch(Int32, Int32, NodeFieldReference, IComparer)

		/// <summary>
		/// Searches a section of the sorted
		/// <see cref="NodeFieldReferenceCollection"/> for an
		/// <see cref="NodeFieldReference"/> element using the
		/// specified comparer and returns the zero-based index of the element.
		/// </summary>
		/// <param name="index">
		/// The zero-based starting index of the range of elements to search.
		/// </param>
		/// <param name="count">The number of elements to search.</param>
		/// <param name="value">
		/// The <see cref="NodeFieldReference"/> object to locate
		/// in the <see cref="NodeFieldReferenceCollection"/>.
		/// This argument may be a null reference.
		/// </param>
		/// <param name="comparer">
		/// <para>The <see cref="IComparer"/> implementation
		/// to use when comparing elements.</para>
		/// <para>-or-</para>
		/// <para>A null reference to use the <see cref="IComparable"/>
		/// implementation of each element.</para></param>
		/// <returns>
		/// The zero-based index of <paramref name="value"/> in the sorted
		/// <see cref="NodeFieldReferenceCollection"/>, if <paramref name="value"/>
		/// is found; otherwise, a negative number, which is the bitwise
		/// complement of the index of the next element that is larger than
		/// <paramref name="value"/> or, if there is no larger element, the
		/// bitwise complement of <see cref="Count"/>.</returns>
		/// <exception cref="ArgumentException"><para>
		/// <paramref name="index"/> and <paramref name="count"/>
		/// do not denote a valid range of elements in the
		/// <see cref="NodeFieldReferenceCollection"/>.
		/// </para><para>-or-</para><para>
		/// <paramref name="comparer"/> is a null reference,
		/// and NodeFieldReference does not implement
		/// the <see cref="IComparable"/> interface.
		/// </para></exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <para><paramref name="index"/> is less than zero.</para>
		/// <para>-or-</para>
		/// <para><paramref name="count"/> is less than zero.</para>
		/// </exception>
		/// <remarks>
		/// Please refer to
		/// <see cref="ArrayList.BinarySearch(Int32, Int32, Object, IComparer)"/>
		/// for details.
		/// </remarks>
		public int BinarySearch(int index, int count, NodeFieldReference value, IComparer comparer)
		{
			if (index < 0)
				throw new ArgumentOutOfRangeException("index", index, "Argument cannot be negative.");

			if (count < 0)
				throw new ArgumentOutOfRangeException("count", count, "Argument cannot be negative.");

			if (index + count > _data.Count)
				throw new ArgumentException("Arguments denote invalid range of elements.");

			return Array.BinarySearch(_data.Items, index, _data.Count, value, comparer);
		}

		#endregion

		#region Clear

		/// <summary>
		/// Removes all elements from the <see cref="NodeFieldReferenceCollection"/>.
		/// </summary>
		/// <exception cref="NotSupportedException">
		/// The <see cref="NodeFieldReferenceCollection"/> 
		/// is read-only or has a fixed size.</exception>
		/// <remarks>
		/// Please refer to <see cref="ArrayList.Clear"/> for details.
		/// </remarks>
		public virtual void Clear()
		{
			if (_data.Count == 0) return;
			Array.Clear(_data.Items, 0, _data.Count);
			_data.Count = 0;
		}

		#endregion

		#region Clone

		/// <summary>
		/// Creates a shallow copy of the <see cref="NodeFieldReferenceCollection"/>.
		/// </summary>
		/// <returns>
		/// A shallow copy of the <see cref="NodeFieldReferenceCollection"/>.
		/// </returns>
		/// <remarks>
		/// Please refer to <see cref="ArrayList.Clone"/> for details.
		/// </remarks>
		public virtual object Clone()
		{
			NodeFieldReferenceCollection clone = new NodeFieldReferenceCollection(_data.Count);

			Array.Copy(_data.Items, 0, clone._data.Items, 0, _data.Count);
			clone._data.Count = _data.Count;
			clone._data.IsUnique = _data.IsUnique;

			return clone;
		}

		#endregion

		#region Contains(NodeFieldReference)

		/// <summary>
		/// Determines whether the <see cref="NodeFieldReferenceCollection"/>
		/// contains the specified <see cref="NodeFieldReference"/> element.
		/// </summary>
		/// <param name="value">
		/// The <see cref="NodeFieldReference"/> object to locate
		/// in the <see cref="NodeFieldReferenceCollection"/>.
		/// This argument may be a null reference.
		/// </param>
		/// <returns>
		/// <c>true</c> if <paramref name="value"/> is found in the
		/// <see cref="NodeFieldReferenceCollection"/>; otherwise, <c>false</c>.
		/// </returns>
		/// <remarks>
		/// Please refer to <see cref="ArrayList.Contains"/> for details.
		/// </remarks>
		public bool Contains(NodeFieldReference value)
		{
			return (IndexOf(value) >= 0);
		}

		#endregion

		#region IList.Contains(Object)

		/// <summary>
		/// Determines whether the <see cref="NodeFieldReferenceCollection"/>
		/// contains the specified element.
		/// </summary>
		/// <param name="value">
		/// The object to locate in the
		/// <see cref="NodeFieldReferenceCollection"/>. This argument
		/// must be compatible with <see cref="NodeFieldReference"/>.
		/// This argument may be a null reference.
		/// </param>
		/// <returns>
		/// <c>true</c> if <paramref name="value"/> is found in the
		/// <see cref="NodeFieldReferenceCollection"/>; otherwise, <c>false</c>.
		/// </returns>
		/// <exception cref="InvalidCastException">
		/// <paramref name="value"/> is not compatible with
		/// <see cref="NodeFieldReference"/>.</exception>
		/// <remarks>
		/// Please refer to <see cref="ArrayList.Contains"/> for details.
		/// </remarks>
		bool IList.Contains(object value)
		{
			return Contains((NodeFieldReference) value);
		}

		#endregion

		#region ContainsKey

		/// <summary>
		/// Determines whether the <see cref="NodeFieldReferenceCollection"/> contains
		/// the specified <see cref="NodeFieldReference.Name"/> value.
		/// </summary>
		/// <param name="key">
		/// The <see cref="NodeFieldReference.Type"/> value to locate
		/// in the <see cref="NodeFieldReferenceCollection"/>.
		/// This argument may be a null reference.
		/// </param>
		/// <returns>
		/// <c>true</c> if <paramref name="key"/> is found in the
		/// <see cref="NodeFieldReferenceCollection"/>; otherwise, <c>false</c>.
		/// </returns>
		/// <remarks>
		/// <b>ContainsKey</b> is similar to <see cref="Contains"/> but
		/// compares the specified <paramref name="key"/> to the value
		/// of the <see cref="NodeFieldReference.Name"/> property
		/// of each <see cref="NodeFieldReference"/> element, rather than
		/// to the element itself.
		/// </remarks>
		public bool ContainsKey(Type key)
		{
			return (IndexOfKey(key) >= 0);
		}

		#endregion

		#region CopyTo(NodeFieldReference[])

		/// <overloads>
		/// Copies the <see cref="NodeFieldReferenceCollection"/>
		/// or a portion of it to a one-dimensional array.
		/// </overloads>
		/// <summary>
		/// Copies the entire <see cref="NodeFieldReferenceCollection"/>
		/// to a one-dimensional <see cref="Array"/>
		/// of <see cref="NodeFieldReference"/> elements,
		/// starting at the beginning of the target array.
		/// </summary>
		/// <param name="array">
		/// The one-dimensional <see cref="Array"/> that is the destination
		/// of the <see cref="NodeFieldReference"/> elements copied from the
		/// <see cref="NodeFieldReferenceCollection"/>.
		/// The <b>Array</b> must have zero-based indexing.</param>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="array"/> is a null reference.</exception>
		/// <exception cref="ArgumentException">
		/// The number of elements in the source
		/// <see cref="NodeFieldReferenceCollection"/> is greater than
		/// the available space in the destination <paramref name="array"/>.
		/// </exception>
		/// <remarks>
		/// Please refer to <see cref="ArrayList.CopyTo"/> for details.
		/// </remarks>
		public void CopyTo(NodeFieldReference[] array)
		{
			((ICollection) this).CopyTo(array, 0);
		}

		#endregion

		#region CopyTo(NodeFieldReference[], Int32)

		/// <summary>
		/// Copies the entire <see cref="NodeFieldReferenceCollection"/>
		/// to a one-dimensional <see cref="Array"/>
		/// of <see cref="NodeFieldReference"/> elements,
		/// starting at the specified index of the target array.
		/// </summary>
		/// <param name="array">
		/// The one-dimensional <see cref="Array"/> that is the destination
		/// of the <see cref="NodeFieldReference"/> elements copied from the
		/// <see cref="NodeFieldReferenceCollection"/>.
		/// The <b>Array</b> must have zero-based indexing.</param>
		/// <param name="arrayIndex">
		/// The zero-based index in <paramref name="array"/>
		/// at which copying begins.</param>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="array"/> is a null reference.</exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="arrayIndex"/> is less than zero.</exception>
		/// <exception cref="ArgumentException"><para>
		/// <paramref name="arrayIndex"/> is equal to or
		/// greater than the length of <paramref name="array"/>.
		/// </para><para>-or-</para><para>
		/// The number of elements in the source
		/// <see cref="NodeFieldReferenceCollection"/> is greater
		/// than the available space from <paramref name="arrayIndex"/>
		/// to the end of the destination <paramref name="array"/>.
		/// </para></exception>
		/// <remarks>
		/// Please refer to <see cref="ArrayList.CopyTo"/> for details.
		/// </remarks>
		public void CopyTo(NodeFieldReference[] array, int arrayIndex)
		{
			((ICollection) this).CopyTo(array, arrayIndex);
		}

		#endregion

		#region ICollection.CopyTo(Array, Int32)

		/// <summary>
		/// Copies the entire <see cref="NodeFieldReferenceCollection"/>
		/// to a one-dimensional <see cref="Array"/>,
		/// starting at the specified index of the target array.
		/// </summary>
		/// <param name="array">
		/// The one-dimensional <see cref="Array"/> that is the destination
		/// of the <see cref="NodeFieldReference"/> elements copied from the
		/// <see cref="NodeFieldReferenceCollection"/>.
		/// The <b>Array</b> must have zero-based indexing.</param>
		/// <param name="arrayIndex">
		/// The zero-based index in <paramref name="array"/>
		/// at which copying begins.</param>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="array"/> is a null reference.</exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="arrayIndex"/> is less than zero.</exception>
		/// <exception cref="ArgumentException"><para>
		/// <paramref name="array"/> is multidimensional.
		/// </para><para>-or-</para><para>
		/// <paramref name="arrayIndex"/> is equal to or
		/// greater than the length of <paramref name="array"/>.
		/// </para><para>-or-</para><para>
		/// The number of elements in the source
		/// <see cref="NodeFieldReferenceCollection"/> is greater
		/// than the available space from <paramref name="arrayIndex"/>
		/// to the end of the destination <paramref name="array"/>.
		/// </para></exception>
		/// <exception cref="InvalidCastException">
		/// <see cref="NodeFieldReference"/>
		/// cannot be cast automatically to the type of the destination
		/// <paramref name="array"/>.</exception>
		/// <remarks>
		/// Please refer to <see cref="ArrayList.CopyTo"/> for details.
		/// </remarks>
		void ICollection.CopyTo(Array array, int arrayIndex)
		{
			_data.CheckTargetArray(array, arrayIndex);
			Array.Copy(_data.Items, 0, array, arrayIndex, _data.Count);
		}

		#endregion

		#region GetByKey

		/// <summary>
		/// Gets the <see cref="NodeFieldReference"/> element
		/// associated with the first occurrence of the specified
		/// <see cref="NodeFieldReference.Name"/> value.
		/// </summary>
		/// <param name="key">
		/// The <see cref="NodeFieldReference.Type"/>
		/// value whose element to get.
		/// This argument may be a null reference.
		/// </param>
		/// <returns>
		/// The <see cref="NodeFieldReference"/> element associated
		/// with the first occurrence of the specified
		/// <paramref name="key"/>, if found; otherwise,
		/// a null reference.
		/// </returns>
		/// <remarks>
		/// <b>GetByKey</b> compares the specified <paramref name="key"/>
		/// to the value of the <see cref="NodeFieldReference.Name"/>
		/// property of each <see cref="NodeFieldReference"/> element,
		/// and returns the first matching element.
		/// </remarks>
		public NodeFieldReference GetByKey(Type key)
		{
			int index = IndexOfKey(key);
			if (index >= 0) return _data.Items[index];
			return null;
		}

		#endregion

		#region GetEnumerator: INodeFieldReferenceEnumerator

		/// <summary>
		/// Returns an <see cref="INodeFieldReferenceEnumerator"/> that can
		/// iterate through the <see cref="NodeFieldReferenceCollection"/>.
		/// </summary>
		/// <returns>
		/// An <see cref="INodeFieldReferenceEnumerator"/> for the entire
		/// <see cref="NodeFieldReferenceCollection"/>.
		/// </returns>
		/// <remarks>
		/// Please refer to <see cref="ArrayList.GetEnumerator"/> for details.
		/// </remarks>
		public INodeFieldReferenceEnumerator GetEnumerator()
		{
			return new Enumerator(_data);
		}

		#endregion

		#region IEnumerable.GetEnumerator: IEnumerator

		/// <summary>
		/// Returns an <see cref="IEnumerator"/> that can iterate through the
		/// <see cref="NodeFieldReferenceCollection"/>.
		/// </summary>
		/// <returns>
		/// An <see cref="IEnumerator"/> for the entire
		/// <see cref="NodeFieldReferenceCollection"/>.
		/// </returns>
		/// <remarks>
		/// Please refer to <see cref="ArrayList.GetEnumerator"/> for details.
		/// </remarks>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return (IEnumerator) GetEnumerator();
		}

		#endregion

		#region IndexOf(NodeFieldReference)

		/// <summary>
		/// Returns the zero-based index of the first occurrence
		/// of the specified <see cref="NodeFieldReference"/> in the
		/// <see cref="NodeFieldReferenceCollection"/>.
		/// </summary>
		/// <param name="value">
		/// The <see cref="NodeFieldReference"/> object
		/// to locate in the <see cref="NodeFieldReferenceCollection"/>.
		/// This argument may be a null reference.
		/// </param>
		/// <returns>
		/// The zero-based index of the first occurrence of
		/// <paramref name="value"/> in the <see cref="NodeFieldReferenceCollection"/>,
		/// if found; otherwise, -1.</returns>
		/// <remarks>
		/// Please refer to <see cref="ArrayList.IndexOf"/> for details.
		/// </remarks>
		public int IndexOf(NodeFieldReference value)
		{
			int count = _data.Count;
			NodeFieldReference[] items = _data.Items;

			if ((object) value == null)
			{
				for (int i = 0; i < count; i++)
				{
					if ((object) items[i] == null)
						return i;
				}

				return -1;
			}

			for (int i = 0; i < count; i++)
			{
				if (value.Equals(items[i]))
					return i;
			}

			return -1;
		}

		#endregion

		#region IList.IndexOf(Object)

		/// <summary>
		/// Returns the zero-based index of the first occurrence
		/// of the specified <see cref="Object"/> in the
		/// <see cref="NodeFieldReferenceCollection"/>.
		/// </summary>
		/// <param name="value">
		/// The object to locate in the <see cref="NodeFieldReferenceCollection"/>.
		/// This argument must be compatible with <see cref="NodeFieldReference"/>.
		/// This argument may be a null reference.
		/// </param>
		/// <returns>
		/// The zero-based index of the first occurrence of
		/// <paramref name="value"/> in the <see cref="NodeFieldReferenceCollection"/>,
		/// if found; otherwise, -1.</returns>
		/// <exception cref="InvalidCastException">
		/// <paramref name="value"/> is not compatible with
		/// <see cref="NodeFieldReference"/>.</exception>
		/// <remarks>
		/// Please refer to <see cref="ArrayList.IndexOf"/> for details.
		/// </remarks>
		int IList.IndexOf(object value)
		{
			return IndexOf((NodeFieldReference) value);
		}

		#endregion

		#region IndexOfKey

		/// <summary>
		/// Returns the zero-based index of the first occurrence of the
		/// specified <see cref="NodeFieldReference.Name"/> value
		/// in the <see cref="NodeFieldReferenceCollection"/>.
		/// </summary>
		/// <param name="key">
		/// The <see cref="NodeFieldReference.Type"/> value
		/// to locate in the <see cref="NodeFieldReferenceCollection"/>.
		/// This argument may be a null reference.
		/// </param>
		/// <returns>
		/// The zero-based index of the first occurrence of
		/// <paramref name="key"/> in the <see cref="NodeFieldReferenceCollection"/>,
		/// if found; otherwise, -1.</returns>
		/// <remarks>
		/// <b>IndexOfKey</b> is similar to <see cref="IndexOf"/> but
		/// compares  the specified <paramref name="key"/> to the value
		/// of the <see cref="NodeFieldReference.Names"/> property
		/// of each <see cref="NodeFieldReference"/> element, rather than
		/// to the element itself.
		/// </remarks>
		public int IndexOfKey(Type key)
		{
			int count = _data.Count;
			NodeFieldReference[] items = _data.Items;

			for (int i = 0; i < count; i++)
			{
				if (items[i] == null)
					continue;

				if (key == null)
				{
					continue;
				}

				for (int j = 0; j < items[i].Types.Length; j++)
				{
					if (key.Equals(items[i].Types[j]))
						return i;
				}

			}

			return -1;
		}

		#endregion

		#region Insert(Int32, NodeFieldReference)

		/// <summary>
		/// Inserts a <see cref="NodeFieldReference"/> element into the
		/// <see cref="NodeFieldReferenceCollection"/> at the specified index.
		/// </summary>
		/// <param name="index">
		/// The zero-based index at which <paramref name="value"/>
		/// should be inserted.</param>
		/// <param name="value">
		/// The <see cref="NodeFieldReference"/> object to insert
		/// into the <see cref="NodeFieldReferenceCollection"/>.
		/// This argument may be a null reference.
		/// </param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <para><paramref name="index"/> is less than zero.</para>
		/// <para>-or-</para><para>
		/// <paramref name="index"/> is greater than <see cref="Count"/>.
		/// </para></exception>
		/// <exception cref="NotSupportedException"><para>
		/// The <see cref="NodeFieldReferenceCollection"/> 
		/// is read-only or has a fixed size.
		/// </para><para>-or-</para><para>
		/// The <b>NodeFieldReferenceCollection</b>
		/// already contains <paramref name="value"/>,
		/// and the <b>NodeFieldReferenceCollection</b>
		/// ensures that all elements are unique.
		/// </para></exception>
		/// <remarks>
		/// Please refer to <see cref="ArrayList.Insert"/> for details.
		/// </remarks>
		public virtual void Insert(int index, NodeFieldReference value)
		{
			int count = _data.Count;

			if (index < 0)
				throw new ArgumentOutOfRangeException("index", index, "Argument cannot be negative.");

			if (index > count)
				throw new ArgumentOutOfRangeException("index", index, "Argument cannot exceed Count.");

			if (_data.IsUnique) CheckUnique(value);

			if (count == _data.Items.Length)
				_data.EnsureCapacity(count + 1);

			if (index < count)
				Array.Copy(_data.Items, index, _data.Items, index + 1, count - index);

			_data.Items[index] = value;
			_data.Count++;
		}

		#endregion

		#region IList.Insert(Int32, Object)

		/// <summary>
		/// Inserts an element into the
		/// <see cref="NodeFieldReferenceCollection"/> at the specified index.
		/// </summary>
		/// <param name="index">
		/// The zero-based index at which <paramref name="value"/>
		/// should be inserted.</param>
		/// <param name="value">
		/// The object to insert into the <see cref="NodeFieldReferenceCollection"/>.
		/// This argument must be compatible with <see cref="NodeFieldReference"/>.
		/// This argument may be a null reference.
		/// </param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <para><paramref name="index"/> is less than zero.</para>
		/// <para>-or-</para><para>
		/// <paramref name="index"/> is greater than <see cref="Count"/>.
		/// </para></exception>
		/// <exception cref="InvalidCastException"><paramref name="value"/>
		/// is not compatible with <see cref="NodeFieldReference"/>.</exception>
		/// <exception cref="NotSupportedException"><para>
		/// The <see cref="NodeFieldReferenceCollection"/> 
		/// is read-only or has a fixed size.
		/// </para><para>-or-</para><para>
		/// The <b>NodeFieldReferenceCollection</b>
		/// already contains <paramref name="value"/>,
		/// and the <b>NodeFieldReferenceCollection</b>
		/// ensures that all elements are unique.
		/// </para></exception>
		/// <remarks>
		/// Please refer to <see cref="ArrayList.Insert"/> for details.
		/// </remarks>
		void IList.Insert(int index, object value)
		{
			Insert(index, (NodeFieldReference) value);
		}

		#endregion

		#region ReadOnly

		/// <summary>
		/// Returns a read-only wrapper for the specified
		/// <see cref="NodeFieldReferenceCollection"/>.
		/// </summary>
		/// <param name="collection">
		/// The <see cref="NodeFieldReferenceCollection"/> to wrap.</param>
		/// <returns>
		/// A read-only wrapper around <paramref name="collection"/>.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="collection"/> is a null reference.</exception>
		/// <remarks>
		/// Please refer to <see cref="ArrayList.ReadOnly"/> for details.
		/// </remarks>
		public static NodeFieldReferenceCollection ReadOnly(NodeFieldReferenceCollection collection)
		{
			if (collection == null)
				throw new ArgumentNullException("collection");

			return new ReadOnlyWrapper(collection._data);
		}

		#endregion

		#region Remove(NodeFieldReference)

		/// <summary>
		/// Removes the first occurrence of the specified
		/// <see cref="NodeFieldReference"/> from the
		/// <see cref="NodeFieldReferenceCollection"/>.
		/// </summary>
		/// <param name="value">
		/// The <see cref="NodeFieldReference"/> object to remove
		/// from the <see cref="NodeFieldReferenceCollection"/>.
		/// This argument may be a null reference.
		/// </param>
		/// <exception cref="NotSupportedException">
		/// The <see cref="NodeFieldReferenceCollection"/> 
		/// is read-only or has a fixed size.</exception>
		/// <remarks>
		/// Please refer to <see cref="ArrayList.Remove"/> for details.
		/// </remarks>
		public void Remove(NodeFieldReference value)
		{
			int index = IndexOf(value);
			if (index >= 0) RemoveAt(index);
		}

		#endregion

		#region IList.Remove(Object)

		/// <summary>
		/// Removes the first occurrence of the specified <see cref="Object"/>
		/// from the <see cref="NodeFieldReferenceCollection"/>.
		/// </summary>
		/// <param name="value">
		/// The object to remove from the
		/// <see cref="NodeFieldReferenceCollection"/>. This argument
		/// must be compatible with <see cref="NodeFieldReference"/>.
		/// This argument may be a null reference.
		/// </param>
		/// <exception cref="InvalidCastException">
		/// <paramref name="value"/> is not compatible with
		/// <see cref="NodeFieldReference"/>.</exception>
		/// <exception cref="NotSupportedException">
		/// The <see cref="NodeFieldReferenceCollection"/> 
		/// is read-only or has a fixed size.</exception>
		/// <remarks>
		/// Please refer to <see cref="ArrayList.Remove"/> for details.
		/// </remarks>
		void IList.Remove(object value)
		{
			Remove((NodeFieldReference) value);
		}

		#endregion

		#region RemoveAt

		/// <summary>
		/// Removes the element at the specified index of the
		/// <see cref="NodeFieldReferenceCollection"/>.
		/// </summary>
		/// <param name="index">
		/// The zero-based index of the element to remove.</param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <para><paramref name="index"/> is less than zero.</para>
		/// <para>-or-</para>
		/// <para><paramref name="index"/> is equal to or
		/// greater than <see cref="Count"/>.</para></exception>
		/// <exception cref="NotSupportedException">
		/// The <see cref="NodeFieldReferenceCollection"/> 
		/// is read-only or has a fixed size.</exception>
		/// <remarks>
		/// Please refer to <see cref="ArrayList.RemoveAt"/> for details.
		/// </remarks>
		public virtual void RemoveAt(int index)
		{
			if (index < 0)
				throw new ArgumentOutOfRangeException("index", index, "Argument cannot be negative.");

			if (index >= _data.Count)
				throw new ArgumentOutOfRangeException("index", index, "Argument must be less than Count.");

			int count = --_data.Count;
			NodeFieldReference[] items = _data.Items;

			if (index < count)
				Array.Copy(items, index + 1, items, index, count - index);

			items[count] = null;
		}

		#endregion

		#region RemoveRange

		/// <summary>
		/// Removes the specified range of elements from the
		/// <see cref="NodeFieldReferenceCollection"/>.
		/// </summary>
		/// <param name="index">
		/// The zero-based starting index of the range of elements to remove.
		/// </param>
		/// <param name="count">The number of elements to remove.</param>
		/// <exception cref="ArgumentException">
		/// <paramref name="index"/> and <paramref name="count"/>
		/// do not denote a valid range of elements in the
		/// <see cref="NodeFieldReferenceCollection"/>.</exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <para><paramref name="index"/> is less than zero.</para>
		/// <para>-or-</para>
		/// <para><paramref name="count"/> is less than zero.</para>
		/// </exception>
		/// <exception cref="NotSupportedException">
		/// The <see cref="NodeFieldReferenceCollection"/> 
		/// is read-only or has a fixed size.</exception>
		/// <remarks>
		/// Please refer to <see cref="ArrayList.RemoveRange"/> for details.
		/// </remarks>
		public virtual void RemoveRange(int index, int count)
		{
			if (index < 0)
				throw new ArgumentOutOfRangeException("index", index, "Argument cannot be negative.");

			if (count < 0)
				throw new ArgumentOutOfRangeException("count", count, "Argument cannot be negative.");

			if (index + count > _data.Count)
				throw new ArgumentException("Arguments denote invalid range of elements.");

			if (count == 0) return;
			_data.Count -= count;

			if (index < _data.Count)
				Array.Copy(_data.Items, index + count, _data.Items, index, _data.Count - index);

			Array.Clear(_data.Items, _data.Count, count);
		}

		#endregion

		#region Reverse()

		/// <overloads>
		/// Reverses the order of the elements in the
		/// <see cref="NodeFieldReferenceCollection"/> or a portion of it.
		/// </overloads>
		/// <summary>
		/// Reverses the order of the elements in the entire
		/// <see cref="NodeFieldReferenceCollection"/>.
		/// </summary>
		/// <exception cref="NotSupportedException">
		/// The <see cref="NodeFieldReferenceCollection"/> is read-only.
		/// </exception>
		/// <remarks>
		/// Please refer to <see cref="ArrayList.Reverse"/> for details.
		/// </remarks>
		public virtual void Reverse()
		{
			if (_data.Count > 1)
				Array.Reverse(_data.Items, 0, _data.Count);
		}

		#endregion

		#region Reverse(Int32, Int32)

		/// <summary>
		/// Reverses the order of the elements in the specified range.
		/// </summary>
		/// <param name="index">
		/// The zero-based starting index of the range of elements to reverse.
		/// </param>
		/// <param name="count">The number of elements to reverse.</param>
		/// <exception cref="ArgumentException">
		/// <paramref name="index"/> and <paramref name="count"/>
		/// do not denote a valid range of elements in the
		/// <see cref="NodeFieldReferenceCollection"/>.</exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <para><paramref name="index"/> is less than zero.</para>
		/// <para>-or-</para>
		/// <para><paramref name="count"/> is less than zero.</para>
		/// </exception>
		/// <exception cref="NotSupportedException">
		/// The <see cref="NodeFieldReferenceCollection"/> is read-only.
		/// </exception>
		/// <remarks>
		/// Please refer to <see cref="ArrayList.Reverse"/> for details.
		/// </remarks>
		public virtual void Reverse(int index, int count)
		{
			if (index < 0)
				throw new ArgumentOutOfRangeException("index", index, "Argument cannot be negative.");

			if (count < 0)
				throw new ArgumentOutOfRangeException("count", count, "Argument cannot be negative.");

			if (index + count > _data.Count)
				throw new ArgumentException("Arguments denote invalid range of elements.");

			if (count > 1 && _data.Count > 1)
				Array.Reverse(_data.Items, index, count);
		}

		#endregion

		#region Sort()

		/// <overloads>
		/// Sorts the elements in the <see cref="NodeFieldReferenceCollection"/>
		/// or a portion of it.
		/// </overloads>
		/// <summary>
		/// Sorts the elements in the entire <see cref="NodeFieldReferenceCollection"/>
		/// using the <see cref="IComparable"/> implementation of each element.
		/// </summary>
		/// <exception cref="NotSupportedException">
		/// The <see cref="NodeFieldReferenceCollection"/> is read-only.
		/// </exception>
		/// <exception cref="InvalidOperationException">
		/// One or more elements in the <see cref="NodeFieldReferenceCollection"/>
		/// do not implement the <see cref="IComparable"/> interface.
		/// </exception>
		/// <remarks>
		/// Please refer to <see cref="ArrayList.Sort()"/> for details.
		/// </remarks>
		public virtual void Sort()
		{
			if (_data.Count > 1)
				Array.Sort(_data.Items, 0, _data.Count);
		}

		#endregion

		#region Sort(IComparer)

		/// <summary>
		/// Sorts the elements in the entire
		/// <see cref="NodeFieldReferenceCollection"/> using the specified comparer.
		/// </summary>
		/// <param name="comparer">
		/// <para>The <see cref="IComparer"/> implementation
		/// to use when comparing elements.</para>
		/// <para>-or-</para>
		/// <para>A null reference to use the <see cref="IComparable"/>
		/// implementation of each element.</para></param>
		/// <exception cref="NotSupportedException">
		/// The <see cref="NodeFieldReferenceCollection"/> is read-only.
		/// </exception>
		/// <remarks>
		/// Please refer to
		/// <see cref="ArrayList.Sort(IComparer)"/> for details.
		/// </remarks>
		public virtual void Sort(IComparer comparer)
		{
			if (_data.Count > 1)
				Array.Sort(_data.Items, 0, _data.Count, comparer);
		}

		#endregion

		#region Sort(Int32, Int32, IComparer)

		/// <summary>
		/// Sorts the elements in the specified range
		/// using the specified comparer.
		/// </summary>
		/// <param name="index">
		/// The zero-based starting index of the range of elements to sort.
		/// </param>
		/// <param name="count">The number of elements to sort.</param>
		/// <param name="comparer">
		/// <para>The <see cref="IComparer"/> implementation
		/// to use when comparing elements.</para>
		/// <para>-or-</para>
		/// <para>A null reference to use the <see cref="IComparable"/>
		/// implementation of each element.</para></param>
		/// <exception cref="ArgumentException">
		/// <paramref name="index"/> and <paramref name="count"/>
		/// do not denote a valid range of elements in the
		/// <see cref="NodeFieldReferenceCollection"/>.</exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <para><paramref name="index"/> is less than zero.</para>
		/// <para>-or-</para>
		/// <para><paramref name="count"/> is less than zero.</para>
		/// </exception>
		/// <exception cref="NotSupportedException">
		/// The <see cref="NodeFieldReferenceCollection"/> is read-only.
		/// </exception>
		/// <remarks>
		/// Please refer to
		/// <see cref="ArrayList.Sort(Int32, Int32, IComparer)"/> for details.
		/// </remarks>
		public virtual void Sort(int index, int count, IComparer comparer)
		{
			if (index < 0)
				throw new ArgumentOutOfRangeException("index", index, "Argument cannot be negative.");

			if (count < 0)
				throw new ArgumentOutOfRangeException("count", count, "Argument cannot be negative.");

			if (index + count > _data.Count)
				throw new ArgumentException("Arguments denote invalid range of elements.");

			if (count > 1 && _data.Count > 1)
				Array.Sort(_data.Items, index, count, comparer);
		}

		#endregion

		#region ToArray

		/// <summary>
		/// Copies the elements of the <see cref="NodeFieldReferenceCollection"/>
		/// to a new <see cref="Array"/> of
		/// <see cref="NodeFieldReference"/> elements.
		/// </summary>
		/// <returns>
		/// A one-dimensional <see cref="Array"/> of
		/// <see cref="NodeFieldReference"/> elements containing copies of the
		/// elements of the <see cref="NodeFieldReferenceCollection"/>.
		/// </returns>
		/// <remarks>
		/// Please refer to <see cref="ArrayList.ToArray"/> for details.
		/// </remarks>
		public NodeFieldReference[] ToArray()
		{
			NodeFieldReference[] array = new NodeFieldReference[_data.Count];
			Array.Copy(_data.Items, array, _data.Count);
			return array;
		}

		#endregion

		#region TrimToSize

		/// <summary>
		/// Sets the capacity to the actual number of elements in the
		/// <see cref="NodeFieldReferenceCollection"/>.
		/// </summary>
		/// <exception cref="NotSupportedException">
		/// The <see cref="NodeFieldReferenceCollection"/> 
		/// is read-only or has a fixed size.</exception>
		/// <remarks>
		/// Please refer to <see cref="ArrayList.TrimToSize"/> for details.
		/// </remarks>
		public virtual void TrimToSize()
		{
			_data.SetCapacity(_data.Count);
		}

		#endregion

		#endregion

		#region Private Methods

		#region AddRange

		private void AddRange(NodeFieldReference[] array, int range)
		{
			if (range == 0) return;

			if (_data.IsUnique)
			{
				foreach (NodeFieldReference value in array)
					CheckUnique(value);
			}

			if (_data.Count + range > _data.Items.Length)
				_data.EnsureCapacity(_data.Count + range);

			Array.Copy(array, 0, _data.Items, _data.Count, range);
			_data.Count += range;
		}

		#endregion

		#region CheckUnique

		private void CheckUnique()
		{
			NodeFieldReference[] items = _data.Items;
			for (int i = _data.Count - 1; i > 0; i--)
			{
				if (IndexOf(items[i]) < i)
					throw new InvalidOperationException("Collection cannot contain duplicate elements.");
			}
		}

		private void CheckUnique(NodeFieldReference value)
		{
			if (IndexOf(value) >= 0)
				throw new NotSupportedException("Unique collections cannot contain duplicate elements.");
		}

		private void CheckUnique(NodeFieldReference value, int index)
		{
			int existing = IndexOf(value);
			if (existing >= 0 && existing != index)
				throw new NotSupportedException("Unique collections cannot contain duplicate elements.");
		}

		#endregion

		#endregion

		#region Class Data

		[Serializable]
		private sealed class Data
		{
			public const int DefaultCapacity = 16;

			public NodeFieldReference[] Items;
			public int Count;
			public bool IsUnique;

			#region CheckTargetArray

			public void CheckTargetArray(Array array, int arrayIndex)
			{
				if (array == null)
					throw new ArgumentNullException("array");

				if (array.Rank > 1)
					throw new ArgumentException("Argument cannot be multidimensional.", "array");

				// skip length checks for empty collection and index zero
				if (arrayIndex == 0 && Count == 0)
					return;

				if (arrayIndex < 0)
					throw new ArgumentOutOfRangeException("arrayIndex", arrayIndex, "Argument cannot be negative.");

				if (arrayIndex >= array.Length)
					throw new ArgumentException("Argument must be less than array length.", "arrayIndex");

				if (Count > array.Length - arrayIndex)
					throw new ArgumentException("Argument section must be large enough for collection.", "array");
			}

			#endregion

			#region EnsureCapacity

			public void EnsureCapacity(int minimum)
			{
				int capacity = (Items.Length == 0 ? DefaultCapacity : Items.Length * 2);

				if (capacity < minimum) capacity = minimum;
				SetCapacity(capacity);
			}

			#endregion

			#region SetCapacity

			public void SetCapacity(int capacity)
			{
				if (capacity == Items.Length) return;

				if (capacity == 0)
				{
					Items = new NodeFieldReference[DefaultCapacity];
					return;
				}

				NodeFieldReference[] items = new NodeFieldReference[capacity];
				Array.Copy(Items, items, Count);
				Items = items;
			}

			#endregion
		}

		#endregion

		#region Class Enumerator

		[Serializable]
		private sealed class Enumerator : INodeFieldReferenceEnumerator, IEnumerator
		{
			private readonly Data _data;
			private int _count, _index;

			internal Enumerator(Data data)
			{
				_data = data;
				_count = data.Count;
				_index = -1;
			}

			public NodeFieldReference Current
			{
				get
				{
					if (_index < 0 || _index >= _data.Count)
						throw new InvalidOperationException("Enumerator is not on a collection element.");

					return _data.Items[_index];
				}
			}

			object IEnumerator.Current
			{
				get { return Current; }
			}

			public bool MoveNext()
			{
				if (_count != _data.Count)
					throw new InvalidOperationException("Enumerator invalidated by modification to collection.");

				return (++_index < _count);
			}

			public void Reset()
			{
				_count = _data.Count;
				_index = -1;
			}
		}

		#endregion

		#region Class ReadOnlyWrapper

		private sealed class ReadOnlyWrapper : NodeFieldReferenceCollection
		{
			public ReadOnlyWrapper(Data data) : base(data)
			{
			}

			public override int Capacity
			{
				get { return base.Capacity; }
				set { throw new NotSupportedException("Read-only collections cannot be modified."); }
			}

			public override bool IsFixedSize
			{
				get { return true; }
			}

			public override bool IsReadOnly
			{
				get { return true; }
			}

			public override bool IsUnique
			{
				get { return base.IsUnique; }
				set { throw new NotSupportedException("Read-only collections cannot be modified."); }
			}

			public override NodeFieldReference this[int index]
			{
				get { return base[index]; }
				set { throw new NotSupportedException("Read-only collections cannot be modified."); }
			}

			public override int Add(NodeFieldReference value)
			{
				throw new NotSupportedException("Read-only collections cannot be modified.");
			}

			public override void AddRange(NodeFieldReferenceCollection collection)
			{
				throw new NotSupportedException("Read-only collections cannot be modified.");
			}

			public override void AddRange(NodeFieldReference[] array)
			{
				throw new NotSupportedException("Read-only collections cannot be modified.");
			}

			public override void Clear()
			{
				throw new NotSupportedException("Read-only collections cannot be modified.");
			}

			public override object Clone()
			{
				NodeFieldReferenceCollection wrapper = (NodeFieldReferenceCollection) base.Clone();

				return new ReadOnlyWrapper(wrapper._data);
			}

			public override void Insert(int index, NodeFieldReference value)
			{
				throw new NotSupportedException("Read-only collections cannot be modified.");
			}

			public override void RemoveAt(int index)
			{
				throw new NotSupportedException("Read-only collections cannot be modified.");
			}

			public override void RemoveRange(int index, int count)
			{
				throw new NotSupportedException("Read-only collections cannot be modified.");
			}

			public override void Reverse()
			{
				throw new NotSupportedException("Read-only collections cannot be modified.");
			}

			public override void Reverse(int index, int count)
			{
				throw new NotSupportedException("Read-only collections cannot be modified.");
			}

			public override void Sort()
			{
				throw new NotSupportedException("Read-only collections cannot be modified.");
			}

			public override void Sort(IComparer comparer)
			{
				throw new NotSupportedException("Read-only collections cannot be modified.");
			}

			public override void Sort(int index, int count, IComparer comparer)
			{
				throw new NotSupportedException("Read-only collections cannot be modified.");
			}

			public override void TrimToSize()
			{
				throw new NotSupportedException("Read-only collections cannot be modified.");
			}
		}

		#endregion
	}

	#endregion
}