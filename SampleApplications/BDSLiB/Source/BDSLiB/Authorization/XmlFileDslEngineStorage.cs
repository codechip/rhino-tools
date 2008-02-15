namespace BDSLiB.Authorization
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml;
    using Boo.Lang.Compiler;
    using Boo.Lang.Compiler.IO;
    using Rhino.DSL;

    public class XmlFileDslEngineStorage : IDslEngineStorage
    {
        private readonly XmlDocument xdoc;

        public XmlFileDslEngineStorage(string pathToXmlFile)
        {
            xdoc = new XmlDocument();
            xdoc.Load(pathToXmlFile);
        }

        public string[] GetMatchingUrlsIn(string parentPath, ref string url)
        {
            List<string> ruleNames = new List<string>();
            foreach (XmlNode node in xdoc.SelectNodes("/authorizationRules/rule"))
            {
                if (node.Attributes["operation"].Value == url)
                    ruleNames.Add(node.Attributes["name"].Value);
            }
            if (ruleNames.Count > 0)
                url = ruleNames[0];
            return ruleNames.ToArray();
        }

        public void NotifyOnChange(IEnumerable<string> urls, Action<string> action)
        {
            //not supporting this
        }

        public ICompilerInput CreateInput(string url)
        {
            string xpath = string.Format("/authorizationRules/rule[@name='{0}']/text()", url);
            string text = xdoc.SelectSingleNode(xpath).Value;
            return new StringInput(url, text);
        }

        public bool IsUrlIncludeIn(string[] urls, string parentPath, string url)
        {
            return urls.Length != 0;
        }

        public string GetTypeNameFromUrl(string url)
        {
            return url;
        }

        public void Dispose()
        {
            //nothing to do here
        }
    }
}