using System;
using System.Collections.Generic;
using System.Text;

namespace NHibernate.Query.Generator
{
	public class GeneratorPart
	{
		public string Ns;
		public string ClauseName;
		public string RootClassName;
		public string EntityClassName;
		public string PropertyClassName;
		public bool RootIsGeneric;
		public bool EntityIsGeneric;

		public GeneratorPart(string ns, string clauseName, string rootClassName, string entityClassName, string propertyClassName, bool rootIsGeneric, bool entityIsGeneric)
		{
			this.Ns = ns;
			this.ClauseName = clauseName;
			this.RootClassName = rootClassName;
			this.EntityClassName = entityClassName;
			this.PropertyClassName = propertyClassName;
			this.RootIsGeneric = rootIsGeneric;
			this.EntityIsGeneric = entityIsGeneric;
		}
	}
}
