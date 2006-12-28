using System;
using System.Collections.Generic;
using System.Text;
using Castle.ActiveRecord;
using Iesi.Collections.Generic;

namespace Exesto.Model
{
	[ActiveRecord]
	public class Subject
	{
		private int id;
		private string name;
		private ISet<Question> questions = new HashedSet<Question>();

		public Subject()
		{
		}

		public Subject(int id, string name)
		{
			this.id = id;
			this.name = name;
		}

		[PrimaryKey]
		public virtual int Id
		{
			get { return id; }
			set { id = value; }
		}

		[Property]
		public virtual string Name
		{
			get { return name; }
			set { name = value; }
		}

		[HasMany]
		public virtual ISet<Question> Questions
		{
			get { return questions; }
			set { questions = value; }
		}

		public override string ToString()
		{
			return Name;
		}
	}
}