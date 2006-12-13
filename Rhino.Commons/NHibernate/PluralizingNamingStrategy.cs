using System;
using System.Collections.Generic;
using System.Text;
using Castle.ActiveRecord.Framework.Internal;
using NHibernate.Cfg;

namespace Rhino.Commons.NHibernateUtil
{
	public class PluralizingNamingStrategy : INamingStrategy
	{
		public static PluralizingNamingStrategy Instnace = new PluralizingNamingStrategy();
		
		public string ClassToTableName(string fullClassName)
		{
			string className = GetClassName(fullClassName);
			string toPlural = Inflector.Pluralize(className);
			return toPlural;
		}

		public string PropertyToColumnName(string propertyName)
		{
			return propertyName;
		}

		public string TableName(string tableName)
		{
			return tableName;
		}

		public string ColumnName(string columnName)
		{
			return columnName;
		}

		Dictionary<string, string> previousAssociations = new Dictionary<string, string>();
		
		public string PropertyToTableName(string className, string propertyName)
		{
			string first = Inflector.Pluralize(GetClassName(className));
			string second = propertyName;
			string association_table = string.Format("{0}_{1}", first, second);

			if (previousAssociations.ContainsKey(association_table))
				return previousAssociations[association_table];
			
			// This is needed so reversed assoications will work.
			// for isntnace, if we have many to many, we want only a single connecting 
			// table between the two. This way, the first association determains which is the 
			// table name.
			string reversed_association_table = string.Format("{0}_{1}", second, first);
			previousAssociations.Add(reversed_association_table, association_table);
			return association_table;
		}

		private static string GetClassName(string fullClassName)
		{
			int lastIndexOfDot = fullClassName.LastIndexOf('.');
			return fullClassName.Substring(lastIndexOfDot + 1);
		}
	}
}
