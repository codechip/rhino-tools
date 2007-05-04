using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using Boo.Lang.Interpreter;
using Boo.Lang.Runtime;
using Bumbler;

namespace PlayGround
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			InteractiveInterpreter  interpreter = new InteractiveInterpreter();
			interpreter.RememberLastValue = true;
			interpreter.References.Add(Assembly.Load("Bumbler"));
			interpreter.References.Add(Assembly.Load("NHibernate"));
			interpreter.Eval("import Bumbler");
			interpreter.ConsoleLoopEval();
		}
	}
}