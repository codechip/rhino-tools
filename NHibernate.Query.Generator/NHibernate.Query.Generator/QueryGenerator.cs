using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;
using System.Xml;
using System.Xml.Schema;

namespace NHibernate.Query.Generator
{
	/// <summary>
	/// Read an HBM file and generates a cs query file
	/// </summary>
	public class QueryGenerator
	{
		private const string UseTheQueryClass = "UseTheQueryClass";

		private readonly TextReader _reader;
		private CodeDomProvider _provider;
		private XmlDocument hbm = new XmlDocument();
		private XmlNamespaceManager nsMgr;
		private string hbmCodeNameSpace;

		/// <summary>
		/// This is used to avoid name collisions when generating the generic query types.
		/// </summary>
		private int genericTypeNamesRequested = 1;

		public QueryGenerator(TextReader reader, CodeDomProvider provider)
		{
			_reader = reader;
			_provider = provider;
		}

		public void Generate(TextWriter writer)
		{
			//General CodeDOM setup
			CodeCompileUnit unit = new CodeCompileUnit();
			CodeNamespace queryNameSpace = new CodeNamespace("Query");
			unit.Namespaces.Add(queryNameSpace);
			CodeTypeDeclaration whereTypeDeclaration = new CodeTypeDeclaration("Where");
			queryNameSpace.Types.Add(whereTypeDeclaration);
			whereTypeDeclaration.IsPartial = true;

			ValidateAndLoadXmlDocument();

			CreateClasses(whereTypeDeclaration);

			CreateQueries(queryNameSpace, hbm);

			_provider.GenerateCodeFromCompileUnit(unit, writer, new CodeGeneratorOptions());
		}

		private void ValidateAndLoadXmlDocument()
		{
			XmlReaderSettings settings = new XmlReaderSettings();
			settings.ValidationType = ValidationType.Schema;
			settings.Schemas.Add(
				XmlSchema.Read(typeof(NHibernateUtil).Assembly.GetManifestResourceStream("NHibernate.nhibernate-mapping-2.0.xsd"),
							   null));
			hbm.Load(XmlReader.Create(_reader, settings));
			nsMgr = new XmlNamespaceManager(hbm.NameTable);
			nsMgr.AddNamespace("nh", "urn:nhibernate-mapping-2.0");

			XmlNode codeNameSpace = hbm.SelectSingleNode("/nh:hibernate-mapping/@namespace", nsMgr);
			if (codeNameSpace != null)
				hbmCodeNameSpace = codeNameSpace.Value;
		}

		/// <summary>
		/// Creates strongly typed queries for eached registered named query
		/// </summary>
		private void CreateQueries(CodeNamespace queryNameSpace, XmlDocument xdoc)
		{
			foreach (XmlNode queryNode in xdoc.SelectNodes("/nh:hibernate-mapping/nh:query", nsMgr))
			{
				GeneratePropertyForQuery(queryNode, queryNameSpace);
			}
		}

		/// <summary>
		/// This method is here because I want to be able to easily control what GenerateClasses
		/// will process. In the past, GenerateClasses was also recursive.
		/// </summary>
		private void CreateClasses(CodeTypeDeclaration whereTypeDeclaration)
		{
			GenerateClasses(whereTypeDeclaration,
							hbm,
							"/nh:hibernate-mapping//nh:class",
							"/nh:hibernate-mapping//nh:joined-subclass",
							"/nh:hibernate-mapping//nh:subclass");
		}

		/// <summary>
		/// Generates the classes, for each class, a root class and a query class are generated
		/// </summary>
		private void GenerateClasses(CodeTypeDeclaration parent, XmlNode node, params string[] classesXpath)
		{
			foreach (string xpathForClass in classesXpath)
			{
				foreach (XmlNode classNode in node.SelectNodes(xpathForClass, nsMgr))
				{
					string genericTypeName = GetTypeNameForCode(classNode);
					string typeNameForDisplay = GetTypeNameForDisplay(classNode);

					// This creates the query class Query_Blog<T2>
					CodeTypeDeclaration innerClass = CreateQueryClassInParentClass(parent, typeNameForDisplay);
					CreateChildProperties(classNode,
										  innerClass,
										  AssoicationBehavior.AddAssoicationFromName,
										  innerClass.TypeParameters[0].Name);

					//This creates the root query class, Where.Blog
					CodeTypeDeclaration rootClass = CreateRootClassAndPropertyInParentClass(parent, typeNameForDisplay);
					CreateChildProperties(classNode, rootClass, AssoicationBehavior.AddAssoicationHardCoded, genericTypeName);
				}
			}
		}

