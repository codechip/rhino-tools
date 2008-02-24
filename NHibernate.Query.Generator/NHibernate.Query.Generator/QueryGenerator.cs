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
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using NHibernate.Expressions;
using NHibernate.SqlCommand;

namespace NHibernate.Query.Generator
{
	using System.Collections.Generic;

	/// <summary>
    /// Read an HBM file and generates a cs query file
    /// </summary>
    public class QueryGenerator
    {
        private static readonly string[] classesXpath = {
                                                            "/nh:hibernate-mapping//nh:class",
                                                            "/nh:hibernate-mapping//nh:joined-subclass",
                                                            "/nh:hibernate-mapping//nh:subclass"
                                                        };

        private static readonly string[] compnentsPath = { "nh:component", "nh:dynamic-component", "nh:composite-id" };
        private static readonly string[] classes = { "class", "subclass", "joined-subclass" };

        private static readonly string[] orderableProperties = { "nh:property", "nh:key-property", "nh:id" };

        private static readonly string[] projectByProperties = { "nh:property", "nh:key-property", "nh:id" };
        private static readonly string[] groupableProperties = { "nh:property", "nh:key-property", "nh:id" };

        private static readonly string UseTheQueryClass = "UseTheQueryClass";

        private static readonly string[] numericTypeNames = new string[]
            {
                "int", "integer", "smallint", "bigint", "tinyint", "decimal"
                , "float", "double", "short", "long", "big_decimal", "Int32"
                , "Int64", "Int16", "Single", "Decimal", "Double"
                , "System.Int32", "System.Int64", "System.Int16"
                , "System.Single", "System.Decimal", "System.Double"
            };

        private readonly TextReader _reader;
        private readonly CodeDomProvider _provider;
        private readonly string baseNamespace;
        private readonly string import;
        private readonly XmlDocument hbm = new XmlDocument();
        private XmlNamespaceManager nsMgr;
        private string hbmCodeNameSpace;

        /// <summary>
        /// This is used to avoid name collisions when generating the generic query types.
        /// </summary>
        private int genericTypeNamesRequested = 1;

        public QueryGenerator(TextReader reader, CodeDomProvider provider, string baseNamespace, string import)
        {
            _reader = reader;
            _provider = provider;
            this.baseNamespace = baseNamespace;
            this.import = import;
            Array.Sort(numericTypeNames);
        }

