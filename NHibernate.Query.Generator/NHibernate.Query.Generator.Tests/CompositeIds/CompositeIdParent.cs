using System;
using System.Collections.Generic;

namespace NHibernate.Query.Generator.Tests.CompositeIds
{
	public class CompositeIdParent
	{
		private Guid id;
		private IList<CompositeIdChild> children;

		public virtual Guid Id
		{
			get
			{
				return id;
			}
			set
			{
				id = value;
			}
		}

		public virtual IList<CompositeIdChild> Children
		{
			get
			{
				return children;
			}
			set
			{
				children = value;
			}
		}
	}
}