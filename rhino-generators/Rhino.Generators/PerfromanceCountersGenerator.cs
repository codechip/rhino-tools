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
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Ast;
using Rhino.Generators.Definitions;
using Attribute = ICSharpCode.NRefactory.Ast.Attribute;

namespace Rhino.Generators
{
	public class PerfromanceCountersGenerator
	{
		private CodeDomProvider provider;


		public PerfromanceCountersGenerator(CodeDomProvider provider)
		{
			this.provider = provider;
		}

		public string Generate(TextReader input)
		{
			StringWriter writer = new StringWriter();
			CodeCompileUnit unit = new CodeCompileUnit();

			IParser parser = ParserFactory.CreateParser(SupportedLanguage.CSharp, input);
			parser.Parse();
			foreach (INode maybeNamespaceNode in parser.CompilationUnit.Children)
			{
				if (!(maybeNamespaceNode is NamespaceDeclaration))
					continue;
				NamespaceDeclaration namespaceDeclaration = (NamespaceDeclaration)maybeNamespaceNode;
				CodeNamespace ns = new CodeNamespace(namespaceDeclaration.Name);
				foreach (INode maybeClassNode in namespaceDeclaration.Children)
				{
					if (!(maybeClassNode is TypeDeclaration))
						continue;

					TypeDeclaration type = (TypeDeclaration)maybeClassNode;
					CodeTypeDeclaration codeTypeDeclaration = ParseClass(type);
					if (codeTypeDeclaration != null)
						ns.Types.Add(codeTypeDeclaration);
				}
				if (ns.Types.Count > 0)
				{
					unit.Namespaces.Add(ns);
				}
			}
			provider.GenerateCodeFromCompileUnit(unit, writer, new CodeGeneratorOptions());
			return writer.GetStringBuilder().ToString();
		}

