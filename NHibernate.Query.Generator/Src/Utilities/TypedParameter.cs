using System;
using System.Globalization;
using NHibernate;
using NHibernate.Type;
using System.Data;

namespace Ayende.NHibernateQueryAnalyzer.Utilities
{
	/// <summary>
	/// Need to use this because NHibernate's parameters are not serializable, so I get problems
	/// when crossing app-domains.
	/// </summary>
	[Serializable]
	public class TypedParameter
	{
		private string name;
		private Type type;
		private object value;

		private static IType ITypeFromType(Type type)
		{
			if (type == typeof (Byte[])) return NHibernateUtil.Binary;
			if (type == typeof (Boolean)) return NHibernateUtil.Boolean;
			if (type == typeof (Byte)) return NHibernateUtil.Byte;
			if (type == typeof (Char)) return NHibernateUtil.Character;
			if (type == typeof (CultureInfo)) return NHibernateUtil.CultureInfo;
			if (type == typeof (DateTime)) return NHibernateUtil.DateTime;
			if (type == typeof (Decimal)) return NHibernateUtil.Decimal;
			if (type == typeof (Double)) return NHibernateUtil.Double;
			if (type == typeof (Guid)) return NHibernateUtil.Guid;
			if (type == typeof (Int16)) return NHibernateUtil.Int16;
			if (type == typeof (Int32)) return NHibernateUtil.Int32;
			if (type == typeof (Int64)) return NHibernateUtil.Int64;
			if (type == typeof (SByte)) return NHibernateUtil.SByte;
			if (type == typeof (Single)) return NHibernateUtil.Single;
			if (type == typeof (String)) return NHibernateUtil.String;
			if (type == typeof (TimeSpan)) return NHibernateUtil.TimeSpan;
			return NHibernateUtil.GetSerializable(type);
		}

		public string Name
		{
			get { return name; }
			set
			{ name = value; }
		}

		public Type Type
		{
			get { return type; }
			set { type = value; }
		}

		public object Value
		{
			get { return this.value; }
			set { this.value = value; }
		}

		public TypedParameter(string name, Type type, object value)
		{
			this.name = name;
			this.type = type;
			this.value = value;
		}

		public IType IType
		{
			get { return ITypeFromType(type); }
		}
	}
}