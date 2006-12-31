using System;
using System.Xml;

namespace Importer
{
	public class Xml
	{
		private XmlNode node;
		private XmlNamespaceManager nsMgr;


		public Xml(XmlNode node, XmlNamespaceManager nsMgr)
		{
			this.node = node;
			this.nsMgr = nsMgr;
		}

		public string GetString(string xpath)
		{
			XmlNode xmlNode = node.SelectSingleNode(xpath, nsMgr);
			if (xmlNode == null)
				return "";
			return xmlNode.Value;
		}

		public DateTime GetDate(string xpath)
		{
			return DateTime.Parse(node.SelectSingleNode(xpath, nsMgr).Value);
		}

		public Xml GetNode(string xpath)
		{
			return new Xml(node.SelectSingleNode(xpath, nsMgr), nsMgr);
		}

		public Guid GetGuid(string xpath)
		{
			XmlNode xmlNode = node.SelectSingleNode(xpath, nsMgr);
			if (xmlNode == null)
				return Guid.Empty;
			return new Guid(xmlNode.Value);
		}
	}
}