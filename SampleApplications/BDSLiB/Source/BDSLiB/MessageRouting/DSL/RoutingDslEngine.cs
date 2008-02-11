namespace BDSLiB.MessageRouting
{
    using System;
    using Boo.Lang.Compiler;
    using DSL;
    using Rhino.DSL;

    public class RoutingDslEngine : DslEngine
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
                    typeof (RoutingBase), 
                    // the method to override
                    "Route",
                    // import the following namespaces
                    "Chapter5.MessageRouting.Handlers",
                    "Chapter5.MessageRouting.Messages"));
        }
    }
}