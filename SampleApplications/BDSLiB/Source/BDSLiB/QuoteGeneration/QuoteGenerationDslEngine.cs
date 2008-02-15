namespace BDSLiB.QuoteGeneration
{
    using System;
    using Boo.Lang.Compiler;
    using Rhino.DSL;

    public class QuoteGenerationDslEngine : DslEngine
    {
        protected override void CustomizeCompiler(
          BooCompiler compiler,
          CompilerPipeline pipeline,
          string[] urls)
        {
            pipeline.Insert(1,
                new AnonymousBaseClassCompilerStep(
                    typeof(QuoteGeneratorRule),
                    "Evaluate",
                    "BDSLiB.QuoteGeneration"));
            pipeline.Insert(2, new UseSymbolsStep());
        }
    }
}