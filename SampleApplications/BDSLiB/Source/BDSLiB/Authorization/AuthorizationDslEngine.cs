namespace BDSLiB.Authorization
{
    using System;
    using Boo.Lang.Compiler;
    using Rhino.DSL;

    public class AuthorizationDslEngine : DslEngine
    {
        protected override void CustomizeCompiler(
           BooCompiler compiler,
           CompilerPipeline pipeline,
           Uri[] urls)
        {
            // The compiler should allow late bound semantics
            compiler.Parameters.Ducky = true;
            pipeline.Insert(1,
                new AnonymousBaseClassCompilerStep(
                // the base type
                    typeof(AuthorizationRule),
                // the method to override
                    "CheckAuthorization",
                // import the following namespaces
                    "Chapter5.Security"));
        }
    }
}