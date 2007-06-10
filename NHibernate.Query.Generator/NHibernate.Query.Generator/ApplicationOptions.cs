using System;
using System.Collections.Generic;
using System.Text;
using CommandLine;

namespace NHibernate.Query.Generator
{
	public class ApplicationOptions
	{
		[Argument(ArgumentType.Required, HelpText = "Choose C# or VB.Net", ShortName = "lang")]
		public Lang Lang;
		[Argument(ArgumentType.Required, HelpText = @"Files for NQG to process, example: mapping\*.hbm.xml, MyAssembly.dll", ShortName = "files")]
		public string InputFilePattern;
		[Argument(ArgumentType.Required, HelpText = "The output directory for the generated code", ShortName = "out")]
		public string OutputDirectory;
		[Argument(ArgumentType.AtMostOnce, HelpText = "The base namespace to use, default to 'Query'", ShortName = "ns")]
		public string BaseNamespace = "Query";
	}

	public enum Lang
	{
		CS,
		VB
	}
}
