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