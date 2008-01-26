namespace Chapter5.MessageRouting
{
    using System;
    using Boo.Lang.Compiler;
    using DSL;
    using Rhino.DSL;

    public class RoutingDslEngine : DslEngine
    {
        protected override void CustomizeCompiler(BooCompiler compiler,
                                                  CompilerPipeline pipeline, Uri[] urls)
        {
            compiler.Parameters.Ducky = true;
            pipeline.Insert(1, new AnonymousBaseClassCompilerStep(typeof (RoutingBase), "Route",
                "Chapter5.MessageRouting.Handlers",
                "Chapter5.MessageRouting.Messages"));
        }
    }
}