using System;
using System.IO;

public class AssureXmlPath
{
public static void Main(string[] args)
{
	string content;
	foreach (string arg in args)
		Console.WriteLine(arg);
	if(args.Length<3)
	{
		Console.WriteLine("AssureXmlPath: <source file> <string to replace in file> <destination file>");
		return;
	}
	using(StreamReader file = File.OpenText(args[0]))
		content = file.ReadToEnd();
	content = string.Format(content,args[1]);
	using(StreamWriter output = new StreamWriter(File.OpenWrite(args[2])))
		output.Write(content);
	Console.WriteLine("Wrote File: " +args[2]);
}
}