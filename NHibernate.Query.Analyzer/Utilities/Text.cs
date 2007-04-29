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


using System.Collections;
using System.Collections.Specialized;
using System.Reflection;
using System.Text;

namespace Ayende.NHibernateQueryAnalyzer.Utilities
{
	/// <summary>
	/// Summary description for TextUtil.
	/// </summary>
	public sealed class Text
	{
		private Text()
		{
		}

		/// <summary>
		/// Check is the string variable is null or empty
		/// </summary>
		/// <param name="str">The string to check</param>
		/// <returns>true is the string has value</returns>
		public static bool NotNullOrEmpty(string str)
		{
			if (str == null || str.Length == 0)
				return false;
			else
				return true;
		}

		/// <summary>
		/// Parses the bool, but allows for empty string an null.
		/// </summary>
		/// <param name="booleanString">String containing the boolean value</param>
		/// <param name="defaultValue">Default value to use if booleanString is null or empty.</param>
		public static bool ParseBool(string booleanString, bool defaultValue)
		{
			if (!NotNullOrEmpty(booleanString))
				return defaultValue;
			return bool.Parse(booleanString);
		}

		/// <summary>
		/// Objects the state to string. With the pattern of:
		/// Object '[Object-Name]'
		///		Property-Name: Property-Value or Type
		/// </summary>
		/// <param name="obj">Obj.</param>
		/// <returns></returns>
		public static string ObjectStateToString(object obj)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("Object '").Append(ReflectionUtil.GetName(obj)).Append("\'\r\n");
			foreach (PropertyInfo property in obj.GetType().GetProperties())
			{
				sb.Append('\t').Append(property.Name).Append(": ");
				if (property.CanRead)
				{
					if (ReflectionUtil.IsSimpleType(property.PropertyType))
						sb.Append(property.GetValue(obj, null));
					else
						sb.Append('{').Append(property.PropertyType.Name).Append('}');
					sb.Append("\r\n");
				}
			}
			return sb.ToString();
		}

		/// <summary>
		/// This method exist because there is no method that Join a
		/// StringCollection.
		/// </summary>
		public static string Join(string seperator, IEnumerable lines)
		{
			StringBuilder sb = new StringBuilder();
			foreach (string line in lines)
				sb.Append(line).Append(seperator);
			sb.Remove(sb.Length - seperator.Length, seperator.Length);
			return sb.ToString();
		}

		public static string ExceptionString(string name)
		{
			return name;
		}

		public static string ResourceString(string name)
		{
			return name;
		}
	}
}