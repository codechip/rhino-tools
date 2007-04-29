#region license
// Copyright (c) 2005 - 2007 Ayende Rahien (ayende@ayende.com)
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//     * Neither the name of Ayende Rahien nor the names of its
//     contributors may be used to endorse or promote products derived from this
//     software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion


using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.XPath;

namespace Rhino.Commons
{
	/// <summary>
	/// Adapted from the MsBuild Community Tasks code, for use from code
	/// and not from MSBuild.
	/// </summary>
	public class XmlUpdate
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
		/// Gets or sets the value to write.
		/// </summary>
		/// <value>The value.</value>
		public string Value
		{
			get { return _value; }
			set { _value = value; }
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

		public void Execute()
		{
			XmlDocument document = new XmlDocument();
			document.Load(_xmlFileName);

			XPathNavigator navigator = document.CreateNavigator();
			XmlNamespaceManager manager = new XmlNamespaceManager(navigator.NameTable);

			if (!string.IsNullOrEmpty(_prefix) && !string.IsNullOrEmpty(_namespace))
			{
				manager.AddNamespace(_prefix, _namespace);
			}

			XPathExpression expression = XPathExpression.Compile(_xpath, manager);
			XPathNodeIterator nodes = navigator.Select(expression);


			while (nodes.MoveNext())
				nodes.Current.SetValue(_value);

			using (XmlTextWriter writer = new XmlTextWriter(_xmlFileName, Encoding.UTF8))
			{
				writer.Formatting = Formatting.Indented;
				document.Save(writer);
				writer.Close();
			}
		}
	}
}