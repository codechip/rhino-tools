using System;
using System.Collections;
using System.Reflection;
using Ayende.NHibernateQueryAnalyzer.Utilities;

namespace Ayende.NHibernateQueryAnalyzer.ProjectLoader
{
	/// <summary>
	/// Summary description for StringSerializedObject.
	/// </summary>
	public class RemoteObject : MarshalByRefObject
	{
		protected object obj;
		protected Type type;
		protected string name;
		protected IDictionary readableProperties;
		protected bool isSimpleType;
		protected string[] keys;

		protected RemoteObject(object obj)
		{
			this.obj = obj;
			type = obj.GetType();
			name = ReflectionUtil.GetName(obj);
            if(InitializeIfObjectIsType(obj)==false)
            {
                readableProperties = ReflectionUtil.GetPropertiesDictionary(obj);
            }
            keys = new string[readableProperties.Count];
            readableProperties.Keys.CopyTo(keys, 0);
            isSimpleType = ReflectionUtil.IsSimpleType(type);
		}

        private bool InitializeIfObjectIsType(object obj)
        {
            Type t = obj as Type;
            if (t != null)
            {
                readableProperties = new Hashtable();
                readableProperties.Add("Name", t.Name);
                readableProperties.Add("FullName", t.FullName);
                return true;
            }
            return false;

        }

		public override object InitializeLifetimeService()
		{
			return null;
		}

		public string TypeName
		{
			get { return type.Name; }
		}

		public string FullTypeName
		{
			get { return type.FullName; }
		}

		public string Name
		{
			get { return name; }
		}

		public string[] Properties
		{
			get {return keys;}
		}

		public object this[string name]
		{
			get
			{
				object obj = readableProperties[name];
				if(obj==null)
					return null;
				if(ReflectionUtil.IsSimpleObject(obj))
					return obj.ToString();
				else
					return RemoteObject.Create(obj);
			}
		}

		public bool IsSimpleType
		{
			get { return isSimpleType; }
		}

		public RemoteObject GetValueOf(string name)
		{
			PropertyInfo prop = type.GetProperty(name);
			if (prop != null)
				return RemoteObject.Create(prop.GetValue(obj, null));
			FieldInfo field = type.GetField(name);
			if (field != null)
				return RemoteObject.Create(field.GetValue(obj));
			throw new InvalidOperationException("Type " + TypeName + " doesn't have a field or property '" + name + "'");
		}

		public string Value
		{
			get { return obj.ToString(); }
		}

		public static RemoteObject Create(object obj)
		{
			if(obj is IList)
				return new RemoteList((IList)obj);
			else if (obj is IDictionary)
				return new RemoteDictionary((IDictionary)obj);
			else
				return new RemoteObject(obj);
		}
	}
}