		/// <summary>
		/// This generate the properties of a query class (or the root class)
		/// </summary>
		private void CreateChildProperties(
			XmlNode classNode, CodeTypeDeclaration innerClass, AssoicationBehavior assoicationBehavior, string genericName)
		{
			// generate full object query for simple properties
			GenerateProperties(null,
							   genericName,
							   AssoicationBehavior.DoNotAdd,
							   "Query.PropertyQueryBuilder",
							   classNode,
							   innerClass,
							   "nh:property",
							   "nh:composite-id/nh:key-property");
			// generate simple equality for id
			GenerateProperties(null,
							   genericName,
							   AssoicationBehavior.DoNotAdd,
							   "Query.QueryBuilder",
							   classNode,
							   innerClass,
							   "nh:id");
			// generate reference to related query obj
			GenerateProperties(null,
							   genericName,
							   assoicationBehavior,
							   UseTheQueryClass,
							   classNode,
							   innerClass,
							   "nh:many-to-one",
							   "nh:composite-id/nh:key-many-to-one");
			// generate reference to component
			GenerateComponents(genericName, innerClass, classNode, "nh:component", "nh:dynamic-component");
		}

		private void GenerateComponents(
			string genericParameterName, CodeTypeDeclaration parent, XmlNode node, params string[] classesXpath)
		{
			foreach (string xpathForClass in classesXpath)
			{
				foreach (XmlNode classNode in node.SelectNodes(xpathForClass, nsMgr))
				{
					string name = GetName(classNode);
					CodeTypeDeclaration innerClass = CreateQueryClassInParentClass(parent, name);

					CreateComponentPropertyInParentClass(genericParameterName, innerClass, name, parent);

					//create full object query object
					GenerateProperties(name,
									   genericParameterName,
									   AssoicationBehavior.DoNotAdd,
									   "Query.PropertyQueryBuilder",
									   classNode,
									   innerClass,
									   "nh:property",
									   "nh:id");
					// create reference query obj
					GenerateProperties(name,
									   genericParameterName,
									   AssoicationBehavior.AddAssoicationFromName,
									   UseTheQueryClass,
									   classNode,
									   innerClass,
									   "nh:many-to-one");
				}
			}
		}

		/// <summary>
		/// Create a property that return a new component each time it is called
		/// </summary>
		private static void CreateComponentPropertyInParentClass(
			string genericParameterName, CodeTypeDeclaration innerClass, string name, CodeTypeDeclaration parent)
		{
			CodeMemberProperty prop = new CodeMemberProperty();
			prop.Name = name;
			prop.Type = new CodeTypeReference(innerClass.Name, new CodeTypeReference(genericParameterName));
			prop.Attributes = MemberAttributes.Public;
			prop.GetStatements.Add(
				new CodeMethodReturnStatement(
					new CodeObjectCreateExpression(
						new CodeTypeReference(innerClass.Name, new CodeTypeReference(genericParameterName)),
						new CodePrimitiveExpression(name),
						new CodePrimitiveExpression(null))));
			parent.Members.Add(prop);
		}

		/// <summary>
		/// Creates the root class and property in parent class.
		/// Note that the Root Class does not inherit from QueryBuilder and is not a generic class. 
		/// The idea of having both root class and a query class is to avoid the possiblity of:
		/// Where.Post.Gt()
		/// </summary>
		private CodeTypeDeclaration CreateRootClassAndPropertyInParentClass(CodeTypeDeclaration parent, string display)
		{
			// Root_Query_Blog
			CodeTypeDeclaration innerClass = new CodeTypeDeclaration("Root_Query_" + display);

			CodeMemberField assoicationPath = new CodeMemberField();
			assoicationPath.Name = "assoicationPath";
			assoicationPath.Type = new CodeTypeReference(typeof(string).FullName);
			assoicationPath.InitExpression = new CodePrimitiveExpression("this");
			innerClass.Members.Add(assoicationPath);

			// class Where { Root_Query_Blog  _query_Blog = new Root_Query_Blog(); }
			CodeMemberField field = new CodeMemberField();
			field.Name = "_root_query_" + display;
			field.Type = new CodeTypeReference(innerClass.Name);
			field.Attributes = MemberAttributes.Static;
			CodeObjectCreateExpression createField = new CodeObjectCreateExpression(field.Type);
			field.InitExpression = createField;

			// proeprty 
			CodeMemberProperty prop = new CodeMemberProperty();
			prop.Name = display;
			prop.Type = field.Type;
			prop.Attributes = MemberAttributes.Public | MemberAttributes.Static;
			prop.GetStatements.Add(new CodeMethodReturnStatement(new CodeFieldReferenceExpression(null, field.Name)));
			parent.Members.Add(innerClass);
			parent.Members.Add(field);
			parent.Members.Add(prop);
			return innerClass;
		}

