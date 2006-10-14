using System;
using System.Collections.Generic;
using System.Text;

namespace NHibernate.Generics.Tests
{
	public class PostsSet : EntitySet<Post>
	{
		public PostsSet(Action<Post> add, Action<Post> remove) : base(add, remove)
		{
		}
	}
}
