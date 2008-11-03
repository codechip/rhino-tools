using Boo.Lang.Compiler;
using Boo.Lang.Compiler.Pipelines;
using Rhino.DSL;

namespace DSL.Demo.SampleDsl
{
    public class SampleDslEngine : DslEngine
    {
        protected override void CustomizeCompiler(
            BooCompiler compiler, 
            CompilerPipeline pipeline, 
            string[] urls)
        {
            // save to disk
            pipeline = new CompileToFile();
            compiler.Parameters.Pipeline = pipeline;
            compiler.Parameters.Ducky = true;
            pipeline.Insert(1, 
                new AnonymousBaseClassCompilerStep(typeof (SampleDslBase), 
                "Prepare", // method to override  
                "DSL.Demo.Model"));// namespace to add
        }
    }
}