		private static CodeTypeDeclaration ParseClass(TypeDeclaration typeDeclaration)
		{
			string category = GetCategory(typeDeclaration);
			CodeTypeDeclaration derived = new CodeTypeDeclaration(typeDeclaration.Name + "Derived");
			CodeTypeDeclaration setup = BeginSetup(category, derived);
			derived.BaseTypes.Add(new CodeTypeReference(typeDeclaration.Name));
			foreach (INode maybeProperty in typeDeclaration.Children)
			{
				if (!(maybeProperty is PropertyDeclaration))
					continue;

				PropertyDeclaration propertyDeclaration = (PropertyDeclaration)maybeProperty;

				if (AttributeUtil.HasAttribute(propertyDeclaration, "PerformanceCounter"))
				{
					AssertPropertyIsAbstractAndPublic(typeDeclaration, propertyDeclaration);
					string fieldName = "_" + propertyDeclaration.Name;

					CodeMemberProperty prop = new CodeMemberProperty();
					prop.Name = propertyDeclaration.Name;
					prop.Type = new CodeTypeReference(typeof(PerformanceCounter));
					prop.Attributes = MemberAttributes.Override | MemberAttributes.Public;
					prop.HasSet = false;
					prop.GetStatements.Add(
						new CodeMethodReturnStatement(
						new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), fieldName))
						);
					PerformanceCounterAttribute attribute = GetAttribute(propertyDeclaration);
					AddToSetup(setup, attribute);
					CodeMemberField field = new CodeMemberField(typeof(PerformanceCounter), fieldName);
					field.InitExpression =
						new CodeObjectCreateExpression(typeof(PerformanceCounter),
						new CodePrimitiveExpression(category),
						new CodePrimitiveExpression(attribute.Name)
							);

					derived.Members.Add(field);
					derived.Members.Add(prop);
				}
			}
			FinalizeSetup(setup, category);
			if (derived.Members.Count > 1)
				return derived;
			return null;
		}

		private static CodeTypeDeclaration BeginSetup(string category, CodeTypeDeclaration derived)
		{
			CodeTypeDeclaration setup = GenerateSetupClass(derived, category);
			CodeMemberField setupField = new CodeMemberField(new CodeTypeReference(setup.Name), "_setup");
			setupField.InitExpression = new CodeObjectCreateExpression(new CodeTypeReference(setup.Name));
			derived.Members.Add(setupField);
			CodeMemberProperty setupProp = new CodeMemberProperty();
			setupProp.Name = "Setup";
			setupProp.Type = new CodeTypeReference(setup.Name);
			setupProp.Attributes = MemberAttributes.Public;
			setupProp.HasSet = false;
			setupProp.GetStatements.Add(new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"_setup")));
			derived.Members.Add(setupProp);
			return setup;
		}

		private static void FinalizeSetup(CodeTypeDeclaration setup, string category)
		{
			CodeMemberMethod create = (CodeMemberMethod)setup.Members[0];
			/*    
				PerformanceCounterCategory.Create(Resources.PerformanceCounters_Category,
                Resources.PerformanceCounters_CetegoryHelp,
                PerformanceCounterCategoryType.SingleInstance,
                counters);
			 */
			CodeMethodInvokeExpression createCategory = new CodeMethodInvokeExpression(
				new CodeTypeReferenceExpression(typeof(PerformanceCounterCategory)), "Create",
				new CodePrimitiveExpression(category),
				new CodePrimitiveExpression(""),
				new CodeFieldReferenceExpression(
					new CodeTypeReferenceExpression(typeof(PerformanceCounterCategoryType)), "SingleInstance"
				),
				new CodeVariableReferenceExpression("counters")
				);
			create.Statements.Add(createCategory);
		}

		private static string GetCategory(TypeDeclaration typeDeclaration)
		{
			Attribute attribute = AttributeUtil.GetAttribute(typeDeclaration, "PerformanceCounterCategory");
			if (attribute != null)
			{
				if (attribute.PositionalArguments.Count == 0)
					throw new InvalidOperationException("Should have at least one parameter here");
				PrimitiveExpression expression = (PrimitiveExpression)attribute.PositionalArguments[0];
				return (string)expression.Value;
			}
			return typeDeclaration.Name;
		}

		private static CodeTypeDeclaration GenerateSetupClass(CodeTypeDeclaration derived, string category)
		{
			CodeTypeDeclaration setup = new CodeTypeDeclaration("HandleSetup");
			derived.Members.Add(setup);
			CodeMemberMethod create = new CodeMemberMethod();
			setup.Members.Add(create);
			create.Name = "Run";
			create.Attributes = MemberAttributes.Public;
			/*
			 if (PerformanceCounterCategory.Exists(Resources.PerformanceCounters_Category))
                PerformanceCounterCategory.Delete(Resources.PerformanceCounters_Category);
			 */
			CodeMethodInvokeExpression exists = new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof(PerformanceCounterCategory)), "Exists", new CodePrimitiveExpression(category));
			CodeMethodInvokeExpression delete = new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof(PerformanceCounterCategory)), "Delete", new CodePrimitiveExpression(category));
			CodeConditionStatement ifStmt = new CodeConditionStatement(
				exists);
			ifStmt.TrueStatements.Add(delete);

			create.Statements.Add(ifStmt);

			/*
			CounterCreationDataCollection counters = new CounterCreationDataCollection();
			*/
			CodeVariableDeclarationStatement counters = new CodeVariableDeclarationStatement(typeof(CounterCreationDataCollection),
				"counters", new CodeObjectCreateExpression(typeof(CounterCreationDataCollection)));
			create.Statements.Add(counters);
			return setup;
		}

		private static void AddToSetup(CodeTypeDeclaration setup, PerformanceCounterAttribute attribute)
		{
			/*
			CounterCreationData totalLogsCreator = new CounterCreationData();
            totalLogsCreator.CounterName = Resources.PerformanceCounters_TotalLogs;
            counters.Add(totalLogsCreator);
			*/
			CodeMemberMethod create = (CodeMemberMethod)setup.Members[0];
			string variableName = attribute.Name + "_Counter";
			CodeVariableDeclarationStatement counter = new CodeVariableDeclarationStatement(
				typeof(CounterCreationData), variableName,
				new CodeObjectCreateExpression(typeof(CounterCreationData)));
			CodeAssignStatement name = new CodeAssignStatement(
				 new CodePropertyReferenceExpression(new CodeVariableReferenceExpression(variableName), "CounterName"),
				 new CodePrimitiveExpression(attribute.Name)
				);
			CodeMethodInvokeExpression add = new CodeMethodInvokeExpression(
				new CodeVariableReferenceExpression("counters"), "Add",
				new CodeVariableReferenceExpression(variableName)
				);
			create.Statements.Add(counter);
			create.Statements.Add(name);
			create.Statements.Add(add);
		}

		private static PerformanceCounterAttribute GetAttribute(PropertyDeclaration propertyNode)
		{
			Attribute attributeNode = AttributeUtil.GetAttribute(propertyNode, "PerformanceCounter");
			string name;
			PerformanceCounterType counterType;

			FieldReferenceExpression expression = (FieldReferenceExpression)attributeNode.PositionalArguments[0];
			
			counterType = (PerformanceCounterType)Enum.Parse(typeof(PerformanceCounterType),
															  expression.FieldName);
			if (attributeNode.NamedArguments.Count>0)
			{
				PrimitiveExpression nameExpr = (PrimitiveExpression)attributeNode.NamedArguments[0].Expression;
				name = (string)nameExpr.Value;
			}
			else
			{
				name = propertyNode.Name;
			}

			PerformanceCounterAttribute performanceCounterAttribute = new PerformanceCounterAttribute(counterType);
			performanceCounterAttribute.Name = name;
			return performanceCounterAttribute;
		}

		private static void AssertPropertyIsAbstractAndPublic(TypeDeclaration classNode, PropertyDeclaration propertyNode)
		{
			if ((propertyNode.Modifier & Modifiers.Abstract) == 0)
				throw new InvalidOperationException(string.Format("{0} must be abstract.", GetFullName(propertyNode, classNode)));
			if ((propertyNode.Modifier & Modifiers.Public) == 0)
				throw new InvalidOperationException(string.Format("{0} must be public.", GetFullName(propertyNode, classNode)));
		}

		private static string GetFullName(PropertyDeclaration node, TypeDeclaration parent)
		{
			return string.Format("Property {0} from {1}", node.Name, parent.Name);
		}
	}
}
