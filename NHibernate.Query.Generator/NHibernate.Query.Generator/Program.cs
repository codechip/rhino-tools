using System;

namespace NHibernate.Query.Generator
{
    internal class Program
    {
        private static string targetExtention;
        private static string outputDir;
        private static string inputFilePattern;

        public static void Main(string[] args)
        {
            ParseCommandLineArguments(args);
            new ProcessQueryGeneration(targetExtention, outputDir, inputFilePattern)
                .Generate();
        }

        private static void ParseCommandLineArguments(string[] args)
        {
            if (args.Length != 3)
            {
                Console.WriteLine("Usage:");
                Console.WriteLine("      NHibernate.Query.Generator <cs or vb> <*.hbm.xml> <output-dir>");
                Console.WriteLine("      NHibernate.Query.Generator <cs or vb> asssembly.dll <output-dir>");
                Environment.Exit(1);
            }
            targetExtention = args[0];
            outputDir = args[2];
            inputFilePattern = args[1];
        }
    }
}