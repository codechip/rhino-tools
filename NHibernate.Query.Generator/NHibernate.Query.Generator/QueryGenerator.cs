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
		string[] classesXpath = {
			                        	"/nh:hibernate-mapping//nh:class", "/nh:hibernate-mapping//nh:joined-subclass",
			                        	"/nh:hibernate-mapping//nh:subclass"
			                        };
		string[] compnentsPath = { "nh:component", "nh:dynamic-component", "nh:composite-id" };

		string[] projectByProperties = { "nh:property", "nh:key-property", };
		string[] groupableProperties = { "nh:property", "nh:key-property", };
		
		private const string UseTheQueryClass = "UseTheQueryClass";
		private string[] numericTypeNames = new string[] { "int", "integer", "smallint", "bigint", "tinyint", "decimal"
		                                                   , "float", "double", "short", "long", "big_decimal", "Int32"
		                                                   , "Int64", "Int16", "Single", "Decimal", "Double"
		                                                   , "System.Int32", "System.Int64", "System.Int16"
		                                                   , "System.Single", "System.Decimal", "System.Double" };
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
			Array.Sort(numericTypeNames);
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

			CreateOrderBy(queryNameSpace, hbm);

			CreateQueries(queryNameSpace, hbm);

			CreateProjectBy(queryNameSpace, hbm);

			CreateGroupBy(queryNameSpace, hbm);

			_provider.GenerateCodeFromCompileUnit(unit, writer, new CodeGeneratorOptions());
		}

		private void CreateGroupBy(CodeNamespace queryNameSpace, XmlDocument document)
		{
		
			CodeTypeDeclaration groupByDeclaration = new CodeTypeDeclaration("GroupBy");
			groupByDeclaration.IsPartial = true;
			queryNameSpace.Types.Add(groupByDeclaration);
			
			foreach(string classPath in classesXpath)
			{
				foreach(XmlNode node in document.SelectNodes(classPath, nsMgr))
				{
					CodeTypeDeclaration groupByForClass = GenerateGroupByForClass(node, false);
					AddBaseTypeIfNeeded(node, groupByForClass);
					AddComponentsGroupBy(node, groupByForClass);

					if (groupByForClass.Members.Count != 0)
                        AddToMembersWithSimpleXmlComment(groupByDeclaration,groupByForClass);
				}
			}
		}

		private void AddComponentsGroupBy(XmlNode node, CodeTypeDeclaration groupByForClass)
		{
			foreach (string componentPath in compnentsPath)
			{
				foreach (XmlNode componentNode in node.SelectNodes(componentPath, nsMgr))
				{
					CodeTypeDeclaration groupByForComponent = GenerateGroupByForClass(componentNode, true);
					groupByForComponent.IsPartial = true;
					//this is to handle the case of a component with a name, for instance, in a composite id 
					if (componentNode.Attributes["name"] == null)
					{
					    foreach (CodeTypeMember member in groupByForComponent.Members)
					    {
					        AddToMembersWithSimpleXmlComment(groupByForClass,member);
					    }
					}
					else if (groupByForComponent.Members.Count != 0)
					{
                        AddToMembersWithSimpleXmlComment(groupByForClass, groupByForComponent);
					}
				}
			}
		}

		private CodeTypeDeclaration GenerateGroupByForClass(XmlNode node, bool addPrefix)
		{
			//here we allow for a node without name
			string typeNameForDisplay;
			if (node.Attributes["name"] != null)
				typeNameForDisplay = GetTypeNameForDisplay(node);
			else
				typeNameForDisplay = "dummy";
			CodeTypeDeclaration groupableClassDeclaration = new CodeTypeDeclaration(typeNameForDisplay);
			groupableClassDeclaration.IsPartial = true;
			foreach (string groupableProperty in groupableProperties)
			{
				foreach (XmlNode propertyNode in node.SelectNodes(groupableProperty, nsMgr))
				{
					CodeMemberProperty prop = new CodeMemberProperty();
					prop.Name = GetName(propertyNode);
					prop.Attributes = MemberAttributes.Static | MemberAttributes.Public;
					prop.Type = new CodeTypeReference(typeof(IProjection).FullName);
					string associationPath = prop.Name;
					if (addPrefix)
						associationPath = typeNameForDisplay + "." + prop.Name;
					
					CodeExpression invokeExpression = new CodeMethodInvokeExpression(
						new CodeTypeReferenceExpression("NHibernate.Expression.Projections"),
						"GroupProperty", new CodePrimitiveExpression(associationPath));

					prop.GetStatements.Add(new CodeMethodReturnStatement(invokeExpression));
                    AddToMembersWithSimpleXmlComment(groupableClassDeclaration, prop);
				}
			}
			return groupableClassDeclaration;
		}
		
		private void CreateOrderBy(CodeNamespace queryNameSpace, XmlDocument document)
		{
			CodeTypeDeclaration orderByDeclaration = new CodeTypeDeclaration("OrderBy");
			orderByDeclaration.IsPartial = true;
			queryNameSpace.Types.Add(orderByDeclaration);

			foreach (string classPath in classesXpath)
			{
				foreach (XmlNode node in document.SelectNodes(classPath, nsMgr))
				{
					CodeTypeDeclaration orderByForClass = GenerateOrderByForClass(node, false);
					AddBaseTypeIfNeeded(node, orderByForClass);
					AddComponentsOrderBy(node, orderByForClass);

					if (orderByForClass.Members.Count != 0)
					{
                        AddToMembersWithSimpleXmlComment(orderByDeclaration, orderByForClass);
					}
				}
			}
		}

		private void AddBaseTypeIfNeeded(XmlNode node, CodeTypeDeclaration genClass)
		{
			string baseTypeForDiplay = GetBaseTypeForDiplay(node);
			if(baseTypeForDiplay!=null)
			{
				genClass.BaseTypes.Add(new CodeTypeReference(baseTypeForDiplay));
			}
		}

		private void AddComponentsOrderBy(XmlNode node, CodeTypeDeclaration orderByForClass)
		{
			foreach (string componentPath in compnentsPath)
			{
				foreach (XmlNode componentNode in node.SelectNodes(componentPath, nsMgr))
				{
					CodeTypeDeclaration orderByForComponent = GenerateOrderByForClass(componentNode, true);
					orderByForComponent.IsPartial = true;
					//this is to handle the case of a component with a name, for instance, in a composite id 
					if (componentNode.Attributes["name"] == null)
					{
					    foreach (CodeTypeMember member in orderByForComponent.Members)
					    {
					        AddToMembersWithSimpleXmlComment(orderByForClass,member);
					    }
					}
					else if (orderByForComponent.Members.Count != 0)
					{
                        AddToMembersWithSimpleXmlComment(orderByForClass, orderByForComponent);
					}
				}
			}
		}

		private CodeTypeDeclaration GenerateOrderByForClass(XmlNode node, bool addPrefix)
		{
			string[] orderableProperties = {"nh:property", "nh:key-property",};
			//here we allow for a node without name
			string typeNameForDisplay;
			if (node.Attributes["name"] != null)
				typeNameForDisplay = GetTypeNameForDisplay(node);
			else
				typeNameForDisplay = "dummy";
			CodeTypeDeclaration orderableClassDeclaration = new CodeTypeDeclaration(typeNameForDisplay);
			orderableClassDeclaration.IsPartial = true;
			foreach (string orderableProperty in orderableProperties)
			{
				foreach (XmlNode propertyNode in node.SelectNodes(orderableProperty, nsMgr))
				{
					CodeMemberProperty prop = new CodeMemberProperty();
					prop.Name = GetName(propertyNode);
					prop.Attributes = MemberAttributes.Static | MemberAttributes.Public;
					prop.Type = new CodeTypeReference("Query.OrderByClause");
					string associationPath = prop.Name;
					if (addPrefix)
						associationPath = typeNameForDisplay + "." + prop.Name;
					CodeObjectCreateExpression create =
						new CodeObjectCreateExpression("Query.OrderByClause", new CodePrimitiveExpression(associationPath));
					prop.GetStatements.Add(new CodeMethodReturnStatement(create));
                    AddToMembersWithSimpleXmlComment(orderableClassDeclaration, prop);
				}
			}
			return orderableClassDeclaration;
		}


		private void ValidateAndLoadXmlDocument()
		{
			XmlReaderSettings settings = new XmlReaderSettings();
			settings.ValidationType = ValidationType.Schema;
			settings.Schemas.Add(
				XmlSchema.Read(typeof (NHibernateUtil).Assembly.GetManifestResourceStream("NHibernate.nhibernate-mapping.xsd"),
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

		private void CreateProjectBy(CodeNamespace queryNameSpace, XmlDocument document)
		{
			CodeTypeDeclaration projectByDeclaration = new CodeTypeDeclaration("ProjectBy");
			projectByDeclaration.IsPartial = true;
			queryNameSpace.Types.Add(projectByDeclaration);

			foreach (string classPath in classesXpath)
			{
				foreach (XmlNode node in document.SelectNodes(classPath, nsMgr))
				{
					CodeTypeDeclaration projectByForClass = GenerateProjectByForClass(node, false);
					AddBaseTypeIfNeeded(node, projectByForClass);
					
					AddComponentsProjectBy(node, projectByForClass);

					if (projectByForClass.Members.Count != 0)
					{
                        AddToMembersWithSimpleXmlComment(projectByDeclaration, projectByForClass);
					}
				}
			}
		}

		private void AddComponentsProjectBy(XmlNode node, CodeTypeDeclaration projectByForClass)
		{
			foreach (string componentPath in compnentsPath)
			{
				foreach (XmlNode componentNode in node.SelectNodes(componentPath, nsMgr))
				{
					CodeTypeDeclaration projectByForComponent = GenerateProjectByForClass(componentNode, true);
					projectByForComponent.IsPartial = true;
					//this is to handle the case of a component with a name, for instance, in a composite id 
					if (componentNode.Attributes["name"] == null)
					{
                        foreach (CodeTypeMember member in projectByForComponent.Members)
					    {
					        AddToMembersWithSimpleXmlComment(projectByForClass,member);
					    }
					}
					else if (projectByForComponent.Members.Count != 0)
					{
                        AddToMembersWithSimpleXmlComment(projectByForClass,projectByForComponent);
					}
				}
			}
		}
		
		private CodeTypeDeclaration GenerateProjectByForClass(XmlNode node, bool addPrefix)
		{
			//here we allow for a node without name
			string typeNameForDisplay;
			if (node.Attributes["name"] != null)
				typeNameForDisplay = GetTypeNameForDisplay(node);
			else
				typeNameForDisplay = "dummy";
			CodeTypeDeclaration projectByForClassDeclaration = new CodeTypeDeclaration(typeNameForDisplay);
			projectByForClassDeclaration.IsPartial = true;
			foreach (string projectableProperty in projectByProperties)
			{
				foreach (XmlNode propertyNode in node.SelectNodes(projectableProperty, nsMgr))
				{
					bool isNumeric = false;
					if (propertyNode.Attributes["type"] != null)
					{
						string nodeTypeName = propertyNode.Attributes["type"].Value;
						isNumeric = (Array.BinarySearch(numericTypeNames, nodeTypeName) >= 0);
					}
					string propBuilderType;
					if (isNumeric)
						propBuilderType = "Query.NumericPropertyProjectionBuilder";
					else
						propBuilderType = "Query.PropertyProjectionBuilder";
					
					CodeMemberProperty prop = new CodeMemberProperty();
					prop.Name = GetName(propertyNode);
					prop.Attributes = MemberAttributes.Static | MemberAttributes.Public;
					prop.Type = new CodeTypeReference(propBuilderType);
					string associationPath = prop.Name;
					if (addPrefix)
						associationPath = typeNameForDisplay + "." + prop.Name;
					CodeObjectCreateExpression create =
						new CodeObjectCreateExpression(propBuilderType,
														 new CodePrimitiveExpression(associationPath));
					prop.GetStatements.Add(new CodeMethodReturnStatement(create));
                    AddToMembersWithSimpleXmlComment(projectByForClassDeclaration,prop);
				}
			}
			return projectByForClassDeclaration;	
		}
		
		/// <summary>
		/// This method is here because I want to be able to easily control what GenerateClasses
		/// will process. In the past, GenerateClasses was also recursive.
		/// </summary>
		private void CreateClasses(CodeTypeDeclaration whereTypeDeclaration)
		{
			GenerateClasses(whereTypeDeclaration,
			                hbm);
		}

		/// <summary>
		/// Generates the classes, for each class, a root class and a query class are generated
		/// </summary>
		private void GenerateClasses(CodeTypeDeclaration parent, XmlNode node)
		{
			foreach (string xpathForClass in classesXpath)
			{
				foreach (XmlNode classNode in node.SelectNodes(xpathForClass, nsMgr))
				{
					string genericTypeName = GetTypeNameForCode(GetName(classNode));
					string typeNameForDisplay = GetTypeNameForDisplay(classNode);
					string typeExtendsForDisplay = GetBaseTypeForDiplay(classNode);

					// This creates the query class Query_Blog<T2>
					CodeTypeDeclaration innerClass = CreateQueryClassInParentClass(parent, typeNameForDisplay, typeExtendsForDisplay);
					CreateChildProperties(classNode,
					                      innerClass,
					                      AssociationBehavior.AddAssociationFromName,
					                      innerClass.TypeParameters[0].Name);

					//This creates the root query class, Where.Blog
					CreateRootClassAndPropertyInParentClass(parent, typeNameForDisplay, genericTypeName);
				}
			}
		}

		/// <summary>
		/// This generate the properties of a query class (or the root class)
		/// </summary>
		private void CreateChildProperties(
			XmlNode classNode, CodeTypeDeclaration innerClass, AssociationBehavior associationBehavior, string genericName)
		{
			// generate full object query for simple properties
			GenerateProperties(null,
			                   genericName,
			                   AssociationBehavior.DoNotAdd,
			                   "Query.PropertyQueryBuilder",
			                   classNode,
			                   innerClass,
			                   "nh:property");

			// generate simple equality for id
			GenerateProperties(null,
			                   genericName,
			                   AssociationBehavior.DoNotAdd,
			                   "Query.QueryBuilder",
			                   classNode,
			                   innerClass,
			                   "nh:id");
			// generate reference to related query obj
			GenerateProperties( null, genericName, associationBehavior, UseTheQueryClass, classNode, innerClass, "nh:many-to-one", "nh:one-to-one" );

			// generate reference to component
			GenerateComponents(genericName, innerClass, classNode, "nh:component", "nh:dynamic-component");

			GenerateCompositeId(genericName, innerClass, associationBehavior, classNode, "nh:composite-id");
		}

		private void GenerateCompositeId(
			string genericName,
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
						prefix = GetName(idNode);
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
									GetName(classNode),
									GetName(idNode)));
						}
						string typeNameForDisplay = GetTypeNameForDisplay(idNode);
						idClass = CreateQueryClassInParentClass(innerClass, typeNameForDisplay, null);
						CreatePropertyInParentClass(genericTypeName,
						                            idClass,
						                            GetName(idNode),
						                            innerClass,
						                            new CodeVariableReferenceExpression("associationPath"));
					}

					// generate full object query for simple properties
					GenerateProperties(prefix,
					                   genericTypeName,
					                   AssociationBehavior.DoNotAdd,
					                   "Query.PropertyQueryBuilder",
					                   idNode,
					                   idClass,
					                   "nh:key-property");

					// generate reference to related query obj
					GenerateProperties(prefix,
					                   genericTypeName,
					                   associationBehavior,
					                   UseTheQueryClass,
					                   idNode,
					                   idClass,
					                   "nh:key-many-to-one");
				}
			}
		}

		private void GenerateComponents(
			string genericParameterName, CodeTypeDeclaration parent, XmlNode node, params string[] componentPath)
		{
			foreach (string xpathForClass in componentPath)
			{
				foreach (XmlNode classNode in node.SelectNodes(xpathForClass, nsMgr))
				{
					string name = GetName(classNode);
					CodeTypeDeclaration innerClass = CreateQueryClassInParentClass(parent, name, null);


					CreatePropertyInParentClass(genericParameterName, innerClass, name, parent, new CodePrimitiveExpression(null));

					//create full object query object
					GenerateProperties(name,
					                   genericParameterName,
					                   AssociationBehavior.DoNotAdd,
					                   "Query.PropertyQueryBuilder",
					                   classNode,
					                   innerClass,
					                   "nh:property",
					                   "nh:id");
					// create reference query obj
					GenerateProperties(name,
					                   genericParameterName,
					                   AssociationBehavior.AddAssociationFromName,
					                   UseTheQueryClass,
					                   classNode,
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
			CodeExpression associationExpression)
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
            AddToMembersWithSimpleXmlComment(parent, prop);
		}

		/// <summary>
		/// Creates the root class and property in parent class.
		/// Note that the Root Class does not inherit from QueryBuilder and is not a generic class. 
		/// The idea of having both root class and a query class is to avoid the possiblity of:
		/// Where.Post.Gt()
		/// </summary>
		private CodeTypeDeclaration CreateRootClassAndPropertyInParentClass(
			CodeTypeDeclaration parent, string display, string entityType)
		{
			// Root_Query_Blog
			CodeTypeDeclaration innerClass = new CodeTypeDeclaration("Root_Query_" + display);
			innerClass.BaseTypes.Add(new CodeTypeReference("Query_" + display, new CodeTypeReference(entityType)));
			innerClass.IsPartial = true;

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
            AddToMembersWithSimpleXmlComment(parent,innerClass);
            AddToMembersWithSimpleXmlComment(parent, field);
            AddToMembersWithSimpleXmlComment(parent, prop);

			//ctor
			CodeConstructor ctor = new CodeConstructor();
			ctor.Attributes = MemberAttributes.Public;

			ctor.BaseConstructorArgs.Add(new CodePrimitiveExpression("this"));
			ctor.BaseConstructorArgs.Add(new CodePrimitiveExpression(null));
            AddToMembersWithSimpleXmlComment(innerClass, ctor);
			return innerClass;
		}

		/// <summary>
		/// Creates the query class in parent class.
		/// The query class return a query object per each property in the persistant class.
		/// It is also keeping track of what is going on and is capable of tracking joins on the fly.
		/// This is done by combining the generated code and QueryBuilder
		/// </summary>
		private CodeTypeDeclaration CreateQueryClassInParentClass(CodeTypeDeclaration parent, string display, string extends)
		{
			// Query_Blog<T1> : Query.QueryBuilder<T1>
			CodeTypeDeclaration innerClass = new CodeTypeDeclaration("Query_" + display);
			innerClass.IsPartial = true;
			string genericParameterName = GetGenericParameterName();
			innerClass.TypeParameters.Add(genericParameterName);

			string classname;
			if (extends == null)
				classname = "Query.QueryBuilder";
			else
				classname = "Query_" + extends;

			innerClass.BaseTypes.Add(new CodeTypeReference(classname, new CodeTypeReference(genericParameterName)));


			// Query_Blog(string name, string associationPath) : QueryBuilder<T1>(name, associationPath);
			CodeConstructor ctor = new CodeConstructor();
			ctor.Attributes = MemberAttributes.Public;
			ctor.Parameters.Add(new CodeParameterDeclarationExpression(typeof (string), "name"));
			ctor.Parameters.Add(new CodeParameterDeclarationExpression(typeof (string), "associationPath"));
			ctor.BaseConstructorArgs.Add(new CodeVariableReferenceExpression("name"));
			ctor.BaseConstructorArgs.Add(new CodeVariableReferenceExpression("associationPath"));
            AddToMembersWithSimpleXmlComment(innerClass, ctor);

			// ctor for backtracking
			CodeConstructor ctor2 = new CodeConstructor();
			ctor2.Attributes = MemberAttributes.Public;
			ctor2.Parameters.Add(new CodeParameterDeclarationExpression(typeof (string), "name"));
			ctor2.Parameters.Add(new CodeParameterDeclarationExpression(typeof (string), "associationPath"));
			ctor2.Parameters.Add(new CodeParameterDeclarationExpression(typeof (bool), "backTrackAssociationOnEquality"));
			ctor2.BaseConstructorArgs.Add(new CodeVariableReferenceExpression("name"));
			ctor2.BaseConstructorArgs.Add(new CodeVariableReferenceExpression("associationPath"));
			ctor2.BaseConstructorArgs.Add(new CodeVariableReferenceExpression("backTrackAssociationOnEquality"));
            AddToMembersWithSimpleXmlComment(innerClass, ctor2);

            AddToMembersWithSimpleXmlComment(parent, innerClass);
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
			string name = GetName(queryNode);
			CodeTypeDeclaration innerClass = new CodeTypeDeclaration("Queries");
			innerClass.IsPartial = true;
			ns.Types.Add(innerClass);
			CodeMemberProperty prop = new CodeMemberProperty();
			prop.Attributes = MemberAttributes.Static | MemberAttributes.Public;
			prop.Name = name;
			prop.Type = new CodeTypeReference(typeof (string));
			prop.HasGet = true;
			prop.HasSet = false;
			prop.GetStatements.Add(new CodeMethodReturnStatement(new CodePrimitiveExpression(name)));
            AddToMembersWithSimpleXmlComment(innerClass, prop);
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
			CodeTypeDeclaration innerClass,
			params string[] props)
		{
			foreach (string prop in props)
			{
				foreach (XmlNode propertyNode in classNode.SelectNodes(prop, nsMgr))
				{
					string type = GetNodeClassType(propertyNode);
					GenerateProperty(prefix,
					                 genericTypeName,
					                 innerClass,
					                 GetName(propertyNode),
					                 propertyType,
					                 type,
					                 associationBehavior);
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
			AssociationBehavior associationBehavior)
		{
			if (propertyType == UseTheQueryClass)
				propertyType = "Query_" + GetTypeNameForDisplay(parentType);
			CodeMemberProperty prop = new CodeMemberProperty();
			prop.Attributes = MemberAttributes.Public;
            AddToMembersWithSimpleXmlComment(innerClass, prop);
			prop.Name = name;
			prop.HasGet = true;
			prop.HasSet = false;
			prop.Type = new CodeTypeReference(propertyType, new CodeTypeReference(genericTypeName));
			string propertyNameInGeneratedCode = prop.Name;
			if (prefix != null)
				propertyNameInGeneratedCode = prefix + "." + propertyNameInGeneratedCode;
			CodeObjectCreateExpression newExpr = new CodeObjectCreateExpression(prop.Type);

			CodeVariableDeclarationStatement var =
				new CodeVariableDeclarationStatement(typeof (string), "temp", new CodeVariableReferenceExpression("associationPath"));
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

		private string GetTypeNameForCode(string name)
		{
			string typeName = name.Split(',')[0];
			bool hasNamespace = typeName.Contains(".");
			typeName = typeName.Replace('+', '.'); //inner classes
			if (hbmCodeNameSpace == null || hasNamespace)
				return typeName;
			return hbmCodeNameSpace + "." + typeName;
		}

		private string GetTypeNameForDisplay(XmlNode classNode)
		{
			string typeName = GetName(classNode);
			return GetTypeNameForDisplay(typeName);
		}

		private string GetTypeNameForDisplay(string typeName)
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

		/// <summary>
		/// Gets the name, we assume that this is safe, since we have schema validation
		/// </summary>
		private static string GetName(XmlNode node)
		{
			if (node.Attributes["name"] == null) //this may happen if the <id> node doesn't have a name
			{
				throw new InvalidOperationException("Can't find attribute 'name' on element " + node.Name);
			}
			return node.Attributes["name"].Value;
		}

		private string GetBaseTypeForDiplay(XmlNode classNode)
		{
			string typeName = GetFullNameOfBaseClass(classNode);
			if (string.IsNullOrEmpty(typeName)) return null;
			return GetTypeNameForDisplay(typeName);
		}

		private static string GetFullNameOfBaseClass(XmlNode node)
		{
			string[] classes = {"class", "subclass", "joined-subclass"};
			if (node.Attributes["extends"] == null) ///does it extends anything explicitly?
			{
				if(Array.IndexOf(classes, node.ParentNode.Name)!=-1)
				{
					return GetName(node.ParentNode);
				}
				return null;
			}
			return node.Attributes["extends"].Value;
		}

        private void AddToMembersWithSimpleXmlComment(CodeTypeDeclaration typeDeclaration, CodeTypeMember member)
        {
            member.Comments.Add(new CodeCommentStatement(
                string.Format(@"Query for member {0}",  member.Name)
                , true));
            typeDeclaration.Members.Add(member);
        }
	}
}
