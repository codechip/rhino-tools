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


using System.Collections.Generic;
using Castle.ActiveRecord.Framework.Internal;
using NHibernate.Cfg;

namespace Rhino.Commons.NHibernateUtil
{
	using global::NHibernate.Util;

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

		public string LogicalColumnName(string columnName, string propertyName)
		{
			return StringHelper.IsNotEmpty(columnName) ? columnName : StringHelper.Unqualify(propertyName);			
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
