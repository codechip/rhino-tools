using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Xml;

namespace Rhino.Licensing
{
    public class LicenseGenerator
    {
        private readonly string privateKey;

        public LicenseGenerator(string privateKey)
        {
            this.privateKey = privateKey;
        }

        public string Generate(string name, Guid id, DateTime expirationDate, LicenseType licenseType)
        {
            using (var rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(privateKey);
                var doc = CreateDocument(id, name, expirationDate,licenseType);

                var signature = GetXmlDigitalSignature(doc, rsa);
                doc.FirstChild.AppendChild(doc.ImportNode(signature, true));

                var textWriter = new StringWriter();
                var writer = XmlWriter.Create(textWriter,new XmlWriterSettings
                {
                    Indent = true
                });
                doc.Save(writer);
                return textWriter.GetStringBuilder().ToString();
            }
        }

        private static XmlElement GetXmlDigitalSignature(XmlDocument x, AsymmetricAlgorithm key)
        {
            var signedXml = new SignedXml(x) { SigningKey = key };
            var reference = new Reference { Uri = "" };
            reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());
            signedXml.AddReference(reference);
            signedXml.ComputeSignature();
            return signedXml.GetXml();
        }

        private static XmlDocument CreateDocument(Guid id, string name, DateTime expirationDate, LicenseType licenseType)
        {
            var doc = new XmlDocument();
            var license = doc.CreateElement("license");
            doc.AppendChild(license);
            var idAttr = doc.CreateAttribute("id");
            license.Attributes.Append(idAttr);
            idAttr.Value = id.ToString();
        
            var expirDateAttr = doc.CreateAttribute("expiration");
            license.Attributes.Append(expirDateAttr);
            expirDateAttr.Value = expirationDate.ToString("yyyy-MM-ddTHH:mm:ss.fffffff");

            var licenseAttr = doc.CreateAttribute("type");
            license.Attributes.Append(licenseAttr);
            licenseAttr.Value = licenseType.ToString();
            
            var nameEl = doc.CreateElement("name");
            license.AppendChild(nameEl);
            nameEl.InnerText = name;
            return doc;
        }
    }
}