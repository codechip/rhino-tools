using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using NHibernate.Expression;

namespace NHibernate.Query.Generator
{
	/// <summary>
	/// Read an HBM file and generates a cs query file
	/// </summary>
	public class QueryGenerator
	{
		static readonly string[] classesXpath = {
			                        	"/nh:hibernate-mapping//nh:class", "/nh:hibernate-mapping//nh:joined-subclass",
			                        	"/nh:hibernate-mapping//nh:subclass"
			                        };
		static readonly string[] compnentsPath = { "nh:component", "nh:dynamic-component", "nh:composite-id" };
		static readonly string[] classes = { "class", "subclass", "joined-subclass" };

		//static readonly string[] orderableProperties = { "nh:property", "nh:key-property", "nh:id" };

		//static readonly string[] projectByProperties = { "nh:property", "nh:key-property", };
		//static readonly string[] groupableProperties = { "nh:property", "nh:key-property", };

		static readonly string UseTheQueryClass = "UseTheQueryClass";
		static readonly string[] numericTypeNames = new string[] { "int", "integer", "smallint", "bigint", "tinyint", "decimal"
		                                                   , "float", "double", "short", "long", "big_decimal", "Int32"
		                                                   , "Int64", "Int16", "Single", "Decimal", "Double"
		                                                   , "System.Int32", "System.Int64", "System.Int16"
		                                                   , "System.Single", "System.Decimal", "System.Double" };
		private readonly TextReader _reader;
		private CodeDomProvider _provider;
		private XmlDocument hbm = new XmlDocument();
		private XmlNamespaceManager nsMgr;
		private string hbmCodeNameSpace;
		private readonly string baseNamespace;

		/// <summary>
		/// This is used to avoid name collisions when generating the generic query types.
		/// </summary>
		private int genericTypeNamesRequested = 1;

		public QueryGenerator(TextReader reader, CodeDomProvider provider, string baseNamespace)
		{
			_reader = reader;
			_provider = provider;
			this.baseNamespace = baseNamespace;
			Array.Sort(numericTypeNames);
		}

		public CodeTypeDeclaration CreateDeclarationAndAddPartialType(CodeNamespace queryNamespace, string name)
		{
			CodeTypeDeclaration ctd = new CodeTypeDeclaration(name);
			ctd.IsPartial = true;
			queryNamespace.Types.Add(ctd);
			return ctd;
		}
		public void Generate(TextWriter writer)
		{
			//General CodeDOM setup
			CodeCompileUnit unit = new CodeCompileUnit();
			CodeNamespace queryNameSpace = new CodeNamespace("Query");
			unit.Namespaces.Add(queryNameSpace);
			
			CodeTypeDeclaration ctdFor = CreateDeclarationAndAddPartialType(queryNameSpace, "For");
			//CodeTypeDeclaration ctdFrom = CreateDeclarationAndAddPartialType(queryNameSpace, "From");
			CodeTypeDeclaration ctdWhere = CreateDeclarationAndAddPartialType(queryNameSpace, "Where");
			CodeTypeDeclaration ctdProject = CreateDeclarationAndAddPartialType(queryNameSpace, "ProjectBy");
			CodeTypeDeclaration ctdOrder = CreateDeclarationAndAddPartialType(queryNameSpace, "OrderBy");

			ValidateAndLoadXmlDocument();

			CreateQueryBuilders(ctdFor, "QueryBuilder");
			//CreateClassesOnlyForEntities(ctdFrom, "FromClause");
			CreateClasses(ctdWhere, new GeneratorPart("Where", "Where", "WhereClause", "WhereClause", "WhereClauseProperty", true, true));
			
			// Todo: Figure out how to isolate the numerics again, for now make everything numeric
			CreateClasses(ctdProject, new GeneratorPart("ProjectBy", "Projection", "ProjectionRoot", "ProjectionEntity", "ProjectionClauseProperty", true, true));
			CreateClasses(ctdOrder, new GeneratorPart("OrderBy", "OrderBy", "QueryPart", "QueryPart", "OrderByClauseProperty", false, false));
			CreateQueries(queryNameSpace, hbm);
			_provider.GenerateCodeFromCompileUnit(unit, writer, new CodeGeneratorOptions());
		}