        public void Generate(TextWriter writer)
        {
            //General CodeDOM setup
            CodeCompileUnit unit = new CodeCompileUnit();

            CodeNamespace queryNameSpace = new CodeNamespace(baseNamespace);
            GenerateVersionComment(queryNameSpace);
            unit.Namespaces.Add(queryNameSpace);

            if (import != null)
            {
                CodeNamespaceImport nsImport = new CodeNamespaceImport(import);
                queryNameSpace.Imports.Add(nsImport);
            }


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

        /// <summary>
        /// method to create a timestamp and version comment
        /// </summary>
        /// <param name="codeNamespace"></param>
        private void GenerateVersionComment(System.CodeDom.CodeNamespace codeNamespace)
        {
            codeNamespace.Comments.Add(new CodeCommentStatement(string.Empty));
            codeNamespace.Comments.Add(new CodeCommentStatement(System.DateTime.Now.ToString()));
            codeNamespace.Comments.Add(new CodeCommentStatement(String.Format(
                                       "This source code was auto-generated by {0} ({1}), Version {2}.",
                                       GetAssemblyInfo.AssemblyName,
                                       GetAssemblyInfo.AssemblyProduct,
                                       GetAssemblyInfo.AssemblyVersion)));
            codeNamespace.Comments.Add(new CodeCommentStatement(string.Empty));

        }

        private void CreateGroupBy(CodeNamespace queryNameSpace, XmlDocument document)
        {
            CodeTypeDeclaration groupByDeclaration = new CodeTypeDeclaration("GroupBy");
            groupByDeclaration.IsPartial = true;
            queryNameSpace.Types.Add(groupByDeclaration);

            foreach (string classPath in classesXpath)
            {
                foreach (XmlNode node in document.SelectNodes(classPath, nsMgr))
                {
                    CodeTypeDeclaration groupByForClass = GenerateGroupByForClass(node, false, "");
                    AddBaseTypeIfNeeded(node, groupByForClass);
                    AddComponentsGroupBy(node, groupByForClass, "");

                    if (groupByForClass.Members.Count != 0)
                        AddToMembersWithSimpleXmlComment(groupByDeclaration, groupByForClass);
                }
            }
        }

        private void AddComponentsGroupBy(XmlNode node, CodeTypeDeclaration groupByForClass, string prefix)
        {
            foreach (string componentPath in compnentsPath)
            {
                foreach (XmlNode componentNode in node.SelectNodes(componentPath, nsMgr))
                {
                    CodeTypeDeclaration groupByForComponent = GenerateGroupByForClass(componentNode, true, prefix);
                    groupByForComponent.IsPartial = true;
                    //this is to handle the case of a component with a name, for instance, in a composite id 
                    if (componentNode.Attributes["name"] == null)
                    {
                        foreach (CodeTypeMember member in groupByForComponent.Members)
                        {
                            AddToMembersWithSimpleXmlComment(groupByForClass, member);
                        }
                    }
                    else if (groupByForComponent.Members.Count != 0)
                    {
                        string newPrefix = GetPath(prefix, GetName(componentNode));

                        AddToMembersWithSimpleXmlComment(groupByForClass, groupByForComponent);

                        AddComponentsGroupBy(componentNode, groupByForComponent, newPrefix);
                    }
                }
            }
        }

        private CodeTypeDeclaration GenerateGroupByForClass(XmlNode node, bool addPrefix, string prefix)
        {
            //here we allow for a node without name
            string typeNameForDisplay;
            if (node.Attributes["name"] != null)
                typeNameForDisplay = GetTypeNameForDisplay(node);
            else
                typeNameForDisplay = "dummy";
            CodeTypeDeclaration groupableClassDeclaration = new CodeTypeDeclaration(typeNameForDisplay);
			AddNotClsCompliant(groupableClassDeclaration);
            groupableClassDeclaration.IsPartial = true;
            if (addPrefix) prefix = GetPath(prefix, typeNameForDisplay);

            foreach (string groupableProperty in groupableProperties)
            {
                foreach (XmlNode propertyNode in node.SelectNodes(groupableProperty, nsMgr))
                {
                    CodeMemberProperty prop = new CodeMemberProperty();
                    prop.Name = GetName(propertyNode);
                    prop.Attributes = MemberAttributes.Static | MemberAttributes.Public;
                    prop.Type = new CodeTypeReference(typeof(IProjection).FullName);
                    string associationPath = GetPath(prefix, prop.Name);

                    CodeExpression invokeExpression = new CodeMethodInvokeExpression(
                        new CodeTypeReferenceExpression("NHibernate.Expressions.Projections"),
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
                    CodeTypeDeclaration orderByForClass = GenerateOrderByForClass(node, false, "");
                    AddBaseTypeIfNeeded(node, orderByForClass);
                    AddComponentsOrderBy(node, orderByForClass, "");

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
            if (baseTypeForDiplay != null)
            {
                genClass.BaseTypes.Add(new CodeTypeReference(baseTypeForDiplay));
            }
        }

        private void AddComponentsOrderBy(XmlNode node, CodeTypeDeclaration orderByForClass, string prefix)
        {
            foreach (string componentPath in compnentsPath)
            {
                foreach (XmlNode componentNode in node.SelectNodes(componentPath, nsMgr))
                {
                    CodeTypeDeclaration orderByForComponent = GenerateOrderByForClass(componentNode, true, prefix);
                    orderByForComponent.IsPartial = true;
                    //this is to handle the case of a component with a name, for instance, in a composite id 
                    if (componentNode.Attributes["name"] == null)
                    {
                        foreach (CodeTypeMember member in orderByForComponent.Members)
                        {
                            AddToMembersWithSimpleXmlComment(orderByForClass, member);
                        }
                    }
                    else if (orderByForComponent.Members.Count != 0)
                    {
                        string newPrefix = GetPath(prefix, GetName(componentNode));

                        AddToMembersWithSimpleXmlComment(orderByForClass, orderByForComponent);

                        AddComponentsOrderBy(componentNode, orderByForComponent, newPrefix);
                    }
                }
            }
        }

        private CodeTypeDeclaration GenerateOrderByForClass(XmlNode node, bool addPrefix, string prefix)
        {
            //here we allow for a node without name
            string typeNameForDisplay;
            if (node.Attributes["name"] != null)
                typeNameForDisplay = GetTypeNameForDisplay(node);
            else
                typeNameForDisplay = "dummy";
            CodeTypeDeclaration orderableClassDeclaration = new CodeTypeDeclaration(typeNameForDisplay);
        	AddNotClsCompliant(orderableClassDeclaration);
            orderableClassDeclaration.IsPartial = true;
            if (addPrefix) prefix = GetPath(prefix, typeNameForDisplay);

            foreach (string orderableProperty in orderableProperties)
            {
                foreach (XmlNode propertyNode in node.SelectNodes(orderableProperty, nsMgr))
                {
                    CodeMemberProperty prop = new CodeMemberProperty();
                    prop.Name = GetName(propertyNode);
                    prop.Attributes = MemberAttributes.Static | MemberAttributes.Public;
                    prop.Type = new CodeTypeReference("OrderByClause");
                    string associationPath = GetPath(prefix, prop.Name);
                    CodeObjectCreateExpression create =
                        new CodeObjectCreateExpression("OrderByClause", new CodePrimitiveExpression(associationPath));
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
                XmlSchema.Read(
                    typeof(NHibernateUtil).Assembly.GetManifestResourceStream("NHibernate.nhibernate-mapping.xsd"),
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
                    CodeTypeDeclaration projectByForClass = GenerateProjectByForClass(node, false, "");
                    AddBaseTypeIfNeeded(node, projectByForClass);

                    AddComponentsProjectBy(node, projectByForClass, "");

                    if (projectByForClass.Members.Count != 0)
                    {
                        AddToMembersWithSimpleXmlComment(projectByDeclaration, projectByForClass);
                    }
                }
            }
        }

        private void AddComponentsProjectBy(XmlNode node, CodeTypeDeclaration projectByForClass, string prefix)
        {
            foreach (string componentPath in compnentsPath)
            {
                foreach (XmlNode componentNode in node.SelectNodes(componentPath, nsMgr))
                {
                    CodeTypeDeclaration projectByForComponent = GenerateProjectByForClass(componentNode, true, prefix);
                    projectByForComponent.IsPartial = true;
                    //this is to handle the case of a component with a name, for instance, in a composite id 
                    if (componentNode.Attributes["name"] == null)
                    {
                        foreach (CodeTypeMember member in projectByForComponent.Members)
                        {
                            AddToMembersWithSimpleXmlComment(projectByForClass, member);
                        }
                    }
                    else if (projectByForComponent.Members.Count != 0)
                    {
                        string newPrefix = GetPath(prefix, GetName(componentNode));

                        AddToMembersWithSimpleXmlComment(projectByForClass, projectByForComponent);

                        AddComponentsProjectBy(componentNode, projectByForComponent, newPrefix);
                    }
                }
            }
        }

        private CodeTypeDeclaration GenerateProjectByForClass(XmlNode node, bool addPrefix, string prefix)
        {
            //here we allow for a node without name
            string typeNameForDisplay;
            if (node.Attributes["name"] != null)
                typeNameForDisplay = GetTypeNameForDisplay(node);
            else
                typeNameForDisplay = "dummy";
            CodeTypeDeclaration projectByForClassDeclaration = new CodeTypeDeclaration(typeNameForDisplay);
        	AddNotClsCompliant(projectByForClassDeclaration);
            projectByForClassDeclaration.IsPartial = true;
            if (addPrefix) prefix = GetPath(prefix, typeNameForDisplay);

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
                        propBuilderType = "NumericPropertyProjectionBuilder";
                    else
                        propBuilderType = "PropertyProjectionBuilder";

                    CodeMemberProperty prop = new CodeMemberProperty();
                    prop.Name = GetName(propertyNode);
                    prop.Attributes = MemberAttributes.Static | MemberAttributes.Public;
                    prop.Type = new CodeTypeReference(propBuilderType);
                    string associationPath = GetPath(prefix, prop.Name);
                    CodeObjectCreateExpression create =
                        new CodeObjectCreateExpression(propBuilderType,
                                                       new CodePrimitiveExpression(associationPath));
                    prop.GetStatements.Add(new CodeMethodReturnStatement(create));
                    AddToMembersWithSimpleXmlComment(projectByForClassDeclaration, prop);
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
                		string typeName = GetName(classNode);
										string genericTypeName = GetTypeNameForCode(typeName);
                    string typeNameForDisplay = GetTypeNameForDisplay(classNode);
                    string typeExtendsForDisplay = GetBaseTypeForDiplay(classNode);

                    // This creates the query class Query_Blog<T2>
                    CodeTypeDeclaration innerClass =
                        CreateQueryClassInParentClass(parent, typeNameForDisplay, typeExtendsForDisplay);
                    CreateChildProperties(classNode,
                                          innerClass,
                                          AssociationBehavior.AddAssociationFromName,
                                          innerClass.TypeParameters[0].Name);

										if (!IsGenericType(typeName))
										{
											//This creates the root query class, Where.Blog
											CreateRootClassAndPropertyInParentClass(parent, typeNameForDisplay, genericTypeName);
										}
                }
            }
        }

				private static bool IsGenericType(string typeName)
				{
					return typeName.IndexOf('`') >= 0;
				}

        /// <summary>
        /// This generate the properties of a query class (or the root class)
        /// </summary>
        private void CreateChildProperties(
            XmlNode classNode, CodeTypeDeclaration innerClass, AssociationBehavior associationBehavior,
            string genericName)
        {
            // generate full object query for simple properties
            GenerateProperties(null,
                               genericName,
                               AssociationBehavior.DoNotAdd,
                               "PropertyQueryBuilder",
                               classNode,
                               innerClass,
                               "nh:property");

            // generate simple equality for id
            GenerateProperties(null,
                               genericName,
                               AssociationBehavior.DoNotAdd,
                               "IdQueryBuilder",
                               classNode,
                               innerClass,
                               "nh:id");

            // generate reference to related query obj
            GenerateProperties(null, genericName, associationBehavior, UseTheQueryClass, classNode, innerClass,
                               "nh:many-to-one", "nh:one-to-one");

            GenerateCollections(genericName, UseTheQueryClass, classNode, innerClass, "nh:set",
                                "nh:bag", "nh:list", "nh:idbag");

            // generate reference to component
            GenerateComponents(genericName, innerClass, classNode, "", "nh:component", "nh:dynamic-component");

            GenerateCompositeId(genericName, innerClass, associationBehavior, classNode, "nh:composite-id");
        }

        private void GenerateCollections(string genericTypeName, string propertyType, XmlNode classNode, CodeTypeDeclaration innerClass, params string[] props)
        {
            foreach (string xpathForClass in props)
            {
                foreach (XmlNode collectionNode in classNode.SelectNodes(xpathForClass, nsMgr))
                {
                    CodeTypeReference type = new CodeTypeReference("CollectionQueryBuilder");
                    type.TypeArguments.Add(genericTypeName);

                    string collectionName = collectionNode.Attributes["name"].Value;
                    
                    string collectionClassName = GetClassNameFromCollection(collectionNode);

                    //If it's composite-element, needs to generate query object for it todo: not always - check
                    if(collectionNode.SelectSingleNode("nh:composite-element/@class", nsMgr) != null)
                        GenerateCompositeElement(collectionNode, collectionClassName, innerClass);

                    if(collectionClassName==null)//not a node type we can handle
                        continue;
                    CodeTypeDeclaration collectionDerived =
                        new CodeTypeDeclaration("Query_Collection_" + collectionName);
                	AddNotClsCompliant(collectionDerived);
                    collectionDerived.BaseTypes.Add(type);

                    CodeConstructor ctor = new CodeConstructor();
                    ctor.Attributes = MemberAttributes.Public;
                    CodeTypeReference parentClass = new CodeTypeReference("QueryBuilder");
                    parentClass.TypeArguments.Add(genericTypeName);
                    ctor.Parameters.Add(new CodeParameterDeclarationExpression(parentClass, "parent"));
                    ctor.Parameters.Add(new CodeParameterDeclarationExpression(typeof(string), "name"));
                    ctor.Parameters.Add(new CodeParameterDeclarationExpression(typeof(string), "associationPath"));

                    ctor.BaseConstructorArgs.Add(new CodeVariableReferenceExpression("parent"));
                    ctor.BaseConstructorArgs.Add(new CodeVariableReferenceExpression("name"));
                    ctor.BaseConstructorArgs.Add(new CodeVariableReferenceExpression("associationPath"));

                    collectionDerived.Members.Add(ctor);

                    GenerateWithMethods(collectionDerived, innerClass, collectionClassName);

                    innerClass.Members.Add(collectionDerived);
                    CodeMemberProperty col = new CodeMemberProperty();
                    col.Name = collectionName;
                    col.Attributes = MemberAttributes.Public;
                    col.Type = new CodeTypeReference(collectionDerived.Name);
                    CodeMethodReturnStatement result = new CodeMethodReturnStatement();
                    CodeVariableDeclarationStatement var = new CodeVariableDeclarationStatement(
                        typeof(string), "temp",
                        new CodeVariableReferenceExpression("associationPath"));
                    CodeAssignStatement assign = new CodeAssignStatement(
                        new CodeVariableReferenceExpression("temp"),
                        new CodeBinaryOperatorExpression(
                            new CodeVariableReferenceExpression("temp"),
                            CodeBinaryOperatorType.Add, 
															(classNode.Name.Equals("component")) ? 
																new CodePrimitiveExpression("." +classNode.Attributes["name"].Value + "." + collectionName) :
																new CodePrimitiveExpression("."+collectionName))
                        );
                    col.GetStatements.Add(var);
                    col.GetStatements.Add(assign);
                    result.Expression = new CodeObjectCreateExpression(col.Type,
											(classNode.Name.Equals("component")) ? (CodeExpression)new CodeVariableReferenceExpression("myQueryParent") : (CodeExpression)new CodeThisReferenceExpression(),
                                                                       new CodePrimitiveExpression(col.Name),
                                                                       new CodeVariableReferenceExpression(var.Name));
                    col.GetStatements.Add(result);
                    innerClass.Members.Add(col);
                }
            }
        }

        private void GenerateCompositeElement(XmlNode collectionNode, string collectionClassName, CodeTypeDeclaration innerClass)
        {
            //note: quick hack (generate class)
                string genericCompositeTypeName = GetTypeNameForCode(collectionClassName);
                string typeNameForDisplay = GetTypeNameForDisplay(collectionClassName);
                string typeExtendsForDisplay = null;

                // This creates the query class Query_Blog<T2>
                CodeTypeDeclaration innerCompositeClass = CreateQueryClassInParentClass(innerClass, typeNameForDisplay, typeExtendsForDisplay);

                // generate full object query for simple properties
                GenerateProperties(null,
                                   genericCompositeTypeName,
                                   AssociationBehavior.DoNotAdd,
                                   "PropertyQueryBuilder",
                                   collectionNode,
                                   innerCompositeClass,
                                   "nh:property");

                // generate reference to related query obj
                GenerateProperties(null, genericCompositeTypeName, AssociationBehavior.AddAssociationFromName, UseTheQueryClass, collectionNode, innerCompositeClass,
                                   "nh:many-to-one", "nh:one-to-one");

                // generate reference to component
                GenerateComponents(genericCompositeTypeName, innerCompositeClass, collectionNode, "", "nh:component", "nh:dynamic-component");
                                                
                //This creates the root query class, Where.Blog
                CreateRootClassAndPropertyInParentClass(innerCompositeClass, typeNameForDisplay, genericCompositeTypeName);
            
        }

        private void GenerateWithMethods(CodeTypeDeclaration collectionDerived, CodeTypeDeclaration innerClass, string collectionClassName)
        {
            //With(JoinType joinType)
              BuildWithMethod(collectionDerived, innerClass, collectionClassName,
                               new CodeFieldReferenceExpression(
                                    new CodeTypeReferenceExpression(typeof(JoinType)), "InnerJoin"),
                               new CodeFieldReferenceExpression(
                                  new CodeTypeReferenceExpression(typeof(FetchMode)), "Default"));
          

            CodeMemberMethod withJoin =
                BuildWithMethod(collectionDerived, innerClass,collectionClassName,
                                new CodeVariableReferenceExpression("joinType"),
                                 new CodeFieldReferenceExpression(
                                    new CodeTypeReferenceExpression(typeof(FetchMode)), "Default"));
            withJoin.Parameters.Add(new CodeParameterDeclarationExpression(typeof(JoinType), "joinType"));

            CodeMemberMethod withFetch =
                BuildWithMethod(collectionDerived, innerClass, collectionClassName,
                                new CodeFieldReferenceExpression(
                                    new CodeTypeReferenceExpression(typeof(JoinType)), "InnerJoin"),
                                new CodeVariableReferenceExpression("fetchMode"));
            withFetch.Parameters.Add(new CodeParameterDeclarationExpression(typeof(FetchMode), "fetchMode"));

            CodeMemberMethod withFetchAndJoin =
                BuildWithMethod(collectionDerived, innerClass, collectionClassName,
                                new CodeVariableReferenceExpression("joinType"),
                                new CodeVariableReferenceExpression("fetchMode"));
            withFetchAndJoin.Parameters.Add(
                new CodeParameterDeclarationExpression(typeof(JoinType), "joinType"));
            withFetchAndJoin.Parameters.Add(
                new CodeParameterDeclarationExpression(typeof(FetchMode), "fetchMode"));
        }

        private static CodeMemberMethod BuildWithMethod(CodeTypeDeclaration collectionDerived, CodeTypeDeclaration innerClass, string collectionClassName,
                                                 CodeExpression joinType, CodeExpression fetchMode)
        {
            CodeMemberMethod withJoin = new CodeMemberMethod();
            withJoin.Name = "With";
            CodeTypeReference queryType = new CodeTypeReference("Query_" + collectionClassName);
            foreach (CodeTypeParameter parameter in innerClass.TypeParameters)
            {
                queryType.TypeArguments.Add(parameter.Name);
            }
            withJoin.ReturnType = queryType;

            withJoin.Statements.Add(
                new CodeVariableDeclarationStatement(queryType, "query",
                     new CodeObjectCreateExpression(queryType,
                                            new CodeThisReferenceExpression(),
                                            new CodeFieldReferenceExpression(
                                                new CodeThisReferenceExpression(),
                                                "myName"),
                                            new CodeFieldReferenceExpression(
                                                new CodeThisReferenceExpression(),
                                                "associationPath"))
                    ));

            withJoin.Statements.Add(
                new CodeAssignStatement(
                    new CodeFieldReferenceExpression(
                        new CodeVariableReferenceExpression("query"), 
                        "joinType"),
                        joinType)
                );
            withJoin.Statements.Add(
               new CodeAssignStatement(
                   new CodeFieldReferenceExpression(
                       new CodeVariableReferenceExpression("query"),
                       "fetchMode"),
                       fetchMode)
               );

        	withJoin.Statements.Add(
        		new CodeAssignStatement(new CodeFieldReferenceExpression(
                    new CodeVariableReferenceExpression("query"),
					   "ShouldSkipJoinOnIdEquality"),
					   new CodePrimitiveExpression(true))
					   );

            withJoin.Statements.Add(
                new CodeMethodReturnStatement(new CodeVariableReferenceExpression("query"))
                );
            withJoin.Attributes = MemberAttributes.Public;
            collectionDerived.Members.Add(withJoin);
            return withJoin;
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
                                                    GetName(idNode),
                                                    innerClass,
                                                    new CodeVariableReferenceExpression("associationPath"));

                        genericTypeName = idClass.TypeParameters[0].Name;
                    }

                    // generate full object query for simple properties
                    GenerateProperties(prefix,
                                       genericTypeName,
                                       AssociationBehavior.DoNotAdd,
                                       "PropertyQueryBuilder",
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

        
        
        private void GenerateComponents(string genericParameterName, CodeTypeDeclaration parent, XmlNode node, string prefix, params string[] componentPath)
        {
            foreach (string xpathForClass in componentPath)
            {
                foreach (XmlNode classNode in node.SelectNodes(xpathForClass, nsMgr))
                {
                    string name = GetName(classNode);
                    CodeTypeDeclaration innerClass = CreateQueryClassInParentClass(parent, name, null);

                    string newPrefix = GetPath(prefix, name);
                    CreatePropertyInParentClass(genericParameterName, innerClass, name, newPrefix, parent,
												new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "associationPath"));

                    //create full object query object
                    string myGenericName = innerClass.TypeParameters[0].Name;
                    GenerateProperties(newPrefix,
                                       myGenericName,
                                       AssociationBehavior.DoNotAdd,
                                       "PropertyQueryBuilder",
                                       classNode,
                                       innerClass,
                                       "nh:property",
                                       "nh:id");
                    // create reference query obj
                    GenerateProperties(newPrefix,
                                       myGenericName,
                                       AssociationBehavior.AddAssociationFromName,
                                       UseTheQueryClass,
                                       classNode,
                                       innerClass,
                                       "nh:many-to-one",
                                       "nh:one-to-one");

                    GenerateComponents(myGenericName, innerClass, classNode, newPrefix, componentPath);

										GenerateCollections(myGenericName, UseTheQueryClass, classNode, innerClass, "nh:set",
																				"nh:bag", "nh:list", "nh:idbag");
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
            string path,
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
                        new CodeThisReferenceExpression(),
                        new CodePrimitiveExpression(path),
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
        	AddNotClsCompliant(innerClass);
            innerClass.BaseTypes.Add(new CodeTypeReference("Query_" + display, new CodeTypeReference(entityType)));
            innerClass.IsPartial = true;

            // proeprty 
            CodeMemberProperty prop = new CodeMemberProperty();
			AddNotClsCompliant(prop);
            prop.Name = display;
            prop.Type = new CodeTypeReference(innerClass.Name);
            prop.Attributes = MemberAttributes.Public | MemberAttributes.Static;
            prop.GetStatements.Add(new CodeMethodReturnStatement(new CodeObjectCreateExpression(prop.Type)));
            AddToMembersWithSimpleXmlComment(parent, innerClass);
            AddToMembersWithSimpleXmlComment(parent, prop);

            return innerClass;
        }

        /// <summary>
        /// Creates the query class in parent class.
        /// The query class return a query object per each property in the persistant class.
        /// It is also keeping track of what is going on and is capable of tracking joins on the fly.
        /// This is done by combining the generated code and QueryBuilder
        /// </summary>
        private CodeTypeDeclaration CreateQueryClassInParentClass(CodeTypeDeclaration parent, string display,
                                                                  string extends)
        {
            // Query_Blog<T1> : Query.QueryBuilder<T1>
            CodeTypeDeclaration innerClass = new CodeTypeDeclaration("Query_" + display);
        	AddNotClsCompliant(innerClass);
            innerClass.IsPartial = true;
            string genericParameterName = GetGenericParameterName();
            innerClass.TypeParameters.Add(genericParameterName);

			AddNotClsCompliant(innerClass);

        	string classname;
            if (extends == null)
                classname = "QueryBuilder";
            else
                classname = "Query_" + extends;

            CodeTypeReference parentClass = new CodeTypeReference(classname, new CodeTypeReference(genericParameterName));
            innerClass.BaseTypes.Add(parentClass);

			// Query_Blog() : this(null, this, null)
			CodeConstructor ctor = new CodeConstructor();
        	ctor.Attributes = MemberAttributes.Public;
			ctor.ChainedConstructorArgs.Add(new CodePrimitiveExpression(null));
        	ctor.ChainedConstructorArgs.Add(new CodePrimitiveExpression("this"));
			ctor.ChainedConstructorArgs.Add(new CodePrimitiveExpression(null));
			AddToMembersWithSimpleXmlComment(innerClass, ctor);

            // Query_Blog(string name, string associationPath) : QueryBuilder<T1>(name, associationPath);
            CodeConstructor ctor2 = new CodeConstructor();
            ctor2.Attributes = MemberAttributes.Public;
            CodeTypeReference queryBuilderClass = new CodeTypeReference("QueryBuilder", new CodeTypeReference(genericParameterName));
            ctor2.Parameters.Add(new CodeParameterDeclarationExpression(queryBuilderClass, "parent"));
            ctor2.Parameters.Add(new CodeParameterDeclarationExpression(typeof(string), "name"));
            ctor2.Parameters.Add(new CodeParameterDeclarationExpression(typeof(string), "associationPath"));
            ctor2.BaseConstructorArgs.Add(new CodeVariableReferenceExpression("parent"));
            ctor2.BaseConstructorArgs.Add(new CodeVariableReferenceExpression("name"));
            ctor2.BaseConstructorArgs.Add(new CodeVariableReferenceExpression("associationPath"));
            AddToMembersWithSimpleXmlComment(innerClass, ctor2);

            // ctor for backtracking
            CodeConstructor ctor3 = new CodeConstructor();
            ctor3.Attributes = MemberAttributes.Public;
            ctor3.Parameters.Add(new CodeParameterDeclarationExpression(queryBuilderClass, "parent"));
            ctor3.Parameters.Add(new CodeParameterDeclarationExpression(typeof(string), "name"));
            ctor3.Parameters.Add(new CodeParameterDeclarationExpression(typeof(string), "associationPath"));
            ctor3.Parameters.Add(new CodeParameterDeclarationExpression(typeof(bool), "backTrackAssociationOnEquality"));
            ctor3.BaseConstructorArgs.Add(new CodeVariableReferenceExpression("parent"));
            ctor3.BaseConstructorArgs.Add(new CodeVariableReferenceExpression("name"));
            ctor3.BaseConstructorArgs.Add(new CodeVariableReferenceExpression("associationPath"));
            ctor3.BaseConstructorArgs.Add(new CodeVariableReferenceExpression("backTrackAssociationOnEquality"));
            AddToMembersWithSimpleXmlComment(innerClass, ctor3);

            AddToMembersWithSimpleXmlComment(parent, innerClass);
            return innerClass;
        }

    	private void AddNotClsCompliant(CodeTypeMember innerClass)
    	{
			if (innerClass.CustomAttributes.Count != 0)
				return;

    		CodeAttributeDeclaration notCLSComliant = new CodeAttributeDeclaration(
    			typeof(CLSCompliantAttribute).FullName,
    			new CodeAttributeArgument(new CodePrimitiveExpression(false))
    			);
    		innerClass.CustomAttributes.Add(notCLSComliant);
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
            prop.Type = new CodeTypeReference(typeof(string));
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
        /// The most important one is associationBehavior which controls
        /// the way the association paths are used.
        /// If propertyType is equals to <see cref="UseTheQueryClass"/> the Query_{0} idiom
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
            string propertyNameInGeneratedCode = GetPath(prefix, prop.Name);
            CodeObjectCreateExpression newExpr = new CodeObjectCreateExpression(prop.Type);
            newExpr.Parameters.Add(new CodePrimitiveExpression(null));

            CodeVariableDeclarationStatement var =
                new CodeVariableDeclarationStatement(typeof(string), "temp",
                                                     new CodeVariableReferenceExpression("associationPath"));

			
            prop.GetStatements.Add(var);
            switch (associationBehavior)
            {
                case AssociationBehavior.DoNotAdd:
                    newExpr.Parameters.Add(new CodePrimitiveExpression(propertyNameInGeneratedCode));
                    break;
                case AssociationBehavior.AddAssociationFromName:
                    AddAssociationPathFromExpression(prop, new CodePrimitiveExpression(propertyNameInGeneratedCode));
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
			CodeVariableDeclarationStatement child =
			  new CodeVariableDeclarationStatement(newExpr.CreateType, "child", newExpr);

        	prop.GetStatements.Add(child);

        	prop.GetStatements.Add(new CodeAssignStatement(
        	                       	new CodeFieldReferenceExpression(new CodeVariableReferenceExpression("child"),
        	                       	                                 "ShouldSkipJoinOnIdEquality"), 
																	 new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"ShouldSkipJoinOnIdEquality")
        	                       	));
          
            prop.GetStatements.Add(new CodeMethodReturnStatement(new CodeVariableReferenceExpression("child")));
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
						typeName = typeName.Split('`')[0]; // Handle generic type names
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

        private static string GetPath(string prefix, string name)
        {
            return string.Format("{0}{1}{2}", prefix, string.IsNullOrEmpty(prefix) ? "" : ".", name);
        }

        private string GetClassNameFromCollection(XmlNode node)
        {
            XmlNode classNode = node.SelectSingleNode("nh:one-to-many/@class", nsMgr) ??
                node.SelectSingleNode("nh:many-to-many/@class", nsMgr) ??
                node.SelectSingleNode("nh:composite-element/@class", nsMgr);
            if (classNode == null) //this may happen if we let NH guess the class type
            {
                return null;
            }
            return GetTypeNameForDisplay(classNode.Value);
     
        }


        private string GetBaseTypeForDiplay(XmlNode classNode)
        {
            string typeName = GetFullNameOfBaseClass(classNode);
            if (string.IsNullOrEmpty(typeName)) return null;
            return GetTypeNameForDisplay(typeName);
        }

        private static string GetFullNameOfBaseClass(XmlNode node)
        {
            if (node.Attributes["extends"] == null) //does it extends anything explicitly?
            {
                if (Array.IndexOf(classes, node.ParentNode.Name) != -1)
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
                                    string.Format(@"<summary>", member.Name)
                                    , true));
            member.Comments.Add(new CodeCommentStatement(
                                    string.Format(@"Query helper for member {0}.{1}", typeDeclaration.Name, member.Name)
                                    , true));
            member.Comments.Add(new CodeCommentStatement(
                                    string.Format(@"</summary>", member.Name)
                                    , true));
            typeDeclaration.Members.Add(member);
        }
    }
}
