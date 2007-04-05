using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace NHibernate.Query.Generator
{
	public class ClassName
	{
		static readonly string[] classes = { "class", "subclass", "joined-subclass" };

		private string genericTypeName;
		private string typeName;
		private string extendsTypeName;
		private string typeNameForCode;

		public ClassName(string hbmCodeNamespace, XmlNode classNode)
		{
			this.genericTypeName = GetTypeNameForCode(hbmCodeNamespace, GetName(classNode));
			this.typeNameForCode = GetTypeNameForCode(hbmCodeNamespace, GetName(classNode));
			this.typeName = GetTypeNameForDisplay(classNode);
			this.extendsTypeName = GetBaseTypeForDiplay(classNode);
		}

		public string TypeNameForCode
		{
			get { return typeNameForCode; }
		}
		public string GenericTypeName
		{
			get { return genericTypeName; }
		}

		public string TypeName
		{
			get { return typeName; }
		}

		public string ExtendsTypeName
		{
			get { return extendsTypeName; }
		}

		public static string GetTypeNameForCode(string hbmCodeNameSpace, string name)
		{
			string typeName = name.Split(',')[0];
			bool hasNamespace = typeName.Contains(".");
			typeName = typeName.Replace('+', '.'); //inner classes
			if (hbmCodeNameSpace == null || hasNamespace)
				return typeName;
			return hbmCodeNameSpace + "." + typeName;
		}

		public static string GetTypeNameForDisplay(XmlNode classNode)
		{
			string typeName = GetName(classNode);
			return GetTypeNameForDisplay(typeName);
		}

		public static string GetTypeNameForDisplay(string typeName)
		{
			if (string.IsNullOrEmpty(typeName))
			{
				throw new ArgumentNullException("typeName", "Typename is empty! you must pass non empty string");
			}
			int firstIndexOfComma = typeName.IndexOf(',');
			if (firstIndexOfComma == -1)
				firstIndexOfComma = typeName.Length;
			if (firstIndexOfComma < 0 && typeName.IndexOf('.') < 0)
			{
				return typeName;
			}
			int lastIndexOfPeriod = typeName.LastIndexOf('.', firstIndexOfComma - 1) + 1;
			if (lastIndexOfPeriod == -1)
				lastIndexOfPeriod = 0;
			return typeName.Substring(lastIndexOfPeriod, firstIndexOfComma - lastIndexOfPeriod);
		}

		public static string GetBaseTypeForDiplay(XmlNode classNode)
		{
			string typeName = GetFullNameOfBaseClass(classNode);
			if (string.IsNullOrEmpty(typeName)) return null;
			return GetTypeNameForDisplay(typeName);
		}

		/// <summary>
		/// Gets the name, we assume that this is safe, since we have schema validation
		/// </summary>
		public static string GetName(XmlNode node)
		{
			if (node.Attributes["name"] == null) //this may happen if the <id> node doesn't have a name
			{
				throw new InvalidOperationException("Can't find attribute 'name' on element " + node.Name);
			}
			return node.Attributes["name"].Value;
		}

		public static string GetFullNameOfBaseClass(XmlNode node)
		{
			if (node.Attributes["extends"] == null) ///does it extends anything explicitly?
			{
				if (Array.IndexOf(classes, node.ParentNode.Name) != -1)
				{
					return ClassName.GetName(node.ParentNode);
				}
				return null;
			}
			return node.Attributes["extends"].Value;
		}


	}
}
