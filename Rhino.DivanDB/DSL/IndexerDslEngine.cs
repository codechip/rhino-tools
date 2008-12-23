namespace Rhino.DivanDB.DSL
{
	using Boo.Lang.Compiler;
	using Boo.Lang.Compiler.Pipelines;
	using Boo.Lang.Compiler.Steps;
	using Rhino.DSL;

	public class IndexerDslEngine : DslEngine
	{
		protected override void CustomizeCompiler(BooCompiler compiler, CompilerPipeline pipeline, string[] urls)
		{
			compiler.Parameters.Pipeline = pipeline = new CompileToFile();

			pipeline.Insert(1,
			                new MethodSubstitutionBaseClassCompilerStep(typeof(IndexerBase), "Rhino.DivanDB.DSL"));

			pipeline.Insert(2, 
			                new UnderscorNamingConventionsToPascalCaseCompilerStep());

		
		}
	}
}