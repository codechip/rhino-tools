namespace BDSLiB.Authorization
{
    using System;
    using System.Collections.Generic;
    using Boo.Lang.Compiler;
    using Boo.Lang.Compiler.Pipelines;
    using Boo.Lang.Compiler.Steps;
    using Rhino.DSL;

    public class AuthorizationDslEngine : DslEngine
    {
        public AuthorizationDslEngine()
        {
            Storage = new XmlFileDslEngineStorage(@"Authorization/AuthorizationRules.xml");
        }

        protected override void CustomizeCompiler(
           BooCompiler compiler,
           CompilerPipeline pipeline,
           string[] urls)
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
                    "BDSLiB.Authorization"));
            pipeline.Insert(2, new AutoReferenceFilesCompilerStep());
        }
    }
}