		/// <summary>
		/// Creates the query class in parent class.
		/// The query class return a query object per each property in the persistant class.
		/// It is also keeping track of what is going on and is capable of tracking joins on the fly.
		/// This is done by combining the generated code and QueryBuilder
		/// </summary>
		private CodeTypeDeclaration CreateQueryClassInParentClass(CodeTypeDeclaration parent, string display)
		{
			// Query_Blog<T1> : Query.QueryBuilder<T1>
			CodeTypeDeclaration innerClass = new CodeTypeDeclaration("Query_" + display);
			string genericParameterName = GetGenericParameterName();
			innerClass.TypeParameters.Add(genericParameterName);
			innerClass.BaseTypes.Add(new CodeTypeReference("Query.QueryBuilder", new CodeTypeReference(genericParameterName)));


			// Query_Blog(string name, string associationPath) : QueryBuilder<T1>(name, assoicationPath);
			CodeConstructor ctor = new CodeConstructor();
			ctor.Attributes = MemberAttributes.Public;
			ctor.Parameters.Add(new CodeParameterDeclarationExpression(typeof(string), "name"));
			ctor.Parameters.Add(new CodeParameterDeclarationExpression(typeof(string), "assoicationPath"));
			ctor.BaseConstructorArgs.Add(new CodeVariableReferenceExpression("name"));
			ctor.BaseConstructorArgs.Add(new CodeVariableReferenceExpression("assoicationPath"));
			innerClass.Members.Add(ctor);


			// ctor for backtracking
			CodeConstructor ctor2 = new CodeConstructor();
			ctor2.Attributes = MemberAttributes.Public;
			ctor2.Parameters.Add(new CodeParameterDeclarationExpression(typeof(string), "name"));
			ctor2.Parameters.Add(new CodeParameterDeclarationExpression(typeof(string), "assoicationPath"));
			ctor2.Parameters.Add(new CodeParameterDeclarationExpression(typeof(bool), "backTrackAssoicationOnEquality"));
			ctor2.BaseConstructorArgs.Add(new CodeVariableReferenceExpression("name"));
			ctor2.BaseConstructorArgs.Add(new CodeVariableReferenceExpression("assoicationPath"));
			ctor2.BaseConstructorArgs.Add(new CodeVariableReferenceExpression("backTrackAssoicationOnEquality"));
			innerClass.Members.Add(ctor2);

			parent.Members.Add(innerClass);
			return innerClass;
		}

		/// <summary>
		/// Gets the name of the next generic parameter.
		/// This is just to make sure that we don't have duplicate names.
		/// </summary>
		/// <returns></returns>
		private string GetGenericParameterName()
		{
			return "T" + genericTypeNamesRequested++;
		}

		/// <summary>
		/// Generates the property for query, e.g:
		/// Queries.GetAllEmployeeByName
		/// </summary>
		private static void GeneratePropertyForQuery(XmlNode queryNode, CodeNamespace ns)
		{
			string name = GetName(queryNode);
			CodeTypeDeclaration innerClass = new CodeTypeDeclaration("Queries");
			innerClass.IsPartial = true;
			ns.Types.Add(innerClass);
			CodeMemberProperty prop = new CodeMemberProperty();
			prop.Attributes = MemberAttributes.Static | MemberAttributes.Public;
			prop.Name = name;
			prop.Type = new CodeTypeReference(typeof(string));
			prop.HasGet = true;
			prop.HasSet = false;
			prop.GetStatements.Add(new CodeMethodReturnStatement(new CodePrimitiveExpression(name)));
			innerClass.Members.Add(prop);
		}

		/// <summary>
		/// This make sure that all the properties are created. Note that it is a template method
		/// which is used with various parameters to generate many different types of properties.
		/// Also, this is mostly pass through method, since we usually just use it for iterating over the properties.
		/// </summary>
		private void GenerateProperties(
			string prefix,
			string genericTypeName,
			AssoicationBehavior assoicationBehavior,
			string propertyType,
			XmlNode classNode,
			CodeTypeDeclaration innerClass,
			params string[] props)
		{
			foreach (string prop in props)
			{
				foreach (XmlNode propertyNode in classNode.SelectNodes(prop, nsMgr))
				{
					string type = null;
					XmlAttribute clazzAttribute = propertyNode.Attributes["class"];
					if (clazzAttribute != null)
						type = clazzAttribute.Value;
					GenerateProperty(prefix,
									 genericTypeName,
									 innerClass,
									 GetName(propertyNode),
									 propertyType,
									 type,
									 assoicationBehavior);
				}
			}
		}

