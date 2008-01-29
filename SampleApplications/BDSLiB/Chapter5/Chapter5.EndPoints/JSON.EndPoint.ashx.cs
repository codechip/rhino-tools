namespace Chapter5.EndPoints
{
    using System.IO;
    using System.Text;
    using System.Web;
    using MessageRouting;
    using Newtonsoft.Json;

    public class JSONEndPoint : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            //verify that we only allow POST http calls
            if (context.Request.RequestType != "POST")
            {
                context.Response.StatusCode = 400;
                context.Response.StatusDescription = "Only accessible via POST";
                context.Response.Write("You can only access this end point using POST");
                return;
            }
            // translate from the post body to a json object
            byte[] bytes = context.Request.BinaryRead(context.Request.TotalBytes);
            string json = Encoding.UTF8.GetString(bytes);
            JsonSerializer jsonSerializer = new JsonSerializer();
            JsonReader reader = new JsonReader(new StringReader(json));
            JavaScriptObject javaScriptObject = (JavaScriptObject)jsonSerializer.Deserialize(reader);
            
            // send the json object to be routed
            string returnMessage = Router.Route(new JsonMessageAdapter(javaScriptObject));
            context.Response.Write(returnMessage);
        }

        public bool IsReusable
        {
            get { return false; }
        }
    }
}