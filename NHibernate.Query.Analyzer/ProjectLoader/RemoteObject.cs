#region license
// Copyright (c) 2005 - 2007 Ayende Rahien (ayende@ayende.com)
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//     * Neither the name of Ayende Rahien nor the names of its
//     contributors may be used to endorse or promote products derived from this
//     software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion


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
			if (obj == null)//select c.Value from Customer c -- where c.Value is null\
				type = typeof(object);
			else
				type = obj.GetType();
			name = ReflectionUtil.GetName(obj);
			if (InitializeIfObjectIsType(obj) == false)
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
			get { return keys; }
		}

		public object this[string name]
		{
			get
			{
				object obj = readableProperties[name];
				if (obj == null)
					return null;
				if (ReflectionUtil.IsSimpleObject(obj))
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
			if (obj is IList)
				return new RemoteList((IList)obj);
			else if (obj is IDictionary)
				return new RemoteDictionary((IDictionary)obj);
			else
				return new RemoteObject(obj);
		}
	}
}