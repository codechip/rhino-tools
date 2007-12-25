using System;

namespace NHibernate.Query.Generator.Tests.CompositeIds
{
	public class CompositeIdChild
	{
		private string compositeIdPart1;
		private string compositeIdPart2;
		private CompositeIdParent parent;

		public virtual string CompositeIdPart1
		{
			get
			{
				return compositeIdPart1;
			}
			set
			{
				compositeIdPart1 = value;
			}
		}

		public virtual string CompositeIdPart2
		{
			get
			{
				return compositeIdPart2;
			}
			set
			{
				compositeIdPart2 = value;
			}
		}

		public virtual CompositeIdParent Parent
		{
			get
			{
				return parent;
			}
			set
			{
				parent = value;
			}
		}

		public override bool Equals(object obj)
		{
			CompositeIdChild other = obj as CompositeIdChild;

			if(((object)other) == null)
			{
				return false;
			}

			return CompositeIdPart1 == other.CompositeIdPart1 && CompositeIdPart2 == other.CompositeIdPart2;
		}

		public override int GetHashCode()
		{
			return String.Concat(compositeIdPart1, compositeIdPart2).GetHashCode();
		}
	}
}