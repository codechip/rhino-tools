using System;
using System.Collections.Generic;
using System.Text;
using NHibernate;
using NHibernate.Generics;
using Iesi.Collections;

namespace NHibernate.Generics
{
	/// <summary>
	/// This class handle strong typing collections that you get from
	/// NHibernate.
	/// It also handles adding/removing items to the other end (by allowing
	/// you to specify delegates to do it).
	/// <remarks>
	/// It also handles the case of adding/removing to the set while it's not loaded without loading it
	/// from the database.
	/// Note: It does so by skipping add/remove to the internal set, which means that you're going to 
	/// get into trouble if you do something like this:
	/// //Posts collection is lazy loaded
	/// Blog blog = GetBlog();
	/// //Posts collection is not loaded because of this
	/// blog.Posts.Add(post);
	/// //Posts collection is loaded, and post is not there since it was not saved
	/// //to the database yet.
	/// Assert.IsTrue(blog.Posts.Contains(post));
	/// 
	/// If you're planning on doing something similiar, you need to Flush() the current
	/// session in order to allow this.
	/// </remarks>
	/// </summary>
	/// <typeparam name="T">The type of the collection to pretend to be</typeparam>
    [Serializable]
    public class EntitySet<T> : AbstractEntityCollection<T> where T : class 
	{
	    private Iesi.Collections.ISet _set;

	    protected override System.Collections.ICollection Collection
	    {
            get { return _set;  }
        }
        #region Constructors

        public EntitySet()
            : this(null, null)
        {

        }

        public EntitySet(ISet set)
            : this(set, null, null)
        {
        }


        public EntitySet(ISet set, InitializeOnLazy lazyBehavior)
            : this(set, null, null, lazyBehavior)
        {
        }


        public EntitySet(Action<T> add, Action<T> remove, InitializeOnLazy lazyBehavior)
            : base(add, remove, lazyBehavior)
        {
            SetInitialSet(new HashedSet());
        }

        public EntitySet(Action<T> add, Action<T> remove)
            : this(new HashedSet(), add, remove)
        { }

        public EntitySet(ISet set, Action<T> add, Action<T> remove, InitializeOnLazy lazyBehavior)
            : base(add, remove, lazyBehavior)
        {
            SetInitialSet(set);
        }
        public EntitySet(ISet set, Action<T> add, Action<T> remove)
            : base(add, remove)
        {
            SetInitialSet(set);
        }
        #endregion

        private void SetInitialSet(ISet set)
	    {
	        if (set == null)
	            throw new ArgumentNullException("set");
	        this._set = set;
	    }

	    #region ICollection<T> Members

	    protected override bool DoAdd(T item)
	    {
	        return _set.Add(item);
	    }

	    protected override void DoClear()
	    {
	        _set.Clear();
	    }

	    public override bool Contains(T item)
		{
			return _set.Contains(item);
		}

	    protected override bool DoRemove(T item)
	    {
	        return  _set.Remove(item);
	    }

	    #endregion

		#region IEnumerable<T> Members

	    #endregion

		#region IEnumerable Members

	    #endregion

		#region IWrapper Members

	    protected override object InnerValue
	    {
            get { return _set; }
            set { _set = (ISet)value; }
	    }

	    #endregion

        #region Implementation

	    #endregion
	}
}
