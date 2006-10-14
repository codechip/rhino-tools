using System.IO;
using System.Xml;

namespace Ayende.NHibernateQueryAnalyzer.Core.SchemaEditing
{
	/// <summary>
	/// Summary description for XmlWriterWithEachAttributeInNewLine.
	/// </summary>
	public class XmlWriterWithEachAttributeInNewLine : XmlWriter
	{
		private XmlTextWriter writer;
		private TextWriter rawWriter;
		private int identCount;
		public XmlWriterWithEachAttributeInNewLine(TextWriter w)
		{
			writer = new XmlTextWriter(w);
			writer.Indentation = 1;
			writer.IndentChar = '\t';
			writer.Formatting = Formatting.Indented;
			writer.QuoteChar = '\'';
			rawWriter = w;
		}

		public override WriteState WriteState
		{
			get { return writer.WriteState; }
		}

		public override XmlSpace XmlSpace
		{
			get { return writer.XmlSpace; }
		}

		public override string XmlLang
		{
			get { return writer.XmlLang; }
		}

		public override void WriteStartDocument()
		{
			writer.WriteStartDocument();
		}

		public override void WriteStartDocument(bool standalone)
		{
			writer.WriteStartDocument(standalone);
		}

		public override void WriteEndDocument()
		{
			writer.WriteEndDocument();
		}

		public override void WriteDocType(string name, string pubid, string sysid, string subset)
		{
			writer.WriteDocType(name, pubid, sysid, subset);
		}

		public override void WriteStartElement(string prefix, string localName, string ns)
		{
			writer.WriteStartElement(prefix, localName, ns);
			identCount++;
		}

		public override void WriteEndElement()
		{
			writer.WriteEndElement();
			identCount--;
		}

		public override void WriteFullEndElement()
		{
			writer.WriteFullEndElement();
			identCount--;
		}

		public override void WriteStartAttribute(string prefix, string localName, string ns)
		{
			rawWriter.WriteLine();
			for (int i = 0; i < identCount; i++)
			{
				rawWriter.Write('\t');
			}
			writer.WriteStartAttribute(prefix, localName, ns);
		}

		public override void WriteEndAttribute()
		{
			writer.WriteEndAttribute();
		}

		public override void WriteCData(string text)
		{
			writer.WriteCData(text);
		}

		public override void WriteComment(string text)
		{
			writer.WriteComment(text);
		}

		public override void WriteProcessingInstruction(string name, string text)
		{
			writer.WriteProcessingInstruction(name, text);
		}

		public override void WriteEntityRef(string name)
		{
			writer.WriteEntityRef(name);
		}

		public override void WriteCharEntity(char ch)
		{
			writer.WriteCharEntity(ch);
		}

		public override void WriteWhitespace(string ws)
		{
			writer.WriteWhitespace(ws);
		}

		public override void WriteString(string text)
		{
			writer.WriteString(text);
		}

		public override void WriteSurrogateCharEntity(char lowChar, char highChar)
		{
			writer.WriteSurrogateCharEntity(lowChar, highChar);
		}

		public override void WriteChars(char[] buffer, int index, int count)
		{
			writer.WriteChars(buffer, index, count);
		}

		public override void WriteRaw(char[] buffer, int index, int count)
		{
			writer.WriteRaw(buffer, index, count);
		}

		public override void WriteRaw(string data)
		{
			writer.WriteRaw(data);
		}

		public override void WriteBase64(byte[] buffer, int index, int count)
		{
			writer.WriteBase64(buffer, index, count);
		}

		public override void WriteBinHex(byte[] buffer, int index, int count)
		{
			writer.WriteBinHex(buffer, index, count);
		}

		public override void Close()
		{
			writer.Close();
		}

		public override void Flush()
		{
			writer.Flush();
		}

		public override string LookupPrefix(string ns)
		{
			return writer.LookupPrefix(ns);
		}

		public override void WriteNmToken(string name)
		{
			writer.WriteNmToken(name);
		}

		public override void WriteName(string name)
		{
			writer.WriteName(name);
		}

		public override void WriteQualifiedName(string localName, string ns)
		{
			writer.WriteQualifiedName(localName, ns);
		}

		public override void WriteAttributes(XmlReader reader, bool defattr)
		{
			writer.WriteAttributes(reader, defattr);
		}

		public override void WriteNode(XmlReader reader, bool defattr)
		{
			writer.WriteNode(reader, defattr);
		}
	}
}