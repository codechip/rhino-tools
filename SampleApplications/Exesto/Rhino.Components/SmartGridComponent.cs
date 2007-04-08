// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections;
using System.Reflection;
using System.Text.RegularExpressions;
using Castle.MonoRail.Framework.Helpers;
using System.Collections.Generic;

namespace Rhino.Components
{
	public class SmartGridComponent : GridComponent
	{
		private static Hashtable validTypesCache = Hashtable.Synchronized(new Hashtable());
		private static Hashtable propertiesCache = Hashtable.Synchronized(new Hashtable());
		private PropertyInfo[] properties;

		public override bool SupportsSection(string name)
		{
			return true;
		}

		protected override void ShowRows(IEnumerable source)
		{
			if (properties == null) //there are no rows, if this is the case
				return;
			bool isAlternate = false;
			foreach (object item in source)
			{
				if (isAlternate)
					RenderText("<tr class='grid_alternateItem'>");
				else
					RenderText("<tr class='grid_item'>");
				foreach (PropertyInfo property in properties)
				{
					if (ValidPropertyToAutoGenerate(property) == false)
						continue;
					if (Context.HasSection(property.Name))
					{
						PropertyBag["item"] = property.GetValue(item, null);
						Context.RenderSection(property.Name);
						continue;
					}
					RenderText("<td>");
					object val = property.GetValue(item, null) ?? "null";
					RenderText(val.ToString());
					RenderText("</td>");
				}
				isAlternate = !isAlternate;
				RenderText("</tr>");
			}
		}

		protected override void ShowHeader(IEnumerable source)
		{
			IEnumerator enumerator = source.GetEnumerator();
			bool hasItem = enumerator.MoveNext();
			if (hasItem == false)
			{
				return;
			}
			object first = enumerator.Current;
			InitializeProperties(first);
			foreach (PropertyInfo property in this.properties)
			{
				if (ValidPropertyToAutoGenerate(property) == false)
					continue;
				string overrideSection = property.Name + "Header";
				if (Context.HasSection(overrideSection))
				{
					Context.RenderSection(overrideSection);
					continue;
				}
				RenderText("<th class='grid_header'>");
				RenderText(SplitPascalCase(property.Name));
				RenderText("</th>");
			}
		}


		/// <summary>
		/// Split a PascalCase string into Pascal Case words.
		/// Note that if the string contains spaces, we assume it is already formatted
		/// http://weblogs.asp.net/jgalloway/archive/2005/09/27/426087.aspx
		/// </summary>
		private static string SplitPascalCase(string input)
		{
			if (input.Contains(" "))
				return input;
			return Regex.Replace(input, "([A-Z])", " $1", RegexOptions.Compiled);
		}

		private void InitializeProperties(object first)
		{
			Type type = first.GetType();
			if (ComponentParams.Contains("columns") == false)
			{
				if (propertiesCache.Contains(type))
					properties = (PropertyInfo[])propertiesCache[type];
				else
					propertiesCache[type] = properties = type.GetProperties();
				return;
			}
			List<PropertyInfo> props = new List<PropertyInfo>();
			IEnumerable columns = (IEnumerable)ComponentParams["columns"];
			foreach (string columnName in columns)
			{
				string key = type.FullName + "." + columnName;
				PropertyInfo propertyInfo;
				if (propertiesCache.Contains(key))
					propertyInfo = (PropertyInfo)propertiesCache[key];
				else
					propertiesCache[key] = propertyInfo = type.GetProperty(columnName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
				if (propertyInfo != null)
					props.Add(propertyInfo);
			}
			properties = props.ToArray();
		}

		private bool ValidPropertyToAutoGenerate(PropertyInfo property)
		{
			if (false.Equals(ComponentParams["Display" + property.Name]))
				return false;
			if (Context.HasSection(property.Name))
				return true;
			return IsValidType(property.PropertyType);
		}

		private static bool IsValidType(Type typeToCheck)
		{
			if (validTypesCache.ContainsKey(typeToCheck))
				return (bool)validTypesCache[typeToCheck];
			bool result;
			if (typeof(ICollection).IsAssignableFrom(typeToCheck))
			{
				result = false;
			}
			else if (typeToCheck.IsGenericType)
			{
				result = typeof(ICollection<>).IsAssignableFrom(typeToCheck.GetGenericTypeDefinition());
			}
			else
			{
				result = true;
			}
			validTypesCache[typeToCheck] = result;
			return result;
		}
	}
}