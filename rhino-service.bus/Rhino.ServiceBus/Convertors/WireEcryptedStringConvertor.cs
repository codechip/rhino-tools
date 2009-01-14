using System;
using System.IO;
using System.Security.Cryptography;
using Rhino.ServiceBus.DataStructures;
using Rhino.ServiceBus.Internal;

namespace Rhino.ServiceBus.Convertors
{
    public class WireEcryptedStringConvertor : IValueConvertor<WireEcryptedString>
    {
        public byte[] Key{ get; set;}
        public byte[] IV { get; set; }

        public WireEcryptedStringConvertor(byte[] key, byte[] iv)
        {
            Key = key;
            IV = iv;
        }

        public string ToString(WireEcryptedString val)
        {
            using (var rijndael = new RijndaelManaged())
            using (var encryptor = rijndael.CreateEncryptor(Key, IV))
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
            using (var decryptor = rijndael.CreateDecryptor(Key,IV))
            using (var memoryStream = new MemoryStream(base64String))
            using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
            using (var reader = new StreamReader(cryptoStream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}