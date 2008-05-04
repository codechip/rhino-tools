/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * This code was borrowed from this article:  http://www.bestbrains.dk/dansk.aspx/Artikler/Using_Files_In_Unit_Tests
 * I have sent an email to the owner for their authorisation.
 * The had no license or copyright notice.
 * 
 * Getting Started
 * 1. Setup Visual Studio to output an XML documentation file (Project -> Properties -> Build -> "XML documentation file")
 * 2. Suppress warning '1591' to stop the compiler from warning you that every method should have an XML summary
 * 3. Start writing unit tests!
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

using System;
using System.Text;
using System.Xml;
using System.Diagnostics;
using System.Reflection;

namespace Rhino.Testing
{
	public class CommentReader
	{
		private static XmlDocument generatedDocumentation = new XmlDocument();
		private static StackFrame callingMethodFrame;

		public static string GetElement(string elementName)
		{
			StackTrace stackTrace = new StackTrace();
			callingMethodFrame = stackTrace.GetFrame(1);

			generatedDocumentation.Load(String.Format("{0}.xml", 
				Assembly.GetCallingAssembly().FullName.Split(',')[0]));

			XmlNode node = generatedDocumentation.SelectSingleNode(
				String.Format("doc/members/member[contains(@name, '{0}.{1}')]/{2}",
				callingMethodFrame.GetMethod().DeclaringType.ToString(),
				callingMethodFrame.GetMethod().Name,
				elementName));

			if (node != null)
				return node.InnerXml;
			else
                throw new ArgumentOutOfRangeException("elementName", elementName, "Element not found.");
		}
	}
}
