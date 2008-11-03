using Boo.Lang.Compiler;
using Boo.Lang.Compiler.Steps;

namespace DSL.Demo.Declerative
{
    public class CaptureCompilerContextStep : AbstractCompilerStep
    {
        public override void Run()
        {
            GlobalMethods.CurrentContext = Context;
            Parameters.Pipeline.AfterStep+=delegate(object sender, CompilerStepEventArgs args)
            {
                if(args.Step is ProcessMethodBodies)
                    GlobalMethods.CurrentContext = null;
            };
        }
    }
}