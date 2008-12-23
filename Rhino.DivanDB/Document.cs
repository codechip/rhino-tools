namespace Rhino.DivanDB
{
	using Newtonsoft.Json.Linq;

	public class Document
	{
		public Document(DocumentId id, JObject payload)
		{
			DocumentId = id;
			Payload = payload;
		}

		public DocumentId DocumentId { get; set; }

		public JObject Payload { get; set; }
	}
}