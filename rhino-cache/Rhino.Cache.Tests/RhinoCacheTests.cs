using System;
using System.IO;
using System.Net;
using Xunit;

namespace Rhino.Cache.Tests
{
    public class RhinoCacheTests : IDisposable
    {
        private readonly HttpListener listener;

        public RhinoCacheTests()
        {
            RhinoCacheHandler.FileName = "test.esent";
            File.Delete(RhinoCacheHandler.FileName);
            listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:6212/");
            listener.Start();
            listener.BeginGetContext(HandleRequest, null);
        }

        private void HandleRequest(IAsyncResult ar)
        {
            try
            {
                var context = listener.EndGetContext(ar);
                new RhinoCacheHandler().ProcessRequest(context);
                listener.BeginGetContext(HandleRequest, null);
            }
            catch (ObjectDisposedException)
            {
            }
        }

        [Fact]
        public void Will_get_404_for_non_existing_entry()
        {
            var request = WebRequest.Create("http://localhost:6212/cache/123`123");
            var e = Assert.Throws<WebException>(() => request.GetResponse());
            Assert.Equal(((HttpWebResponse)e.Response).StatusCode, HttpStatusCode.NotFound);
        }

        [Fact]
        public void Will_reply_with_accepted_to_put()
        {
            var request = WebRequest.Create("http://localhost:6212/cache/test");
            request.Method = "PUT";
            using (var stream = request.GetRequestStream())
            using (var streamWriter = new StreamWriter(stream))
            {
                streamWriter.Write("test test test");
                streamWriter.Flush();
            }
            var response = (HttpWebResponse) request.GetResponse();
            Assert.Equal(response.StatusCode,HttpStatusCode.Accepted);
        }

        [Fact]
        public void Can_get_and_recieve()
        {
            var request = WebRequest.Create("http://localhost:6212/cache/test");
            request.Method = "PUT";
            using (var stream = request.GetRequestStream())
            using (var streamWriter = new StreamWriter(stream))
            {
                streamWriter.Write("test test test");
                streamWriter.Flush();
            }
            request.GetResponse().Close();

            request = WebRequest.Create("http://localhost:6212/cache/test");
            var actual = new StreamReader(request.GetResponse().GetResponseStream()).ReadToEnd();

            Assert.Equal("test test test", actual);
        }

        [Fact]
        public void Can_get_and_remove_item()
        {
            var request = WebRequest.Create("http://localhost:6212/cache/test");
            request.Method = "PUT";
            using (var stream = request.GetRequestStream())
            using (var streamWriter = new StreamWriter(stream))
            {
                streamWriter.Write("test test test");
                streamWriter.Flush();
            }
            request.GetResponse().Close();

            request = WebRequest.Create("http://localhost:6212/cache/test");
            request.Method = "DEL";

            Assert.Equal(((HttpWebResponse) request.GetResponse()).StatusCode, HttpStatusCode.ResetContent);

            request = WebRequest.Create("http://localhost:6212/cache/test");

            var e = Assert.Throws<WebException>(() => request.GetResponse());
            Assert.Equal(((HttpWebResponse)e.Response).StatusCode, HttpStatusCode.NotFound);
        }

        public void Dispose()
        {
            listener.Stop();
            listener.Close();

            RhinoCacheHandler.Reset();
        }
    }
}