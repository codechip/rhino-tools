using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using DDW;
using Rhino.Generators.Definitions;

namespace Rhino.Generators
{
	public class PerfromanceCountersGenerator
	{
		private CodeDomProvider provider;


		public PerfromanceCountersGenerator(CodeDomProvider provider)
		{
			this.provider = provider;
		}

		public string Generate(Stream source)
		{
			StringWriter writer = new StringWriter();
			CodeCompileUnit unit = new CodeCompileUnit();

			BufferedStream bs = new BufferedStream(source);
			Lexer lexer = new Lexer(bs);
			Parser parser = new Parser();
			CompilationUnitNode parsedCompilationUnit = parser.Parse(lexer.Lex(), lexer.StringLiterals);
			foreach (NamespaceNode namespaceNode in parsedCompilationUnit.Namespaces)
			{
				CodeNamespace ns = new CodeNamespace(ASTHelper.GetName(namespaceNode.Name));
				foreach (ClassNode classNode in namespaceNode.Classes)
				{
					CodeTypeDeclaration codeTypeDeclaration = ParseClass(classNode);
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

		private CodeTypeDeclaration ParseClass(ClassNode classNode)
		{
			string category = GetCategory(classNode);
			CodeTypeDeclaration derived = new CodeTypeDeclaration(ASTHelper.GetName(classNode.Name) + "Derived");
			CodeTypeDeclaration setup = GenerateSetupClass(derived, category);
			derived.BaseTypes.Add(new CodeTypeReference(ASTHelper.GetName(classNode.Name)));
			foreach (PropertyNode propertyNode in classNode.Properties)
			{
				if (AttributeUtil.HasAttribute(propertyNode, "PerfCounter"))
				{
					AssertPropertyIsAbstractAndPublic(classNode, propertyNode);
					string fieldName = "_" + ASTHelper.GetName(propertyNode.Names[0]);

					CodeMemberProperty prop = new CodeMemberProperty();
					prop.Name = ASTHelper.GetName(propertyNode.Names[0]);
					prop.Type = new CodeTypeReference(typeof(PerformanceCounter));
					prop.Attributes = MemberAttributes.Override | MemberAttributes.Public;
					prop.HasSet = false;
					prop.GetStatements.Add(
						new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), fieldName))
						);
					PerfCounterAttribute attribute = GetAttribute(propertyNode, classNode);
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
			FinalizeSetup(setup,category);
			if (derived.Members.Count > 1)
				return derived;
			return null;
		}

		private void FinalizeSetup(CodeTypeDeclaration setup,string category)
		{
			CodeMemberMethod create = (CodeMemberMethod) setup.Members[0];
			/*    
				PerformanceCounterCategory.Create(Resources.PerformanceCounters_Category,
                Resources.PerformanceCounters_CetegoryHelp,
                PerformanceCounterCategoryType.SingleInstance,
                counters);
			 */
			CodeMethodInvokeExpression createCategory = new CodeMethodInvokeExpression(
				new CodeTypeReferenceExpression(typeof(PerformanceCounterCategory)),"Create",
				new CodePrimitiveExpression(category),
				new CodePrimitiveExpression(""),
				new CodeFieldReferenceExpression(
					new CodeTypeReferenceExpression(typeof(PerformanceCounterCategoryType)),"SingleInstance"
				),
				new CodeVariableReferenceExpression("counters")
				);
			create.Statements.Add(createCategory);
		}

		private string GetCategory(ClassNode classNode)
		{
			AttributeNode attributeNode = AttributeUtil.GetAttribute(classNode, "PerfCounterCategory");
			if (attributeNode == null)
			{
				return ASTHelper.GetName(classNode.Name);
			}
			else
			{
				if (!(attributeNode.Arguments[0].Expression is IdentifierExpression))
					throw new InvalidOperationException("PerfCounterCategory expression must be IdentifierExpression");
				IdentifierExpression identifierExpression = (IdentifierExpression)attributeNode.Arguments[0].Expression;
				return ASTHelper.GetName(identifierExpression);
			}
		}

		private static CodeTypeDeclaration GenerateSetupClass(CodeTypeDeclaration derived, string category)
		{
			CodeTypeDeclaration setup = new CodeTypeDeclaration("Setup");
			derived.Members.Add(setup);
			CodeMemberMethod create = new CodeMemberMethod();
			setup.Members.Add(create);
			create.Name = "Run";
			create.Attributes = MemberAttributes.Public | MemberAttributes.Static;
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

		private void AddToSetup(CodeTypeDeclaration setup, PerfCounterAttribute attribute)
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

		private PerfCounterAttribute GetAttribute(PropertyNode propertyNode, ClassNode parent)
		{
			AttributeNode attributeNode = AttributeUtil.GetAttribute(propertyNode, "PerfCounter");
			string name;
			PerformanceCounterType counterType;

			AttributeArgumentNode counterTypeArg = attributeNode.Arguments[attributeNode.Arguments.Count - 1];
			AssertIsIdentifier(counterTypeArg, parent, propertyNode);
			IdentifierExpression counterTypeIdentifier = (IdentifierExpression)counterTypeArg.Expression;

			counterType = (PerformanceCounterType)Enum.Parse(typeof(PerformanceCounterType),
															  counterTypeIdentifier.Identifier[1]);
			if (attributeNode.Arguments.Count == 2)
			{
				ExpressionNode nameArg = attributeNode.Arguments[2].Expression;
				AssertIsIdentifier(attributeNode.Arguments[2], parent, propertyNode);
				IdentifierExpression nameIdentifier = (IdentifierExpression)nameArg;
				name = nameIdentifier.Identifier[0];
			}
			else
			{
				name = ASTHelper.GetName(propertyNode.Names[0]);
			}

			return new PerfCounterAttribute(name, counterType);
		}

		private static void AssertIsIdentifier(AttributeArgumentNode argument, ClassNode parent, PropertyNode propertyNode)
		{
			if (!(argument.Expression is IdentifierExpression))
				throw new InvalidOperationException("Can't handle non identifier expression on PerfCounter attribute on " + GetFullName(propertyNode, parent));
		}

		private static void AssertPropertyIsAbstractAndPublic(ClassNode classNode, PropertyNode propertyNode)
		{
			if ((propertyNode.Modifiers & Modifier.Abstract) == 0)
				throw new InvalidOperationException(string.Format("{0} must be abstract.", GetFullName(propertyNode, classNode)));
			if ((propertyNode.Modifiers & Modifier.Public) == 0)
				throw new InvalidOperationException(string.Format("{0} must be public.", GetFullName(propertyNode, classNode)));
		}

		private static string GetFullName(PropertyNode node, ClassNode parent)
		{
			return string.Format("Property {0} from {1}", ASTHelper.GetName(node.Names[0]), ASTHelper.GetName(parent.Name));
		}
	}
}
