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

using System;
using System.Collections;
using System.Reflection;
using System.Text.RegularExpressions;
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
				PropertyBag["item"] = item;
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
						PropertyBag["value"] = property.GetValue(item, null);
						Context.RenderSection(property.Name);
						continue;
					}
					RenderStartCell();
					object val = property.GetValue(item, null) ?? "null";
					RenderText(val.ToString());
					RenderText("</td>");
				}
				if (Context.HasSection("more"))
				{
					Context.RenderSection("more");
				}
				isAlternate = !isAlternate;
				RenderText("</tr>");
			}
		}

		private void RenderStartCell()
		{
			if (Context.HasSection("startCell"))
			{
				Context.RenderSection("startCell");
				return;
			}
			RenderText("<td>");
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
				RenderHeaderCellStart();
				RenderText(SplitPascalCase(property.Name));
				RenderText("</th>");
			}
			if (Context.HasSection("moreHeader"))
			{
				Context.RenderSection("moreHeader");
			}
		}

		private void RenderHeaderCellStart()
		{
			if (Context.HasSection("startHeaderCell"))
			{
				Context.RenderSection("startHeaderCell");
				return;
			}
			RenderText("<th class='grid_header'>");
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