using Ayende.NHibernateQueryAnalyzer.Model;

namespace Ayende.NHibernateQueryAnalyzer.Model
{
	/// <summary>
	/// Summary description for Query.
	/// </summary>
	public class Query
	{
		#region Variables
		Project ownerProject;
		string name;
				string text;
		private int id=0;
		#endregion 

		
				#region Properties
				public Project OwnerProject
		{
			get { return ownerProject; }
			set
			{
				ownerProject = value;
			}
		}

		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		public string Text
		{
			get { return text; }
			set { text = value; }
		} 
				public int Id
		{
			get { return id; }
		}

				#endregion 

		#region c'tors
		public Query(string name, string text)
		{
			this.name = name;
			this.text = text;
		}

		public Query()
		{
			
		}
		#endregion 

	}
}