using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.Steps;

namespace Rhino.Commons.Binsor
{
	internal class BinsorCompilerStep : AbstractCompilerStep
	{
		public override void Run()
		{
			foreach (Module module in CompileUnit.Modules)
			{
				module.Imports.Add(new Import(module.LexicalInfo, "Rhino.Commons"));
				module.Imports.Add(new Import(module.LexicalInfo, "Rhino.Commons.Binsor"));
				ClassDefinition definition = new ClassDefinition();
				definition.Name = module.FullName;
				definition.BaseTypes.Add(new SimpleTypeReference(typeof (IConfigurationRunner).FullName));
				Method method = new Method("Run");
				method.Body = module.Globals;
				module.Globals = new Block();
				definition.Members.Add(method);
				module.Members.Add(definition);
			}
		}
	}
}