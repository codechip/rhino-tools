using System;
using System.IO;
using System.Net;

namespace Rhino.Cache.Client
{
    public class Cache : ICache
    {
        private readonly string[] urls;

        public Cache(params string[] urls)
        {
            this.urls = urls;
        }

        public void Delete(string key)
        {
            try
            {
                var request = WebRequest.Create(GetUrl(key));
                request.Method = "DEL";
                using (request.GetResponse())
                {
                }
            }
            catch (WebException)
            {
            }
        }

        public void Put(string key, string type, byte[] value)
        {
            try
            {
                var request = WebRequest.Create(GetUrl(key));
                request.Method = "PUT";
                request.ContentType = type;

                using (var stream = request.GetRequestStream())
                {
                    stream.Write(value, 0, value.Length);
                    stream.Flush();
                }
                using (request.GetResponse())
                {
                }
            }
            catch (WebException)
            {
            }
        }

        public bool Get(string key, out CachedItem item)
        {
            item = null;
            try
            {
                var request = WebRequest.Create(GetUrl(key));
                request.Method = "GET";

                using (var response = request.GetResponse())
                {
                   var buffer = new byte[1024 * 1024];
                    var stream = response.GetResponseStream();
                    using (var ms = new MemoryStream())
                    {
                        int read;
                        do
                        {
                            read = stream.Read(buffer, 0, buffer.Length);
                            ms.Write(buffer, 0, read);
                        } while (read != 0);

                        item = new CachedItem
                        {
                            Type = response.ContentType,
                            Data = ms.ToArray()
                        };
                    }
                }

                return true;
            }
            catch (WebException)
            {
                return false;
            }
        }

        private string GetUrl(string key)
        {
            var index = Math.Abs(key.GetHashCode()) % urls.Length;
            return urls[index] + key;
        }
    }
}