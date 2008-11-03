using Boo.Lang.Compiler;
using Boo.Lang.Compiler.Pipelines;
using Rhino.DSL;

namespace DSL.Demo.Declerative
{
    public class DeclerativeDslEngine : DslEngine
    {
        protected override void CustomizeCompiler(BooCompiler compiler, CompilerPipeline pipeline, string[] urls)
        {
            // save to disk
            pipeline = new CompileToFile();
            compiler.Parameters.Pipeline = pipeline;
            //compiler.Parameters.Ducky = true;
            pipeline.Insert(1,
                            new AnonymousBaseClassCompilerStep(typeof (DeclerativeDslBase),
                                                               "Prepare", // method to override  
                                                               "DSL.Demo.Model",
                                                               "DSL.Demo.Declerative")); // namespace to add
            pipeline.Insert(2, new CaptureCompilerContextStep());
        }
    }
}