		private void CreateClassesOnlyForEntities(CodeTypeDeclaration from, string s)
		{
			throw new NotImplementedException("Not implemented");
		}

		private void ValidateAndLoadXmlDocument()
		{
			XmlReaderSettings settings = new XmlReaderSettings();
			settings.ValidationType = ValidationType.Schema;
			settings.Schemas.Add(
				XmlSchema.Read(typeof(NHibernateUtil).Assembly.GetManifestResourceStream("NHibernate.nhibernate-mapping.xsd"),
											 null));
			hbm.Load(XmlReader.Create(_reader, settings));
			nsMgr = new XmlNamespaceManager(hbm.NameTable);
			nsMgr.AddNamespace("nh", "urn:nhibernate-mapping-2.2");

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
		private void CreateClasses(CodeTypeDeclaration ctd, GeneratorPart part)
		{
			GenerateClasses(ctd, hbm, part);
		}

		private void CreateQueryBuilders(CodeTypeDeclaration ctd, string className)
		{
			GenerateQueryBuilders(ctd, hbm, className);
		}

		private void GenerateQueryBuilders(CodeTypeDeclaration parent, XmlNode node, string className)
		{
			foreach (string xpathForClass in classesXpath)
			{
				foreach (XmlNode classNode in node.SelectNodes(xpathForClass, nsMgr))
				{
					CreateQueryBuilder(parent, ClassName.GetTypeNameForDisplay(classNode), ClassName.GetTypeNameForCode(hbmCodeNameSpace, ClassName.GetName(classNode)), className);
				}
			}
		}

		/// <summary>
		/// Generates the classes, for each class, a root class and a query class are generated
		/// </summary>
		private void GenerateClasses(CodeTypeDeclaration parent, XmlNode node, GeneratorPart part)
		{
			foreach (string xpathForClass in classesXpath)
			{
				foreach (XmlNode classNode in node.SelectNodes(xpathForClass, nsMgr))
				{
					ClassName className = new ClassName(hbmCodeNameSpace, classNode);

					// This creates the clause class ClauseName_Blog<T2>
					CodeTypeDeclaration innerClass = CreateClassInParentClass(parent, className, part);

					CreateChildProperties(classNode,
																innerClass,
																AssociationBehavior.AddAssociationFromName,
																innerClass.TypeParameters[0].Name, part);

					//This creates the root clause class, Where.Blog
					CreateRootClassAndPropertyInParentClass(parent, className, part);
				}
			}
		}

		/// <summary>
		/// This generate the properties of a query class (or the root class)
		/// </summary>
		private void CreateChildProperties(
			XmlNode classNode, CodeTypeDeclaration innerClass, AssociationBehavior associationBehavior, string genericName, GeneratorPart part)
		{
			// generate full object query for simple properties
			GenerateProperties(null,
												 genericName,
												 AssociationBehavior.DoNotAdd,
												 "Query." + part.PropertyClassName,
												 classNode,
												 part,
												 innerClass,
												 "nh:property");

			if (part.ClauseName.Equals("Where", StringComparison.CurrentCultureIgnoreCase))
			{
				// generate simple equality for id
				GenerateProperties(null,
													 genericName,
													 AssociationBehavior.DoNotAdd,
													 "Query." + part.EntityClassName,
													 classNode,
													 part,
													 innerClass,
													 "nh:id");
			}
			else
			{
				GenerateProperties(null,
									 genericName,
									 AssociationBehavior.DoNotAdd,
									 "Query." + part.PropertyClassName,
									 classNode,
									 part,
									 innerClass,
									 "nh:id");
			}

			// generate reference to related query obj
			GenerateProperties(null, genericName, associationBehavior, UseTheQueryClass, classNode, part, innerClass, "nh:many-to-one", "nh:one-to-one");

			// generate reference to component
			GenerateComponents(genericName, innerClass, part, classNode, "nh:component", "nh:dynamic-component");

			// generate composite id
			GenerateCompositeId(genericName, part, innerClass, associationBehavior, classNode, "nh:composite-id");
		}

		private void GenerateCompositeId(
			string genericName,
			GeneratorPart part,
			CodeTypeDeclaration innerClass,
			AssociationBehavior associationBehavior,
			XmlNode classNode,
			params string[] compositeIdXPath)
		{
			foreach (string xpathForClass in compositeIdXPath)
			{
				foreach (XmlNode idNode in classNode.SelectNodes(xpathForClass, nsMgr))
				{
					string prefix = null;
					string genericTypeName = genericName;
					if (idNode.Attributes["name"] != null)
					{
						prefix = ClassName.GetName(idNode);
					}

					CodeTypeDeclaration idClass = innerClass;
					if (idNode.Attributes["name"] != null) //anonymous composite id
					{
						XmlAttribute typeAttribute = idNode.Attributes["class"];
						if (typeAttribute == null)
						{
							throw new InvalidOperationException(
								string.Format(
									"On class {0} the composite id property {1} doesn't specify a type. NQG must know what the type of the property is in order to generate the query code.",
									ClassName.GetName(classNode),
									ClassName.GetName(idNode)));
						}
						ClassName idClassName = new ClassName(hbmCodeNameSpace, idNode);
						string typeNameForDisplay = ClassName.GetTypeNameForDisplay(idNode);
						idClass = CreateClassInParentClass(innerClass, idClassName, part);
						CreatePropertyInParentClass(genericTypeName,
																				idClass,
																				ClassName.GetName(idNode),
																				innerClass,
																				new CodeVariableReferenceExpression("QpAssociationPath"),
																				part.ClauseName);
					}

					// generate full object query for simple properties
					GenerateProperties(prefix,
														 genericTypeName,
														 AssociationBehavior.DoNotAdd,
														 "Query." + part.PropertyClassName,
														 idNode,
														 part,
														 idClass,
														 "nh:key-property");

					// generate reference to related query obj
					GenerateProperties(prefix,
														 genericTypeName,
														 associationBehavior,
														 UseTheQueryClass,
														 idNode,
														 part,
														 idClass,
														 "nh:key-many-to-one");
				}
			}
		}

		private void GenerateComponents(
			string genericParameterName, CodeTypeDeclaration parent, GeneratorPart part, XmlNode node, params string[] componentPath)
		{
			foreach (string xpathForClass in componentPath)
			{
				foreach (XmlNode classNode in node.SelectNodes(xpathForClass, nsMgr))
				{
					string name = ClassName.GetName(classNode);
					ClassName nameClass = new ClassName(hbmCodeNameSpace, classNode);
					CodeTypeDeclaration innerClass = CreateClassInParentClass(parent, nameClass, part);


					CreatePropertyInParentClass(genericParameterName, innerClass, name, parent, new CodePrimitiveExpression(null), part.ClauseName);

					//create full object query object
					GenerateProperties(name,
														 genericParameterName,
														 AssociationBehavior.DoNotAdd,
														 "Query." + part.PropertyClassName,
														 classNode,
														 part,
														 innerClass,
														 "nh:property",
														 "nh:id");
					// create reference query obj
					GenerateProperties(name,
														 genericParameterName,
														 AssociationBehavior.AddAssociationFromName,
														 UseTheQueryClass,
														 classNode,
														 part,
														 innerClass,
														 "nh:many-to-one",
														 "nh:one-to-one");
				}
			}
		}

		/// <summary>
		/// Create a property that return a new object each time it is called
		/// </summary>
		private void CreatePropertyInParentClass(
			string genericParameterName,
			CodeTypeDeclaration innerClass,
			string name,
			CodeTypeDeclaration parent,
			CodeExpression associationExpression,
			string clauseName)
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
						associationExpression)));
			AddToMembersWithSimpleXmlComment(parent, prop, clauseName);
		}

		private void CreateQueryBuilder(CodeTypeDeclaration parent, string display, string entityType, string className)
		{
			CodeMemberProperty prop = new CodeMemberProperty();
			prop.Name = display;
			prop.Type = new CodeTypeReference("Query." + className, new CodeTypeReference(entityType));
			prop.Attributes = MemberAttributes.Public | MemberAttributes.Static;
			prop.GetStatements.Add(new CodeMethodReturnStatement(new CodeObjectCreateExpression(prop.Type)));
			AddToMembersWithSimpleXmlComment(parent, prop, "Query Builder");
		}

		//public partial class From
		//{
		//  public static QueryBuilder<Contact> Contact { get { return new QueryBuilder<Contact>(); } }
		//  public static QueryBuilder<Task> Task { get { return new QueryBuilder<Task>(); } }
		//}

		/// <summary>
		/// Creates the root class and property in parent class.
		/// Note that the Root Class does not inherit from QueryBuilder and is not a generic class. 
		/// The idea of having both root class and a query class is to avoid the possiblity of:
		/// Where.Post.Gt()
		/// </summary>
		private CodeTypeDeclaration CreateRootClassAndPropertyInParentClass(
			CodeTypeDeclaration parent, ClassName className, GeneratorPart part)
		{
			// Root_Query_Blog
			CodeTypeDeclaration innerClass = new CodeTypeDeclaration("Root_" + part.ClauseName + "_" + className.TypeName);
			innerClass.BaseTypes.Add(new CodeTypeReference(part.ClauseName + "_" + className.TypeName, new CodeTypeReference(className.TypeNameForCode)));
			innerClass.IsPartial = true;

			// class Where { Root_Query_Blog  _query_Blog = new Root_Query_Blog(); }
			CodeMemberField field = new CodeMemberField();
			field.Name = "_root_" + part.ClauseName + "_" + className.TypeName;
			field.Type = new CodeTypeReference(innerClass.Name);
			field.Attributes = MemberAttributes.Static;
			CodeObjectCreateExpression createField = new CodeObjectCreateExpression(field.Type);
			field.InitExpression = createField;

			// proeprty 
			CodeMemberProperty prop = new CodeMemberProperty();
			prop.Name = className.TypeName;
			prop.Type = field.Type;
			prop.Attributes = MemberAttributes.Public | MemberAttributes.Static;
			prop.GetStatements.Add(new CodeMethodReturnStatement(new CodeFieldReferenceExpression(null, field.Name)));
			AddToMembersWithSimpleXmlComment(parent, innerClass, part.ClauseName);
			AddToMembersWithSimpleXmlComment(parent, field, part.ClauseName);
			AddToMembersWithSimpleXmlComment(parent, prop, part.ClauseName);

			//ctor
			CodeConstructor ctor = new CodeConstructor();
			ctor.Attributes = MemberAttributes.Public;

			ctor.BaseConstructorArgs.Add(new CodePrimitiveExpression("this"));
			ctor.BaseConstructorArgs.Add(new CodePrimitiveExpression(null));
			AddToMembersWithSimpleXmlComment(innerClass, ctor, part.ClauseName);
			return innerClass;
		}

		/// <summary>
		/// Creates the query class in parent class.
		/// The query class return a query object per each property in the persistant class.
		/// It is also keeping track of what is going on and is capable of tracking joins on the fly.
		/// This is done by combining the generated code and QueryBuilder
		/// </summary>
		private CodeTypeDeclaration CreateClassInParentClass(CodeTypeDeclaration parent, ClassName className, GeneratorPart part)
		{
			// ClauseName_Blog<T1> : Query.BaseClass<T1>
			CodeTypeDeclaration innerClass = new CodeTypeDeclaration(part.ClauseName + "_" + className.TypeName);
			innerClass.IsPartial = true;
			string genericParameterName = GetGenericParameterName();
			innerClass.TypeParameters.Add(genericParameterName);
			//Console.WriteLine(className.TypeName + " : " + parent.Name);
			string classname;
			if (className.ExtendsTypeName == null)
				classname = "Query." + part.EntityClassName;
			else
				classname = part.ClauseName + "_" + className.ExtendsTypeName;
			if (part.EntityIsGeneric || (className.ExtendsTypeName != null && className.ExtendsTypeName != part.EntityClassName))
				innerClass.BaseTypes.Add(new CodeTypeReference(classname, new CodeTypeReference(genericParameterName)));
			else
				innerClass.BaseTypes.Add(new CodeTypeReference(classname));


			// ClauseName_Blog(string name, string associationPath) : BaseClass<T1>(name, associationPath);
			CodeConstructor ctor = new CodeConstructor();
			ctor.Attributes = MemberAttributes.Public;
			ctor.Parameters.Add(new CodeParameterDeclarationExpression(typeof(string), "QpName"));
			ctor.Parameters.Add(new CodeParameterDeclarationExpression(typeof(string), "QpAssociationPath"));
			ctor.BaseConstructorArgs.Add(new CodeVariableReferenceExpression("QpName"));
			ctor.BaseConstructorArgs.Add(new CodeVariableReferenceExpression("QpAssociationPath"));
			AddToMembersWithSimpleXmlComment(innerClass, ctor, part.ClauseName);

			// ctor for backtracking
			CodeConstructor ctor2 = new CodeConstructor();
			ctor2.Attributes = MemberAttributes.Public;
			ctor2.Parameters.Add(new CodeParameterDeclarationExpression(typeof(string), "QpName"));
			ctor2.Parameters.Add(new CodeParameterDeclarationExpression(typeof(string), "QpAssociationPath"));
			ctor2.Parameters.Add(new CodeParameterDeclarationExpression(typeof(bool), "QpBackTrack"));
			ctor2.BaseConstructorArgs.Add(new CodeVariableReferenceExpression("QpName"));
			ctor2.BaseConstructorArgs.Add(new CodeVariableReferenceExpression("QpAssociationPath"));
			ctor2.BaseConstructorArgs.Add(new CodeVariableReferenceExpression("QpBackTrack"));
			AddToMembersWithSimpleXmlComment(innerClass, ctor2, part.ClauseName);

			AddToMembersWithSimpleXmlComment(parent, innerClass, part.ClauseName);
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
		private void GeneratePropertyForQuery(XmlNode queryNode, CodeNamespace ns)
		{
			string name = ClassName.GetName(queryNode);
			CodeTypeDeclaration innerClass = new CodeTypeDeclaration("Queries");
			innerClass.IsPartial = true;
			ns.Types.Add(innerClass);
			CodeMemberProperty prop = new CodeMemberProperty();
			prop.Attributes = MemberAttributes.Static | MemberAttributes.Public;
			prop.Name = name.Replace(".", "_");
			prop.Type = new CodeTypeReference(typeof(string));
			prop.HasGet = true;
			prop.HasSet = false;
			prop.GetStatements.Add(new CodeMethodReturnStatement(new CodePrimitiveExpression(name)));
			AddToMembersWithSimpleXmlComment(innerClass, prop, "Named Query");
		}

		/// <summary>
		/// This make sure that all the properties are created. Note that it is a template method
		/// which is used with various parameters to generate many different types of properties.
		/// Also, this is mostly pass through method, since we usually just use it for iterating over the properties.
		/// </summary>
		private void GenerateProperties(
			string prefix,
			string genericTypeName,
			AssociationBehavior associationBehavior,
			string propertyType,
			XmlNode classNode,
			GeneratorPart part,
			CodeTypeDeclaration innerClass,
			params string[] props)
		{
			foreach (string prop in props)
			{
				foreach (XmlNode propertyNode in classNode.SelectNodes(prop, nsMgr))
				{
					string propBuilderType = propertyType;
					part.IsNumeric = false;
					if (propertyNode.Attributes["type"] != null)
					{
						string nodeTypeName = propertyNode.Attributes["type"].Value;
						part.IsNumeric = (Array.BinarySearch(numericTypeNames, nodeTypeName) >= 0);
					}
					if (part.ClauseName.Equals("Projection"))
					{
						if (part.IsNumeric)
						{
							propBuilderType = "Query.ProjectionClausePropertyNumeric";
						}
					}
					string type = GetNodeClassType(propertyNode);
					GenerateProperty(prefix,
													 genericTypeName,
													 innerClass,
													 ClassName.GetName(propertyNode),
													 propBuilderType,
													 type,
													 associationBehavior, part);
				}
			}
		}

		private static string GetNodeClassType(XmlNode propertyNode)
		{
			string type = null;
			XmlAttribute clazzAttribute = propertyNode.Attributes["class"];
			if (clazzAttribute != null)
				type = clazzAttribute.Value;
			return type;
		}

		/// <summary>
		/// Generates the property, using the parameters passed.
		/// This is a complex issue, because we have many options here.
		/// The most important one is <param name="associationBehavior"/> which controls
		/// the way the association paths are used.
		/// If <param name="propertyType"/> is equals to <see cref="UseTheQueryClass"/> the Query_{0} idiom
		/// is used.
		/// </summary>
		private void GenerateProperty(
			string prefix,
			string genericTypeName,
			CodeTypeDeclaration innerClass,
			string name,
			string propertyType,
			string parentType,
			AssociationBehavior associationBehavior, GeneratorPart part)
		{
			if (propertyType == UseTheQueryClass)
				propertyType = part.ClauseName + "_" + ClassName.GetTypeNameForDisplay(parentType);
			CodeMemberProperty prop = new CodeMemberProperty();
			prop.Attributes = MemberAttributes.Public;
			AddToMembersWithSimpleXmlComment(innerClass, prop, part.ClauseName);
			prop.Name = name;
			prop.HasGet = true;
			prop.HasSet = false;
			prop.Type = new CodeTypeReference(propertyType, new CodeTypeReference(genericTypeName));
			string propertyNameInGeneratedCode = prop.Name;
			if (prefix != null)
				propertyNameInGeneratedCode = prefix + "." + propertyNameInGeneratedCode;
			CodeObjectCreateExpression newExpr = new CodeObjectCreateExpression(prop.Type);

			CodeVariableDeclarationStatement var =
				new CodeVariableDeclarationStatement(typeof(string), "temp", new CodeVariableReferenceExpression("QpAssociationPath"));
			prop.GetStatements.Add(var);
			switch (associationBehavior)
			{
				case AssociationBehavior.DoNotAdd:
					newExpr.Parameters.Add(new CodePrimitiveExpression(propertyNameInGeneratedCode));
					break;
				case AssociationBehavior.AddAssociationFromName:
					AddAssociationPathFromExpression(prop, new CodePrimitiveExpression(name));
					newExpr.Parameters.Add(new CodePrimitiveExpression(propertyNameInGeneratedCode));
					break;
				case AssociationBehavior.AddAssociationHardCoded:
					newExpr.Parameters.Add(new CodePrimitiveExpression(name));
					AddAssociationPathFromExpression(prop, new CodePrimitiveExpression(name));
					break;
			}
			newExpr.Parameters.Add(new CodeVariableReferenceExpression("temp"));
			if (associationBehavior != AssociationBehavior.DoNotAdd)
				newExpr.Parameters.Add(new CodePrimitiveExpression(true));
			prop.GetStatements.Add(new CodeMethodReturnStatement(newExpr));
		}

		/// <summary>
		/// Adds the association path from the given expression. Including conditional handling if needed
		/// </summary>
		private static void AddAssociationPathFromExpression(CodeMemberProperty prop, CodeExpression addedExpression)
		{
			CodeBinaryOperatorExpression associationExpression =
				new CodeBinaryOperatorExpression(
					new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("temp"),
																					 CodeBinaryOperatorType.Add,
																					 new CodePrimitiveExpression(".")),
					CodeBinaryOperatorType.Add,
					addedExpression);
			CodeAssignStatement assignStatement =
				new CodeAssignStatement(new CodeVariableReferenceExpression("temp"), associationExpression);
			prop.GetStatements.Add(assignStatement);
		}





		private static void AddToMembersWithSimpleXmlComment(CodeTypeDeclaration typeDeclaration, CodeTypeMember member, string clauseName)
		{
			member.Comments.Add(new CodeCommentStatement(
					string.Format(@"{1} for member {0}", member.Name, clauseName)
					, true));
			typeDeclaration.Members.Add(member);
		}
	}
}
