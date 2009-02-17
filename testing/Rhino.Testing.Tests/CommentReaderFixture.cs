using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using MbUnit.Framework;

namespace Rhino.Testing.Tests
{
	[TestFixture]
	public class CommentReaderTest
	{
		/// <sample>
		///		<book>
		///			<isbn>1234-1234</isbn>
		///			<title>Code Complete</title>
		///		</book>
		/// </sample>
		[Test]
		public void TestXmlComment()
		{
			string content = CommentReader.GetElement("sample");

			XmlDocument document = new XmlDocument();
			document.LoadXml(content);
			Assert.AreEqual(@"Code Complete", document.SelectSingleNode("//book/title").InnerText);
		}

		/// <sample>
		///		create table Orders ( id int, customer int );
		/// </sample>
		[Test]
		public void TestNonXmlComment()
		{
			string content = CommentReader.GetElement("sample");
			Assert.AreEqual(@"create table Orders ( id int, customer int );", content.Trim());
		}

	}
}