		/// <summary>
		/// Generates the property, using the parameters passed.
		/// This is a complex issue, because we have many options here.
		/// The most important one is <param name="assoicationBehavior"/> which controls
		/// the way the assoication paths are used.
		/// If <param name="propertyType"/> is equals to <see cref="UseTheQueryClass"/> the Query_{0} idiom
		/// is used.
		/// </summary>
		private void GenerateProperty(
			string prefix,
			string genericTypeName,
			CodeTypeDeclaration innerClass,
			string name,
			string propertyType,
			string type,
			AssoicationBehavior assoicationBehavior)
		{
			if (propertyType == UseTheQueryClass)
				propertyType = "Query_" + GetTypeNameForDisplay(type);
			CodeMemberProperty prop = new CodeMemberProperty();
			prop.Attributes = MemberAttributes.Public;
			innerClass.Members.Add(prop);
			prop.Name = name;
			prop.HasGet = true;
			prop.HasSet = false;
			prop.Type = new CodeTypeReference(propertyType, new CodeTypeReference(genericTypeName));
			string propertyNameInGeneratedCode = prop.Name;
			if (prefix != null)
				propertyNameInGeneratedCode = prefix + "." + propertyNameInGeneratedCode;
			CodeObjectCreateExpression newExpr = new CodeObjectCreateExpression(prop.Type);

			CodeVariableDeclarationStatement var =
				new CodeVariableDeclarationStatement(typeof(string), "temp", new CodeVariableReferenceExpression("assoicationPath"));
			prop.GetStatements.Add(var);
			switch (assoicationBehavior)
			{
				case AssoicationBehavior.DoNotAdd:
					newExpr.Parameters.Add(new CodePrimitiveExpression(propertyNameInGeneratedCode));
					break;
				case AssoicationBehavior.AddAssoicationFromName:
					AddAssoicationPathFromExpression(prop, new CodePrimitiveExpression(name));
					newExpr.Parameters.Add(new CodePrimitiveExpression(propertyNameInGeneratedCode));
					break;
				case AssoicationBehavior.AddAssoicationHardCoded:
					newExpr.Parameters.Add(new CodePrimitiveExpression(name));
					AddAssoicationPathFromExpression(prop, new CodePrimitiveExpression(name));
					break;
			}
			newExpr.Parameters.Add(new CodeVariableReferenceExpression("temp"));
			if (assoicationBehavior != AssoicationBehavior.DoNotAdd)
				newExpr.Parameters.Add(new CodePrimitiveExpression(true));
			prop.GetStatements.Add(new CodeMethodReturnStatement(newExpr));
		}

		/// <summary>
		/// Adds the assoication path from the given expression. Including conditional handling if needed
		/// </summary>
		private static void AddAssoicationPathFromExpression(
			CodeMemberProperty prop, CodeExpression addedExpression)
		{
			CodeBinaryOperatorExpression assoicationExpression =
				new CodeBinaryOperatorExpression(
					new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("temp"),
													 CodeBinaryOperatorType.Add,
													 new CodePrimitiveExpression(".")),
					CodeBinaryOperatorType.Add,
					addedExpression);
			CodeAssignStatement assignStatement =
				new CodeAssignStatement(new CodeVariableReferenceExpression("temp"), assoicationExpression);
			prop.GetStatements.Add(assignStatement);
		}

		private string GetTypeNameForCode(XmlNode classNode)
		{
			string typeName = GetName(classNode).Split(',')[0];
			bool hasNamespace = typeName.Contains(".");
			typeName = typeName.Replace('+', '.'); //inner classes
			if (hbmCodeNameSpace == null || hasNamespace)
				return typeName;
			return hbmCodeNameSpace + "." + typeName;
		}

		private static string GetTypeNameForDisplay(XmlNode classNode)
		{
			string typeName = GetName(classNode);
			return GetTypeNameForDisplay(typeName);
		}

		private static string GetTypeNameForDisplay(string typeName)
		{
			int firstIndexOfComma = typeName.IndexOf(',');
			if (firstIndexOfComma < 0 && typeName.IndexOf('.') < 0)
			{
				return typeName;
			}
			int lastIndexOfPeriod = typeName.LastIndexOf('.', firstIndexOfComma) + 1;
			if (lastIndexOfPeriod == -1)
				lastIndexOfPeriod = 0;
			return typeName.Substring(lastIndexOfPeriod, firstIndexOfComma - lastIndexOfPeriod);
		}

		/// <summary>
		/// Gets the name, we assume that this is safe, since we have schema validation
		/// </summary>
		private static string GetName(XmlNode node)
		{
			if (node.Attributes["name"] == null)//this may happen if the <id> node doesn't have a name
			{
				throw new InvalidOperationException("Can't find attribute 'name' on element " + node.Name);
			}
			return node.Attributes["name"].Value;
		}
	}
}