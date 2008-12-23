namespace Rhino.DivanDB.DSL
{
	using System.Linq;
	using Boo.Lang;
	using Newtonsoft.Json.Linq;

	public class JsonAdapter : IQuackFu
	{
		private readonly Document js;

		public JsonAdapter(Document js)
		{
			this.js = js;
		}

		public object QuackGet(string name, object[] parameters)
		{
			if (name == "id")
				return js.DocumentId.Id;
			if (name == "version")
				return js.DocumentId.Version;

			object value = js.Payload[name];
			var container = value as JContainer;
			if(container!=null)
			{
				return container.Select(token => new JsonAdapter(new Document(js.DocumentId, (JObject)token)));
			}
			return value;
		}

		public object QuackSet(string name, object[] parameters, object value)
		{
			throw new System.NotImplementedException();
		}

		public object QuackInvoke(string name, params object[] args)
		{
			throw new System.NotImplementedException();
		}
	}
}