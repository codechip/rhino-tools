using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Xml;

namespace Rhino.Licensing
{
    public class LicenseValidator
    {
        private readonly string licensePath;
        private readonly string publicKey;

        public DateTime ExpirationDate { get; private set; }
        public LicenseType LicenseType { get; private set; }
        public Guid UserId { get; private set; }
        public string Name { get; private set; }

        public LicenseValidator(string publicKey, string licensePath)
        {
            this.publicKey = publicKey;
            this.licensePath = licensePath;
        }

        public void AssertValidLicense()
        {
            if (File.Exists(licensePath) == false)
                throw new LicenseFileNotFoundException();

            if (HasExistingLicense())
                return;

            throw new LicenseNotFoundException();
        }

        private bool HasExistingLicense()
        {
            try
            {
                if (File.Exists(licensePath) == false)
                    return false;

                if (TryValidate() == false)
                    return false;

                return DateTime.Now < ExpirationDate;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void RemoveExistingLicense()
        {
            File.Delete(licensePath);
        }

        private bool TryValidate()
        {
            try
            {
                XmlDocument doc;
                if (TryGetValidDocument(out doc) == false)
                    return false;

                if (doc.FirstChild == null)
                    return false;

                XmlNode id = doc.SelectSingleNode("/license/@id");
                if (id == null)
                    return false;

                UserId = new Guid(id.Value);

                XmlNode date = doc.SelectSingleNode("/license/@expiration");
                if (date == null)
                    return false;

                ExpirationDate = XmlConvert.ToDateTime(date.Value, XmlDateTimeSerializationMode.Utc);

                XmlNode licenseType = doc.SelectSingleNode("/license/@type");
                if (licenseType == null)
                    return false;

                LicenseType = (LicenseType) Enum.Parse(typeof (LicenseType), licenseType.Value);

                XmlNode name = doc.SelectSingleNode("/license/name/text()");
                if (name == null)
                    return false;

                Name = name.Value;

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private bool TryGetValidDocument(out XmlDocument doc)
        {
            var rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(publicKey);

            doc = new XmlDocument();
            doc.Load(licensePath);

            var nsMgr = new XmlNamespaceManager(doc.NameTable);
            nsMgr.AddNamespace("sig", "http://www.w3.org/2000/09/xmldsig#");

            var signedXml = new SignedXml(doc);
            var sig = (XmlElement) doc.SelectSingleNode("//sig:Signature", nsMgr);
            signedXml.LoadXml(sig);

            return signedXml.CheckSignature(rsa);
        }
    }
}
