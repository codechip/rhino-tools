using System;
using System.IO;
using System.Security.Cryptography;
using Rhino.ServiceBus.DataStructures;
using Rhino.ServiceBus.Internal;

namespace Rhino.ServiceBus.Convertors
{
    public class WireEcryptedStringConvertor : IValueConvertor<WireEcryptedString>
    {
        readonly byte[] rgbKey;
        readonly byte[] rgbIV;

        public WireEcryptedStringConvertor(byte[] key, byte[] iv)
        {
            this.rgbKey = key;
            this.rgbIV = iv;
        }

        public string ToString(WireEcryptedString val)
        {
            using (var rijndael = new RijndaelManaged())
            using (var encryptor = rijndael.CreateEncryptor(rgbKey, rgbIV))
            using (var memoryStream = new MemoryStream())
            using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
            using (var writer = new StreamWriter(cryptoStream))
            {
                writer.Write(val);
                writer.Flush();
                cryptoStream.Flush();
                cryptoStream.FlushFinalBlock();

                return Convert.ToBase64String(memoryStream.ToArray());
            }
        }

        public WireEcryptedString FromString(string val)
        {
            var base64String = Convert.FromBase64String(val);

            using (var rijndael = new RijndaelManaged())
            using (var decryptor = rijndael.CreateDecryptor(rgbKey,rgbIV))
            using (var memoryStream = new MemoryStream(base64String))
            using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
            using (var reader = new StreamReader(cryptoStream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}