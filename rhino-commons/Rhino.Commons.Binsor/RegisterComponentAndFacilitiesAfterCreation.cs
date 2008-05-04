using System;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.Steps;

namespace Rhino.Commons.Binsor
{
    [CLSCompliant(false)]
    public class RegisterComponentAndFacilitiesAfterCreation : AbstractNamespaceSensitiveVisitorCompilerStep
    {
        public override void OnExpressionStatement(ExpressionStatement es)
        {
            if (es.Expression is MethodInvocationExpression)
                es.Expression = ReplaceExpression(es.Expression);
            else if (es.Expression is BinaryExpression)
            {
                BinaryExpression expression = (BinaryExpression)es.Expression;
                expression.Right = ReplaceExpression(expression.Right);
            }
        }

        private Expression ReplaceExpression(Expression expression)
        {
            if ((expression is MethodInvocationExpression) == false)
                return expression;

            MethodInvocationExpression mie = (MethodInvocationExpression)expression;
            Expression newExpr = expression;
            string invokedMethod = GetInvokedMethodName_SafeForPropertyEvals(mie);
            if (invokedMethod == "Component" || invokedMethod == typeof(Component).FullName)
            {
                newExpr = CodeBuilder.CreateMethodInvocation(mie, typeof(Component).GetMethod("Register"));
            }
            else if (invokedMethod == "Facility" || invokedMethod == typeof(Facility).FullName)
            {
                newExpr = CodeBuilder.CreateMethodInvocation(mie, typeof(Facility).GetMethod("Register"));
            }
            expression = newExpr;
            return expression;
        }

        public override void Run()
        {
            Visit(CompileUnit);
        }

        private static string GetInvokedMethodName_SafeForPropertyEvals(MethodInvocationExpression mie)
        {
            if (mie.Target.ToString() == "__eval__")
            {
                BinaryExpression binaryExpression = (BinaryExpression)mie.Arguments[0];
                MethodInvocationExpression right = (MethodInvocationExpression)binaryExpression.Right;
                return right.Target.ToString();
            }
            return mie.Target.ToString();
        }
    }
}
