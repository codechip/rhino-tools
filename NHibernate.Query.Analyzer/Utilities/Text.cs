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