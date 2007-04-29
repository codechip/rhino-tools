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
