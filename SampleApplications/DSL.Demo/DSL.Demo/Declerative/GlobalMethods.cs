using System;
using System.Runtime.CompilerServices;
using Boo.Lang;
using Boo.Lang.Compiler;
using Boo.Lang.Compiler.Ast;
using DSL.Demo.Model;

namespace DSL.Demo.Declerative
{
    [CompilerGlobalScope]
    public static class GlobalMethods
    {
        [ThreadStatic]
        public static CompilerContext CurrentContext;

        [Meta]
        public static Statement when(Expression expression)
        {
            var body = new Block(expression.LexicalInfo);
            body.Add(new ReturnStatement(expression));
            var result = new BlockExpression(body);
            result.Parameters.Add(
                new ParameterDeclaration("order",
                                         CurrentContext.CodeBuilder.CreateTypeReference(typeof(Order))));
            result.Parameters.Add(
                new ParameterDeclaration("customer",
                                         CurrentContext.CodeBuilder.CreateTypeReference(typeof(Customer))));
            return new ReturnStatement(result);
        }

        [Boo.Lang.Extension]
        public static decimal precentage(this decimal self)
        {
            return self*0.01m;
        }

        [Boo.Lang.Extension]
        public static decimal precentage(this int self)
        {
            return self*0.01m;
        }
    }
}