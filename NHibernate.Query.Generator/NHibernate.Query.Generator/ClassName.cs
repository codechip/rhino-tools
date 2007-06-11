using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace NHibernate.Query.Generator
{

	public class ClassName
	{
		static readonly string[] XPathForClass = {
			                        	"/nh:hibernate-mapping//nh:class", "/nh:hibernate-mapping//nh:joined-subclass",
			                        	"/nh:hibernate-mapping//nh:subclass"};

		static readonly string[] XPathForComponents = { "nh:component", "nh:dynamic-component", "nh:composite-id" };

		static readonly string[] XPathForQueries = {"/nh:hibernate-mapping/nh:query"};

		static readonly string[] classes = { "class", "subclass", "joined-subclass" };
		
		static readonly string[] numericTypeNames = new string[] { "int", "integer", "smallint", "bigint", "tinyint", "decimal"
		                                                   , "float", "double", "short", "long", "big_decimal", "Int32"
		                                                   , "Int64", "Int16", "Single", "Decimal", "Double"
		                                                   , "System.Int32", "System.Int64", "System.Int16"
		                                                   , "System.Single", "System.Decimal", "System.Double" };

		private static XmlNamespaceManager xmlNsMgr;
		private static string hbmCodeNamespace;
		private static string baseNamespace;
		private static bool isInitialized;

		private string genericTypeName;
		private string typeName;
		private string extendsTypeName;
		private string typeNameForCode;
		private XmlNode xmlNode;
		private ClassName parentClassName;


		public ClassName(XmlNode classNode)
		{
			this.genericTypeName = GetTypeNameForCode(HbmCodeNamespace, GetName(classNode));
			this.typeNameForCode = GetTypeNameForCode(HbmCodeNamespace, GetName(classNode));
			this.typeName = GetTypeNameForDisplay(classNode);
			this.extendsTypeName = GetBaseTypeForDiplay(classNode);
			this.xmlNode = classNode;
		}

		public ClassName(string hbmCodeNamespace, XmlNode classNode)
		{
			this.genericTypeName = GetTypeNameForCode(hbmCodeNamespace, GetName(classNode));
			this.typeNameForCode = GetTypeNameForCode(hbmCodeNamespace, GetName(classNode));
			this.typeName = GetTypeNameForDisplay(classNode);
			this.extendsTypeName = GetBaseTypeForDiplay(classNode);
			this.xmlNode = classNode;
		}

		public static void Init(XmlNamespaceManager xmlNsMgr, string baseNamespace, string hbmCodeNamespace)
		{
			ClassName.xmlNsMgr = xmlNsMgr;
			ClassName.hbmCodeNamespace = hbmCodeNamespace;
			ClassName.baseNamespace = baseNamespace;
			Array.Sort(numericTypeNames);
			isInitialized = true;
		}

		public static XmlNamespaceManager XmlNsMgr
		{
			get { EnsureInit(); return xmlNsMgr; }
		}

		public static string BaseNamespace
		{
			get { EnsureInit(); return baseNamespace; }
		}

		public static string HbmCodeNamespace
		{
			get { EnsureInit(); return hbmCodeNamespace; }
		}
		
		public XmlNode XmlNode
		{
			get { return xmlNode; }
		}

		public static IEnumerable<ClassName> SelectClassesFromDoc(XmlDocument doc)
		{
			EnsureInit();
			foreach(XmlNode node in SelectNodes(doc, XPathForClass))
				yield return new ClassName(node);
		}

		public static IEnumerable<ClassName> SelectQueriesFromDoc(XmlDocument doc)
		{
			EnsureInit();
			foreach (XmlNode node in SelectNodes(doc, XPathForQueries))
				yield return new ClassName(node);
		}

		private static void EnsureInit()
		{
			if (!isInitialized)
				throw new InvalidOperationException("ClassName must be initialized first by calling the Init method!");
		}

		public IEnumerable<ClassName> SelectPropertiesForClass(ClassName className)
		{
			yield break;
		}

		public IEnumerable<ClassName> SelectComponentsForClass(ClassName className)
		{
			yield break;
		}

		private static IEnumerable<XmlNode> SelectNodes(XmlNode node, params string[] xpathExpressions)
		{
			foreach (string xpExpr in xpathExpressions)
				foreach (XmlNode foundNode in node.SelectNodes(xpExpr, XmlNsMgr))
					yield return foundNode;
		}

		public ClassName ParentClassName
		{
			get { return parentClassName; }
		}

		public string Name
		{
			get { return GetName(XmlNode); }
		}

		public string TypeName
		{
			get { return typeName; }
		}

		public string FqnsTypeName
		{
			get { return ""; }
		}

		public bool IsNumeric
		{
			get 
			{ 
				if (XmlNode == null || string.IsNullOrEmpty(XmlNode.Attributes["type"].Value))
					return false;
				return (Array.BinarySearch(numericTypeNames, XmlNode.Attributes["type"].Value) >= 0);
			}
		}

		public bool IsGeneric
		{
			get { return false; }
		}

		public bool IsComponent
		{
			get { return false; }
		}

		public bool IsCollection
		{
			get { return false; }
		}

		public string GetTypeNameForCode(string baseNamespace)
		{
			return baseNamespace + "." + TypeNameForCode;
		}

		public string TypeNameForCode
		{
			get { return typeNameForCode; }
		}

		public string GenericTypeName
		{
			get { return genericTypeName; }
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
