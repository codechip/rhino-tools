namespace BDSLiB.EndPoints
{
    using Boo.Lang;
    using Newtonsoft.Json;

public class JsonMessageAdapter : IQuackFu
{
    private readonly JavaScriptObject js;

    public JsonMessageAdapter(JavaScriptObject js)
    {
        this.js = js;
    }

    public object QuackGet(string name, object[] parameters)
    {
        object value = js[name];
        JavaScriptArray array = value as JavaScriptArray;
        if(array!=null)
        {
            return array.ConvertAll<JsonMessageAdapter>(delegate(object obj)
            {
                return new JsonMessageAdapter((JavaScriptObject) obj);
            });
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