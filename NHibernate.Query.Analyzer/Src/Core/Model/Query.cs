namespace Ayende.NHibernateQueryAnalyzer.Model {
	/// <summary>
	/// Summary description for Query.
	/// </summary>
	public class Query {
		#region Variables

		private Project ownerProject;
		private string name;
		private string text;
		private int id = 0;

		#endregion

		#region Properties

		public virtual Project OwnerProject {
			get { return ownerProject; }
			set { ownerProject = value; }
		}

		public virtual string Name {
			get { return name; }
			set { name = value; }
		}

		public virtual string Text {
			get { return text; }
			set { text = value; }
		}

		public virtual int Id {
			get { return id; }
		}

		#endregion 

		#region c'tors

		public Query(string name, string text) {
			this.name = name;
			this.text = text;
		}

		public Query() {}

		#endregion
	}
}
