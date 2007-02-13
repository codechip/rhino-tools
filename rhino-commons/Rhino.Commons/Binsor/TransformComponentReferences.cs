using System;
using System.Reflection;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.Steps;
using Boo.Lang.Compiler.TypeSystem;

namespace Rhino.Commons.Binsor
{
	internal class TransformComponentReferences : ProcessMethodBodiesWithDuckTyping
	{
		private readonly ConstructorInfo _componentReferenceConstructor =
			typeof (ComponentReference).GetConstructor(new Type[] {typeof (string)});

		public override void OnReferenceExpression(ReferenceExpression node)
		{
			IEntity entity = NameResolutionService.Resolve(node.Name);
			if (entity != null)
			{
				base.OnReferenceExpression(node);
				return;
			}
			if(node.Name.StartsWith("@"))
			{
				string refComponentName = node.Name.Substring(1);
				StringLiteralExpression literal = CodeBuilder.CreateStringLiteral(refComponentName);
				ExternalConstructor constructor = new ExternalConstructor(TypeSystemServices, _componentReferenceConstructor);
				MethodInvocationExpression invocation = CodeBuilder.CreateConstructorInvocation(constructor, literal);
				node.ParentNode.Replace(node, invocation);
				return;
			}
			else if(node.ParentNode is MethodInvocationExpression)
			{
				MethodInvocationExpression mie = (MethodInvocationExpression) node.ParentNode;
				//Transform the first parameter of Component ctor to string.
				if(mie.Target.ToString() == "Component" && mie.Arguments[0] == node)
				{
					StringLiteralExpression literal = CodeBuilder.CreateStringLiteral(node.Name);
					mie.Replace(node, literal);
					return;
				}
			}
			base.OnReferenceExpression(node);
			
		}
	}
}