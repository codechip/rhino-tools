using System.Text;
using System.Xml;
using System.Xml.XPath;

namespace Rhino.Commons
{
	public class XmlRead
	{
		private string _xmlFileName;

		/// <summary>
		/// Gets or sets the name of the XML file.
		/// </summary>
		/// <value>The name of the XML file.</value>
		public string XmlFileName
		{
			get { return _xmlFileName; }
			set { _xmlFileName = value; }
		}

		private string _xpath;

		/// <summary>
		/// Gets or sets the XPath.
		/// </summary>
		/// <value>The XPath.</value>
		public string XPath
		{
			get { return _xpath; }
			set { _xpath = value; }
		}

		private string _value;

		/// <summary>
		/// Gets the value read from file.
		/// </summary>
		/// <value>The value.</value>
		/// <remarks>
		/// If the XPath returns multiple nodes, the values will be semicolon delimited.
		/// </remarks>
		public string Value
		{
			get { return _value; }
		}

		private string _namespace;

		/// <summary>
		/// Gets or sets the default namespace.
		/// </summary>
		/// <value>The namespace.</value>
		public string Namespace
		{
			get { return _namespace; }
			set { _namespace = value; }
		}

		private string _prefix;

		/// <summary>
		/// Gets or sets the prefix to associate with the namespace being added.
		/// </summary>
		/// <value>The namespace prefix.</value>
		public string Prefix
		{
			get { return _prefix; }
			set { _prefix = value; }
		}


		public string Execute()
		{
			XPathDocument document = new XPathDocument(_xmlFileName);
			XPathNavigator navigator = document.CreateNavigator();
			XmlNamespaceManager manager = new XmlNamespaceManager(navigator.NameTable);

			if (!string.IsNullOrEmpty(_prefix) && !string.IsNullOrEmpty(_namespace))
			{
				manager.AddNamespace(_prefix, _namespace);
			}

			XPathExpression expression = XPathExpression.Compile(_xpath, manager);
			switch (expression.ReturnType)
			{
				case XPathResultType.Number:
				case XPathResultType.Boolean:
				case XPathResultType.String:
					_value = navigator.Evaluate(expression).ToString();
					break;
				case XPathResultType.NodeSet:
					XPathNodeIterator nodes = navigator.Select(expression);


					StringBuilder builder = new StringBuilder();
					while (nodes.MoveNext())
						builder.AppendFormat("{0};", nodes.Current.Value);

					if (builder.Length > 0)
						builder.Remove(builder.Length - 1, 1);

					_value = builder.ToString();

					break;
			}
			return Value;
		}
	}
}
