using System;
using System.IO;
using Rhino.Queues.Extensions;

namespace Rhino.Queues.Impl
{
	public class ValidationUtil
	{
		public static void ValidateQueueUrl(string url)
		{
			Uri result;
			if (Uri.TryCreate(url, UriKind.Absolute, out result) == false)
				throw new ArgumentException("invalid queue url: " + url);
			ValidateQueueUrl(result);
		}

		public static void ValidateQueueUrl(Uri uri)
		{
			if ("queue".Equals(uri.Scheme, StringComparison.InvariantCultureIgnoreCase) == false)
				throw new ArgumentException("only 'queue://' url are supported");
			if (uri.Query != "")
				throw new ArgumentException("urls with query paraemters are not suppoerted");
			if (uri.LocalPath == "/")
				throw new ArgumentException("empty queue name is invalid, exepected 'queue://localhost/someQueueName'");
			var queueName = uri.ToQueueName();
			foreach (var invalidFileNameChar in Path.GetInvalidFileNameChars())
			{
				if (queueName.Contains(invalidFileNameChar.ToString()))
					throw new ArgumentException("invalid queue name '" + queueName + "', queue names cannot contain '" + invalidFileNameChar + "'");
			}
		}
	}
}