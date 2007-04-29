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


using System.ComponentModel;
using System.Xml.Serialization;
using Ayende.NHibernateQueryAnalyzer.SchemaEditing;
namespace Ayende.NHibernateQueryAnalyzer.HbmSchema
{
	/// <remarks/>
	[XmlType(Namespace="urn:nhibernate-mapping-2.0")]
	[XmlRoot("hibernate-mapping", Namespace="urn:nhibernate-mapping-2.0", IsNullable=false)]

	public class hibernatemapping
	{
		/// <remarks/>
		[XmlElement("meta")] public meta[] meta;

		/// <remarks/>
		[XmlElement("import")] public import[] import;

		/// <remarks/>
		[XmlElement("class", typeof (@class))]
		[XmlElement("subclass", typeof (subclass))]
		[XmlElement("joined-subclass", typeof (joinedsubclass))] public object[] Items;

		/// <remarks/>
		[XmlElement("query")] public query[] query;

		/// <remarks/>
		[XmlAttribute()] public string schema;

		/// <remarks/>
		[XmlAttribute("default-cascade")]
		[DefaultValue(cascadeStyle.none)] public cascadeStyle defaultcascade = cascadeStyle.none;

		/// <remarks/>
		[XmlAttribute("default-access")]
		[DefaultValue(propertyAccess.property)] public propertyAccess defaultaccess = propertyAccess.property;

		/// <remarks/>
		[XmlAttribute("auto-import")]
		[DefaultValue(true)] public bool autoimport = true;
	}

	/// <remarks/>
	[XmlType(Namespace="urn:nhibernate-mapping-2.0")]
	[XmlRoot(Namespace="urn:nhibernate-mapping-2.0", IsNullable=false)]

	public class meta
	{
		/// <remarks/>
		[XmlAttribute()]
		[RequiredTag(MinimumAmount=1)] public string attribute;

		/// <remarks/>
		[XmlAttribute()]
		[DefaultValue(true)] public bool inherit = true;

		/// <remarks/>
		[XmlText()] public string[] Text;
	}

	/// <remarks/>
	[XmlType(Namespace="urn:nhibernate-mapping-2.0")]
	[XmlRoot(Namespace="urn:nhibernate-mapping-2.0", IsNullable=false)]

	public class query
	{
		/// <remarks/>
		[XmlAttribute()]
		[RequiredTag(MinimumAmount=1)] public string name;

		/// <remarks/>
		[XmlText()] public string[] Text;
	}

	/// <remarks/>
	[XmlType(Namespace="urn:nhibernate-mapping-2.0")]
	[XmlRoot("joined-subclass", Namespace="urn:nhibernate-mapping-2.0", IsNullable=false)]

	public class joinedsubclass
	{
		/// <remarks/>
		[XmlElement("meta")] public meta[] meta;

		/// <remarks/>
		[RequiredTag(MinimumAmount=1)] public key key;

		/// <remarks/>
		[XmlElement("one-to-one", typeof (onetoone))]
		[XmlElement("bag", typeof (bag))]
		[XmlElement("map", typeof (map))]
		[XmlElement("property", typeof (property))]
		[XmlElement("component", typeof (component))]
		[XmlElement("many-to-one", typeof (manytoone))]
		[XmlElement("set", typeof (set))]
		[XmlElement("array", typeof (array))]
		[XmlElement("any", typeof (any))]
		[XmlElement("idbag", typeof (idbag))]
		[XmlElement("list", typeof (list))]
		[XmlElement("primitive-array", typeof (primitivearray))] public object[] Items;

		/// <remarks/>
		[XmlElement("joined-subclass")] public joinedsubclass[] joinedsubclass1;

		/// <remarks/>
		[XmlAttribute()]
		[RequiredTag(MinimumAmount=1)] public string name;

		/// <remarks/>
		[XmlAttribute()] public string proxy;

		/// <remarks/>
		[XmlAttribute()] public string table;

		/// <remarks/>
		[XmlAttribute()] public string schema;

		/// <remarks/>
		[XmlAttribute("dynamic-update")]
		[DefaultValue(false)] public bool dynamicupdate = false;

		/// <remarks/>
		[XmlAttribute("dynamic-insert")]
		[DefaultValue(false)] public bool dynamicinsert = false;

		/// <remarks/>
		[XmlAttribute()] public string extends;
	}

	/// <remarks/>
	[XmlType(Namespace="urn:nhibernate-mapping-2.0")]
	[XmlRoot(Namespace="urn:nhibernate-mapping-2.0", IsNullable=false)]

	public class key
	{
		/// <remarks/>
		[XmlElement("column")] public column[] column;

		/// <remarks/>
		[XmlAttribute("column")] public string column1;
	}

	/// <remarks/>
	[XmlType(Namespace="urn:nhibernate-mapping-2.0")]
	[XmlRoot(Namespace="urn:nhibernate-mapping-2.0", IsNullable=false)]

	public class column
	{
		/// <remarks/>
		[XmlAttribute()]
		[RequiredTag(MinimumAmount=1)] public string name;

		/// <remarks/>
		[XmlAttribute(DataType="positiveInteger")] public string length;

		/// <remarks/>
		[XmlAttribute("not-null")] public bool notnull;

		/// <remarks/>
		[XmlIgnore()] public bool notnullSpecified;

		/// <remarks/>
		[XmlAttribute()] public bool unique;

		/// <remarks/>
		[XmlIgnore()] public bool uniqueSpecified;

		/// <remarks/>
		[XmlAttribute("unique-key")] public string uniquekey;

		/// <remarks/>
		[XmlAttribute("sql-type")] public string sqltype;

		/// <remarks/>
		[XmlAttribute()] public string index;
	}

	/// <remarks/>
	[XmlType(Namespace="urn:nhibernate-mapping-2.0")]
	[XmlRoot("one-to-one", Namespace="urn:nhibernate-mapping-2.0", IsNullable=false)]

	public class onetoone
	{
		/// <remarks/>
		[XmlElement("meta")] public meta[] meta;

		/// <remarks/>
		[XmlAttribute()]
		[RequiredTag(MinimumAmount=1)] public string name;

		/// <remarks/>
		[XmlAttribute()] public propertyAccess access;

		/// <remarks/>
		[XmlIgnore()] public bool accessSpecified;

		/// <remarks/>
		[XmlAttribute()] public string @class;

		/// <remarks/>
		[XmlAttribute()] public cascadeStyle cascade;

		/// <remarks/>
		[XmlIgnore()] public bool cascadeSpecified;

		/// <remarks/>
		[XmlAttribute("outer-join")]
		[DefaultValue(outerJoinStrategy.auto)] public outerJoinStrategy outerjoin = outerJoinStrategy.auto;

		/// <remarks/>
		[XmlAttribute()]
		[DefaultValue(false)] public bool constrained = false;
	}

	/// <remarks/>
	[XmlType(Namespace="urn:nhibernate-mapping-2.0")]
	public enum propertyAccess
	{
		/// <remarks/>
		property,

		/// <remarks/>
		field,

		/// <remarks/>
		[XmlEnum("field.camelcase")] fieldcamelcase,

		/// <remarks/>
		[XmlEnum("field.camelcase-underscore")] fieldcamelcaseunderscore,

		/// <remarks/>
		[XmlEnum("field.pascalcase-m-underscore")] fieldpascalcasemunderscore,

		/// <remarks/>
		[XmlEnum("field.lowercase-underscore")] fieldlowercaseunderscore,

		/// <remarks/>
		[XmlEnum("nosetter.camelcase")] nosettercamelcase,

		/// <remarks/>
		[XmlEnum("nosetter.camelcase-underscore")] nosettercamelcaseunderscore,

		/// <remarks/>
		[XmlEnum("nosetter.pascalcase-m-underscore")] nosetterpascalcasemunderscore,

		/// <remarks/>
		[XmlEnum("nosetter.lowercase-underscore")] nosetterlowercaseunderscore,
	}

	/// <remarks/>
	[XmlType(Namespace="urn:nhibernate-mapping-2.0")]
	public enum cascadeStyle
	{
		/// <remarks/>
		all,

		/// <remarks/>
		[XmlEnum("all-delete-orphan")] alldeleteorphan,

		/// <remarks/>
		none,

		/// <remarks/>
		[XmlEnum("save-update")] saveupdate,

		/// <remarks/>
		delete,

		/// <remarks/>
		[XmlEnum("delete-orphan")] deleteorphan,
	}

	/// <remarks/>
	[XmlType(Namespace="urn:nhibernate-mapping-2.0")]
	public enum outerJoinStrategy
	{
		/// <remarks/>
		auto,

		/// <remarks/>
		@true,

		/// <remarks/>
		@false,
	}

	/// <remarks/>
	[XmlType(Namespace="urn:nhibernate-mapping-2.0")]
	[XmlRoot(Namespace="urn:nhibernate-mapping-2.0", IsNullable=false)]

	public class bag
	{
		/// <remarks/>
		[XmlElement("meta")] public meta[] meta;

		/// <remarks/>
		[XmlElement("jcs-cache")] public jcscache jcscache;

		/// <remarks/>
		[RequiredTag(MinimumAmount=1)] public key key;

		/// <remarks/>
		[XmlElement("many-to-many", typeof (manytomany))]
		[XmlElement("composite-element", typeof (compositeelement))]
		[XmlElement("many-to-any", typeof (manytoany))]
		[XmlElement("element", typeof (element))]
		[XmlElement("one-to-many", typeof (onetomany))]
		[RequiredTag(MinimumAmount=1)] public object Item;

		/// <remarks/>
		[XmlAttribute()]
		[RequiredTag(MinimumAmount=1)] public string name;

		/// <remarks/>
		[XmlAttribute()] public propertyAccess access;

		/// <remarks/>
		[XmlIgnore()] public bool accessSpecified;

		/// <remarks/>
		[XmlAttribute()] public string table;

		/// <remarks/>
		[XmlAttribute()] public string schema;

		/// <remarks/>
		[XmlAttribute()]
		[DefaultValue(false)] public bool lazy = false;

		/// <remarks/>
		[XmlAttribute()]
		[DefaultValue(false)] public bool inverse = false;

		/// <remarks/>
		[XmlAttribute()] public cascadeStyle cascade;

		/// <remarks/>
		[XmlIgnore()] public bool cascadeSpecified;

		/// <remarks/>
		[XmlAttribute("order-by")] public string orderby;

		/// <remarks/>
		[XmlAttribute()] public string where;
	}

	/// <remarks/>
	[XmlType(Namespace="urn:nhibernate-mapping-2.0")]
	[XmlRoot("jcs-cache", Namespace="urn:nhibernate-mapping-2.0", IsNullable=false)]

	public class jcscache
	{
		/// <remarks/>
		[XmlAttribute()]
		[RequiredTag(MinimumAmount=1)] public jcscacheUsage usage;
	}

	/// <remarks/>
	[XmlType(Namespace="urn:nhibernate-mapping-2.0")]
	public enum jcscacheUsage
	{
		/// <remarks/>
		[XmlEnum("read-only")] @readonly,

		/// <remarks/>
		[XmlEnum("read-write")] readwrite,

		/// <remarks/>
		[XmlEnum("nonstrict-read-write")] nonstrictreadwrite,
	}

	/// <remarks/>
	[XmlType(Namespace="urn:nhibernate-mapping-2.0")]
	[XmlRoot("many-to-many", Namespace="urn:nhibernate-mapping-2.0", IsNullable=false)]

	public class manytomany
	{
		/// <remarks/>
		[XmlElement("meta")] public meta[] meta;

		/// <remarks/>
		[XmlElement("column")] public column[] column;

		/// <remarks/>
		[XmlAttribute()]
		[RequiredTag(MinimumAmount=1)] public string @class;

		/// <remarks/>
		[XmlAttribute("column")] public string column1;

		/// <remarks/>
		[XmlAttribute("outer-join")]
		[DefaultValue(outerJoinStrategy.auto)] public outerJoinStrategy outerjoin = outerJoinStrategy.auto;
	}

	/// <remarks/>
	[XmlType(Namespace="urn:nhibernate-mapping-2.0")]
	[XmlRoot("composite-element", Namespace="urn:nhibernate-mapping-2.0", IsNullable=false)]

	public class compositeelement
	{
		/// <remarks/>
		public parent parent;

		/// <remarks/>
		[XmlElement("property", typeof (property))]
		[XmlElement("many-to-one", typeof (manytoone))]
		[XmlElement("nested-composite-element", typeof (nestedcompositeelement))] public object[] Items;

		/// <remarks/>
		[XmlAttribute()]
		[RequiredTag(MinimumAmount=1)] public string @class;
	}

	/// <remarks/>
	[XmlType(Namespace="urn:nhibernate-mapping-2.0")]
	[XmlRoot(Namespace="urn:nhibernate-mapping-2.0", IsNullable=false)]

	public class parent
	{
		/// <remarks/>
		[XmlAttribute()]
		[RequiredTag(MinimumAmount=1)] public string name;
	}

	/// <remarks/>
	[XmlType(Namespace="urn:nhibernate-mapping-2.0")]
	[XmlRoot(Namespace="urn:nhibernate-mapping-2.0", IsNullable=false)]

	public class property
	{
		/// <remarks/>
		[XmlElement("meta")] public meta[] meta;

		/// <remarks/>
		[XmlElement("column")] public column[] column;

		/// <remarks/>
		[XmlAttribute()]
		[RequiredTag(MinimumAmount=1)] public string name;

		/// <remarks/>
		[XmlAttribute()] public propertyAccess access;

		/// <remarks/>
		[XmlIgnore()] public bool accessSpecified;

		/// <remarks/>
		[XmlAttribute()] public string type;

		/// <remarks/>
		[XmlAttribute("column")] public string column1;

		/// <remarks/>
		[XmlAttribute(DataType="positiveInteger")] public string length;

		/// <remarks/>
		[XmlAttribute("not-null")]
		[DefaultValue(false)] public bool notnull = false;

		/// <remarks/>
		[XmlAttribute()]
		[DefaultValue(false)] public bool unique = false;

		/// <remarks/>
		[XmlAttribute()]
		[DefaultValue(true)] public bool update = true;

		/// <remarks/>
		[XmlAttribute()]
		[DefaultValue(true)] public bool insert = true;

		/// <remarks/>
		[XmlAttribute()] public string formula;
	}

	/// <remarks/>
	[XmlType(Namespace="urn:nhibernate-mapping-2.0")]
	[XmlRoot("many-to-one", Namespace="urn:nhibernate-mapping-2.0", IsNullable=false)]

	public class manytoone
	{
		/// <remarks/>
		[XmlElement("meta")] public meta[] meta;

		/// <remarks/>
		[XmlElement("column")] public column[] column;

		/// <remarks/>
		[XmlAttribute()]
		[RequiredTag(MinimumAmount=1)] public string name;

		/// <remarks/>
		[XmlAttribute()] public propertyAccess access;

		/// <remarks/>
		[XmlIgnore()] public bool accessSpecified;

		/// <remarks/>
		[XmlAttribute()] public string @class;

		/// <remarks/>
		[XmlAttribute("column")] public string column1;

		/// <remarks/>
		[XmlAttribute("not-null")]
		[DefaultValue(false)] public bool notnull = false;

		/// <remarks/>
		[XmlAttribute()]
		[DefaultValue(false)] public bool unique = false;

		/// <remarks/>
		[XmlAttribute()] public cascadeStyle cascade;

		/// <remarks/>
		[XmlIgnore()] public bool cascadeSpecified;

		/// <remarks/>
		[XmlAttribute("outer-join")]
		[DefaultValue(outerJoinStrategy.auto)] public outerJoinStrategy outerjoin = outerJoinStrategy.auto;

		/// <remarks/>
		[XmlAttribute()]
		[DefaultValue(true)] public bool update = true;

		/// <remarks/>
		[XmlAttribute()]
		[DefaultValue(true)] public bool insert = true;
	}

	/// <remarks/>
	[XmlType(Namespace="urn:nhibernate-mapping-2.0")]
	[XmlRoot("nested-composite-element", Namespace="urn:nhibernate-mapping-2.0", IsNullable=false)]

	public class nestedcompositeelement
	{
		/// <remarks/>
		public parent parent;

		/// <remarks/>
		[XmlElement("property", typeof (property))]
		[XmlElement("many-to-one", typeof (manytoone))]
		[XmlElement("nested-composite-element", typeof (nestedcompositeelement))] public object[] Items;

		/// <remarks/>
		[XmlAttribute()]
		[RequiredTag(MinimumAmount=1)] public string @class;

		/// <remarks/>
		[XmlAttribute()]
		[RequiredTag(MinimumAmount=1)] public string name;

		/// <remarks/>
		[XmlAttribute()] public propertyAccess access;

		/// <remarks/>
		[XmlIgnore()] public bool accessSpecified;
	}

	/// <remarks/>
	[XmlType(Namespace="urn:nhibernate-mapping-2.0")]
	[XmlRoot("many-to-any", Namespace="urn:nhibernate-mapping-2.0", IsNullable=false)]

	public class manytoany
	{
		/// <remarks/>
		[XmlElement("column")]
		[RequiredTag(MinimumAmount=1)] public column[] column;

		/// <remarks/>
		[XmlAttribute("id-type")]
		[RequiredTag(MinimumAmount=1)] public string idtype;

		/// <remarks/>
		[XmlAttribute("meta-type")] public string metatype;
	}

	/// <remarks/>
	[XmlType(Namespace="urn:nhibernate-mapping-2.0")]
	[XmlRoot(Namespace="urn:nhibernate-mapping-2.0", IsNullable=false)]

	public class element
	{
		/// <remarks/>
		[XmlElement("column")] public column[] column;

		/// <remarks/>
		[XmlAttribute("column")] public string column1;

		/// <remarks/>
		[XmlAttribute()]
		[RequiredTag(MinimumAmount=1)] public string type;

		/// <remarks/>
		[XmlAttribute(DataType="positiveInteger")] public string length;

		/// <remarks/>
		[XmlAttribute("not-null")]
		[DefaultValue(false)] public bool notnull = false;

		/// <remarks/>
		[XmlAttribute()]
		[DefaultValue(false)] public bool unique = false;
	}

	/// <remarks/>
	[XmlType(Namespace="urn:nhibernate-mapping-2.0")]
	[XmlRoot("one-to-many", Namespace="urn:nhibernate-mapping-2.0", IsNullable=false)]

	public class onetomany
	{
		/// <remarks/>
		[XmlAttribute()]
		[RequiredTag(MinimumAmount=1)] public string @class;
	}

	/// <remarks/>
	[XmlType(Namespace="urn:nhibernate-mapping-2.0")]
	[XmlRoot(Namespace="urn:nhibernate-mapping-2.0", IsNullable=false)]

	public class map
	{
		/// <remarks/>
		[XmlElement("meta")] public meta[] meta;

		/// <remarks/>
		[XmlElement("jcs-cache")] public jcscache jcscache;

		/// <remarks/>
		[RequiredTag(MinimumAmount=1)] public key key;

		/// <remarks/>
		[XmlElement("index", typeof (index))]
		[XmlElement("index-many-to-many", typeof (indexmanytomany))]
		[XmlElement("composite-index", typeof (compositeindex))]
		[XmlElement("index-many-to-any", typeof (indexmanytoany))]
		[RequiredTag(MinimumAmount=1)] public object Item;

		/// <remarks/>
		[XmlElement("many-to-many", typeof (manytomany))]
		[XmlElement("composite-element", typeof (compositeelement))]
		[XmlElement("many-to-any", typeof (manytoany))]
		[XmlElement("element", typeof (element))]
		[XmlElement("one-to-many", typeof (onetomany))]
		[RequiredTag(MinimumAmount=1)] public object Item1;

		/// <remarks/>
		[XmlAttribute()]
		[RequiredTag(MinimumAmount=1)] public string name;

		/// <remarks/>
		[XmlAttribute()] public propertyAccess access;

		/// <remarks/>
		[XmlIgnore()] public bool accessSpecified;

		/// <remarks/>
		[XmlAttribute()] public string table;

		/// <remarks/>
		[XmlAttribute()] public string schema;

		/// <remarks/>
		[XmlAttribute()]
		[DefaultValue(false)] public bool lazy = false;

		/// <remarks/>
		[XmlAttribute()]
		[DefaultValue(false)] public bool inverse = false;

		/// <remarks/>
		[XmlAttribute()]
		[DefaultValue("unsorted")] public string sort = "unsorted";

		/// <remarks/>
		[XmlAttribute()] public cascadeStyle cascade;

		/// <remarks/>
		[XmlIgnore()] public bool cascadeSpecified;

		/// <remarks/>
		[XmlAttribute("order-by")] public string orderby;

		/// <remarks/>
		[XmlAttribute()] public string where;
	}

	/// <remarks/>
	[XmlType(Namespace="urn:nhibernate-mapping-2.0")]
	[XmlRoot(Namespace="urn:nhibernate-mapping-2.0", IsNullable=false)]

	public class index
	{
		/// <remarks/>
		[XmlElement("column")] public column[] column;

		/// <remarks/>
		[XmlAttribute("column")] public string column1;

		/// <remarks/>
		[XmlAttribute()] public string type;

		/// <remarks/>
		[XmlAttribute(DataType="positiveInteger")] public string length;
	}

	/// <remarks/>
	[XmlType(Namespace="urn:nhibernate-mapping-2.0")]
	[XmlRoot("index-many-to-many", Namespace="urn:nhibernate-mapping-2.0", IsNullable=false)]

	public class indexmanytomany
	{
		/// <remarks/>
		[XmlElement("column")] public column[] column;

		/// <remarks/>
		[XmlAttribute()]
		[RequiredTag(MinimumAmount=1)] public string @class;

		/// <remarks/>
		[XmlAttribute("column")] public string column1;
	}

	/// <remarks/>
	[XmlType(Namespace="urn:nhibernate-mapping-2.0")]
	[XmlRoot("composite-index", Namespace="urn:nhibernate-mapping-2.0", IsNullable=false)]

	public class compositeindex
	{
		/// <remarks/>
		[XmlElement("key-property", typeof (keyproperty))]
		[XmlElement("key-many-to-one", typeof (keymanytoone))]
		[RequiredTag(MinimumAmount=1)] public object[] Items;

		/// <remarks/>
		[XmlAttribute()]
		[RequiredTag(MinimumAmount=1)] public string @class;
	}

	/// <remarks/>
	[XmlType(Namespace="urn:nhibernate-mapping-2.0")]
	[XmlRoot("key-property", Namespace="urn:nhibernate-mapping-2.0", IsNullable=false)]

	public class keyproperty
	{
		/// <remarks/>
		[XmlElement("column")] public column[] column;

		/// <remarks/>
		[XmlAttribute()]
		[RequiredTag(MinimumAmount=1)] public string name;

		/// <remarks/>
		[XmlAttribute()] public propertyAccess access;

		/// <remarks/>
		[XmlIgnore()] public bool accessSpecified;

		/// <remarks/>
		[XmlAttribute()] public string type;

		/// <remarks/>
		[XmlAttribute("column")] public string column1;

		/// <remarks/>
		[XmlAttribute(DataType="positiveInteger")] public string length;
	}

	/// <remarks/>
	[XmlType(Namespace="urn:nhibernate-mapping-2.0")]
	[XmlRoot("key-many-to-one", Namespace="urn:nhibernate-mapping-2.0", IsNullable=false)]

	public class keymanytoone
	{
		/// <remarks/>
		[XmlElement("column")] public column[] column;

		/// <remarks/>
		[XmlAttribute()]
		[RequiredTag(MinimumAmount=1)] public string name;

		/// <remarks/>
		[XmlAttribute()] public propertyAccess access;

		/// <remarks/>
		[XmlIgnore()] public bool accessSpecified;

		/// <remarks/>
		[XmlAttribute()] public string @class;

		/// <remarks/>
		[XmlAttribute("column")] public string column1;
	}

	/// <remarks/>
	[XmlType(Namespace="urn:nhibernate-mapping-2.0")]
	[XmlRoot("index-many-to-any", Namespace="urn:nhibernate-mapping-2.0", IsNullable=false)]

	public class indexmanytoany
	{
		/// <remarks/>
		[XmlElement("column")]
		[RequiredTag(MinimumAmount=1)] public column[] column;

		/// <remarks/>
		[XmlAttribute("id-type")]
		[RequiredTag(MinimumAmount=1)] public string idtype;

		/// <remarks/>
		[XmlAttribute("meta-type")] public string metatype;
	}

	/// <remarks/>
	[XmlType(Namespace="urn:nhibernate-mapping-2.0")]
	[XmlRoot(Namespace="urn:nhibernate-mapping-2.0", IsNullable=false)]

	public class component
	{
		/// <remarks/>
		public parent parent;

		/// <remarks/>
		[XmlElement("one-to-one", typeof (onetoone))]
		[XmlElement("bag", typeof (bag))]
		[XmlElement("map", typeof (map))]
		[XmlElement("property", typeof (property))]
		[XmlElement("component", typeof (component))]
		[XmlElement("many-to-one", typeof (manytoone))]
		[XmlElement("set", typeof (set))]
		[XmlElement("array", typeof (array))]
		[XmlElement("any", typeof (any))]
		[XmlElement("list", typeof (list))]
		[XmlElement("primitive-array", typeof (primitivearray))] public object[] Items;

		/// <remarks/>
		[XmlAttribute()]
		[RequiredTag(MinimumAmount=1)] public string name;

		/// <remarks/>
		[XmlAttribute()] public propertyAccess access;

		/// <remarks/>
		[XmlIgnore()] public bool accessSpecified;

		/// <remarks/>
		[XmlAttribute()] public string @class;

		/// <remarks/>
		[XmlAttribute()]
		[DefaultValue(true)] public bool update = true;

		/// <remarks/>
		[XmlAttribute()]
		[DefaultValue(true)] public bool insert = true;
	}

	/// <remarks/>
	[XmlType(Namespace="urn:nhibernate-mapping-2.0")]
	[XmlRoot(Namespace="urn:nhibernate-mapping-2.0", IsNullable=false)]

	public class set
	{
		/// <remarks/>
		[XmlElement("meta")] public meta[] meta;

		/// <remarks/>
		[XmlElement("jcs-cache")] public jcscache jcscache;

		/// <remarks/>
		[RequiredTag(MinimumAmount=1)] public key key;

		/// <remarks/>
		[XmlElement("many-to-many", typeof (manytomany))]
		[XmlElement("composite-element", typeof (compositeelement))]
		[XmlElement("many-to-any", typeof (manytoany))]
		[XmlElement("element", typeof (element))]
		[XmlElement("one-to-many", typeof (onetomany))]
		[RequiredTag(MinimumAmount=1)] public object Item;

		/// <remarks/>
		[XmlAttribute()]
		[RequiredTag(MinimumAmount=1)] public string name;

		/// <remarks/>
		[XmlAttribute()] public propertyAccess access;

		/// <remarks/>
		[XmlIgnore()] public bool accessSpecified;

		/// <remarks/>
		[XmlAttribute()] public string table;

		/// <remarks/>
		[XmlAttribute()] public string schema;

		/// <remarks/>
		[XmlAttribute()]
		[DefaultValue(false)] public bool lazy = false;

		/// <remarks/>
		[XmlAttribute()]
		[DefaultValue("unsorted")] public string sort = "unsorted";

		/// <remarks/>
		[XmlAttribute()]
		[DefaultValue(false)] public bool inverse = false;

		/// <remarks/>
		[XmlAttribute()] public cascadeStyle cascade;

		/// <remarks/>
		[XmlIgnore()] public bool cascadeSpecified;

		/// <remarks/>
		[XmlAttribute("order-by")] public string orderby;

		/// <remarks/>
		[XmlAttribute()] public string where;
	}

	/// <remarks/>
	[XmlType(Namespace="urn:nhibernate-mapping-2.0")]
	[XmlRoot(Namespace="urn:nhibernate-mapping-2.0", IsNullable=false)]

	public class array
	{
		/// <remarks/>
		[XmlElement("meta")] public meta[] meta;

		/// <remarks/>
		[XmlElement("jcs-cache")] public jcscache jcscache;

		/// <remarks/>
		[RequiredTag(MinimumAmount=1)] public key key;

		/// <remarks/>
		[RequiredTag(MinimumAmount=1)] public index index;

		/// <remarks/>
		[XmlElement("many-to-many", typeof (manytomany))]
		[XmlElement("composite-element", typeof (compositeelement))]
		[XmlElement("many-to-any", typeof (manytoany))]
		[XmlElement("element", typeof (element))]
		[XmlElement("one-to-many", typeof (onetomany))]
		[RequiredTag(MinimumAmount=1)] public object Item;

		/// <remarks/>
		[XmlAttribute()]
		[RequiredTag(MinimumAmount=1)] public string name;

		/// <remarks/>
		[XmlAttribute()] public propertyAccess access;

		/// <remarks/>
		[XmlIgnore()] public bool accessSpecified;

		/// <remarks/>
		[XmlAttribute()] public string table;

		/// <remarks/>
		[XmlAttribute()] public string schema;

		/// <remarks/>
		[XmlAttribute("element-class")] public string elementclass;

		/// <remarks/>
		[XmlAttribute()] public cascadeStyle cascade;

		/// <remarks/>
		[XmlIgnore()] public bool cascadeSpecified;

		/// <remarks/>
		[XmlAttribute()] public string where;
	}

	/// <remarks/>
	[XmlType(Namespace="urn:nhibernate-mapping-2.0")]
	[XmlRoot(Namespace="urn:nhibernate-mapping-2.0", IsNullable=false)]

	public class any
	{
		/// <remarks/>
		[XmlElement("column")]
		[RequiredTag(MinimumAmount=1)] public column[] column;

		/// <remarks/>
		[XmlAttribute()]
		[RequiredTag(MinimumAmount=1)] public string name;

		/// <remarks/>
		[XmlAttribute()] public propertyAccess access;

		/// <remarks/>
		[XmlIgnore()] public bool accessSpecified;

		/// <remarks/>
		[XmlAttribute()]
		[DefaultValue(cascadeStyle.none)] public cascadeStyle cascade = cascadeStyle.none;

		/// <remarks/>
		[XmlAttribute("meta-type")] public string metatype;

		/// <remarks/>
		[XmlAttribute("id-type")]
		[RequiredTag(MinimumAmount=1)] public string idtype;
	}

	/// <remarks/>
	[XmlType(Namespace="urn:nhibernate-mapping-2.0")]
	[XmlRoot(Namespace="urn:nhibernate-mapping-2.0", IsNullable=false)]

	public class list
	{
		/// <remarks/>
		[XmlElement("meta")] public meta[] meta;

		/// <remarks/>
		[XmlElement("jcs-cache")] public jcscache jcscache;

		/// <remarks/>
		[RequiredTag(MinimumAmount=1)] public key key;

		/// <remarks/>
		[RequiredTag(MinimumAmount=1)] public index index;

		/// <remarks/>
		[XmlElement("many-to-many", typeof (manytomany))]
		[XmlElement("composite-element", typeof (compositeelement))]
		[XmlElement("many-to-any", typeof (manytoany))]
		[XmlElement("element", typeof (element))]
		[XmlElement("one-to-many", typeof (onetomany))]
		[RequiredTag(MinimumAmount=1)] public object Item;

		/// <remarks/>
		[XmlAttribute()]
		[RequiredTag(MinimumAmount=1)] public string name;

		/// <remarks/>
		[XmlAttribute()] public propertyAccess access;

		/// <remarks/>
		[XmlIgnore()] public bool accessSpecified;

		/// <remarks/>
		[XmlAttribute()] public string table;

		/// <remarks/>
		[XmlAttribute()] public string schema;

		/// <remarks/>
		[XmlAttribute()]
		[DefaultValue(false)] public bool lazy = false;

		/// <remarks/>
		[XmlAttribute()]
		[DefaultValue(false)] public bool inverse = false;

		/// <remarks/>
		[XmlAttribute()] public cascadeStyle cascade;

		/// <remarks/>
		[XmlIgnore()] public bool cascadeSpecified;

		/// <remarks/>
		[XmlAttribute()] public string where;
	}

	/// <remarks/>
	[XmlType(Namespace="urn:nhibernate-mapping-2.0")]
	[XmlRoot("primitive-array", Namespace="urn:nhibernate-mapping-2.0", IsNullable=false)]

	public class primitivearray
	{
		/// <remarks/>
		[XmlElement("meta")] public meta[] meta;

		/// <remarks/>
		[XmlElement("jcs-cache")] public jcscache jcscache;

		/// <remarks/>
		[RequiredTag(MinimumAmount=1)] public key key;

		/// <remarks/>
		[RequiredTag(MinimumAmount=1)] public index index;

		/// <remarks/>
		[RequiredTag(MinimumAmount=1)] public element element;

		/// <remarks/>
		[XmlAttribute()]
		[RequiredTag(MinimumAmount=1)] public string name;

		/// <remarks/>
		[XmlAttribute()] public propertyAccess access;

		/// <remarks/>
		[XmlIgnore()] public bool accessSpecified;

		/// <remarks/>
		[XmlAttribute()] public string table;

		/// <remarks/>
		[XmlAttribute()] public string schema;

		/// <remarks/>
		[XmlAttribute()] public string where;
	}

	/// <remarks/>
	[XmlType(Namespace="urn:nhibernate-mapping-2.0")]
	[XmlRoot(Namespace="urn:nhibernate-mapping-2.0", IsNullable=false)]

	public class idbag
	{
		/// <remarks/>
		[XmlElement("meta")] public meta[] meta;

		/// <remarks/>
		[XmlElement("jcs-cache")] public jcscache jcscache;

		/// <remarks/>
		[XmlElement("collection-id")]
		[RequiredTag(MinimumAmount=1)] public collectionid collectionid;

		/// <remarks/>
		[RequiredTag(MinimumAmount=1)] public key key;

		/// <remarks/>
		[XmlElement("many-to-many", typeof (manytomany))]
		[XmlElement("composite-element", typeof (compositeelement))]
		[XmlElement("many-to-any", typeof (manytoany))]
		[XmlElement("element", typeof (element))]
		[RequiredTag(MinimumAmount=1)] public object Item;

		/// <remarks/>
		[XmlAttribute()]
		[RequiredTag(MinimumAmount=1)] public string name;

		/// <remarks/>
		[XmlAttribute()] public propertyAccess access;

		/// <remarks/>
		[XmlIgnore()] public bool accessSpecified;

		/// <remarks/>
		[XmlAttribute()] public string table;

		/// <remarks/>
		[XmlAttribute()] public string schema;

		/// <remarks/>
		[XmlAttribute()]
		[DefaultValue(false)] public bool lazy = false;

		/// <remarks/>
		[XmlAttribute()] public cascadeStyle cascade;

		/// <remarks/>
		[XmlIgnore()] public bool cascadeSpecified;

		/// <remarks/>
		[XmlAttribute("order-by")] public string orderby;

		/// <remarks/>
		[XmlAttribute()] public string where;
	}

	/// <remarks/>
	[XmlType(Namespace="urn:nhibernate-mapping-2.0")]
	[XmlRoot("collection-id", Namespace="urn:nhibernate-mapping-2.0", IsNullable=false)]

	public class collectionid
	{
		/// <remarks/>
		[XmlElement("meta")] public meta[] meta;

		/// <remarks/>
		[XmlElement("column")]
		[RequiredTag(MinimumAmount=1)] public column[] column;

		/// <remarks/>
		[RequiredTag(MinimumAmount=1)] public generator generator;

		/// <remarks/>
		[XmlAttribute("column")] public string column1;

		/// <remarks/>
		[XmlAttribute()]
		[RequiredTag(MinimumAmount=1)] public string type;

		/// <remarks/>
		[XmlAttribute(DataType="positiveInteger")] public string length;
	}

	/// <remarks/>
	[XmlType(Namespace="urn:nhibernate-mapping-2.0")]
	[XmlRoot(Namespace="urn:nhibernate-mapping-2.0", IsNullable=false)]

	public class generator
	{
		/// <remarks/>
		[XmlElement("param")] public param[] param;

		/// <remarks/>
		[XmlAttribute()]
		[RequiredTag(MinimumAmount=1)] public string @class;
	}

	/// <remarks/>
	[XmlType(Namespace="urn:nhibernate-mapping-2.0")]
	[XmlRoot(Namespace="urn:nhibernate-mapping-2.0", IsNullable=false)]

	public class param
	{
		/// <remarks/>
		[XmlAttribute()]
		[RequiredTag(MinimumAmount=1)] public string name;

		/// <remarks/>
		[XmlText()] public string[] Text;
	}

	/// <remarks/>
	[XmlType(Namespace="urn:nhibernate-mapping-2.0")]
	[XmlRoot(Namespace="urn:nhibernate-mapping-2.0", IsNullable=false)]

	public class subclass
	{
		/// <remarks/>
		[XmlElement("meta")] public meta[] meta;

		/// <remarks/>
		[XmlElement("one-to-one", typeof (onetoone))]
		[XmlElement("bag", typeof (bag))]
		[XmlElement("map", typeof (map))]
		[XmlElement("property", typeof (property))]
		[XmlElement("component", typeof (component))]
		[XmlElement("many-to-one", typeof (manytoone))]
		[XmlElement("set", typeof (set))]
		[XmlElement("array", typeof (array))]
		[XmlElement("any", typeof (any))]
		[XmlElement("idbag", typeof (idbag))]
		[XmlElement("list", typeof (list))]
		[XmlElement("primitive-array", typeof (primitivearray))] public object[] Items;

		/// <remarks/>
		[XmlElement("subclass")] public subclass[] subclass1;

		/// <remarks/>
		[XmlAttribute()]
		[RequiredTag(MinimumAmount=1)] public string name;

		/// <remarks/>
		[XmlAttribute()] public string proxy;

		/// <remarks/>
		[XmlAttribute("discriminator-value")] public string discriminatorvalue;

		/// <remarks/>
		[XmlAttribute("dynamic-update")]
		[DefaultValue(false)] public bool dynamicupdate = false;

		/// <remarks/>
		[XmlAttribute("dynamic-insert")]
		[DefaultValue(false)] public bool dynamicinsert = false;

		/// <remarks/>
		[XmlAttribute()] public string extends;
	}

	/// <remarks/>
	[XmlType(Namespace="urn:nhibernate-mapping-2.0")]
	[XmlRoot(Namespace="urn:nhibernate-mapping-2.0", IsNullable=false)]

	public class timestamp
	{
		/// <remarks/>
		[XmlAttribute()]
		[RequiredTag(MinimumAmount=1)] public string name;

		/// <remarks/>
		[XmlAttribute()] public propertyAccess access;

		/// <remarks/>
		[XmlIgnore()] public bool accessSpecified;

		/// <remarks/>
		[XmlAttribute()] public string column;
	}

	/// <remarks/>
	[XmlType(Namespace="urn:nhibernate-mapping-2.0")]
	[XmlRoot(Namespace="urn:nhibernate-mapping-2.0", IsNullable=false)]

	public class version
	{
		/// <remarks/>
		[XmlAttribute()]
		[RequiredTag(MinimumAmount=1)] public string name;

		/// <remarks/>
		[XmlAttribute()] public propertyAccess access;

		/// <remarks/>
		[XmlIgnore()] public bool accessSpecified;

		/// <remarks/>
		[XmlAttribute()] public string column;

		/// <remarks/>
		[XmlAttribute()]
		[DefaultValue("Int32")] public string type = "Int32";
	}

	/// <remarks/>
	[XmlType(Namespace="urn:nhibernate-mapping-2.0")]
	[XmlRoot(Namespace="urn:nhibernate-mapping-2.0", IsNullable=false)]

	public class discriminator
	{
		/// <remarks/>
		public column column;

		/// <remarks/>
		[XmlAttribute("column")] public string column1;

		/// <remarks/>
		[XmlAttribute()]
		[DefaultValue("String")] public string type = "String";

		/// <remarks/>
		[XmlAttribute("not-null")]
		[DefaultValue(true)] public bool notnull = true;

		/// <remarks/>
		[XmlAttribute(DataType="positiveInteger")] public string length;

		/// <remarks/>
		[XmlAttribute()]
		[DefaultValue(false)] public bool force = false;
	}

	/// <remarks/>
	[XmlType(Namespace="urn:nhibernate-mapping-2.0")]
	[XmlRoot("composite-id", Namespace="urn:nhibernate-mapping-2.0", IsNullable=false)]

	public class compositeid
	{
		/// <remarks/>
		[XmlElement("key-property", typeof (keyproperty))]
		[XmlElement("key-many-to-one", typeof (keymanytoone))]
		[RequiredTag(MinimumAmount=1)] public object[] Items;

		/// <remarks/>
		[XmlAttribute()] public string @class;

		/// <remarks/>
		[XmlAttribute()] public string name;

		/// <remarks/>
		[XmlAttribute()] public propertyAccess access;

		/// <remarks/>
		[XmlIgnore()] public bool accessSpecified;

		/// <remarks/>
		[XmlAttribute("unsaved-value")]
		[DefaultValue(compositeidUnsavedvalue.none)] public compositeidUnsavedvalue unsavedvalue = compositeidUnsavedvalue.none;
	}

	/// <remarks/>
	[XmlType(Namespace="urn:nhibernate-mapping-2.0")]
	public enum compositeidUnsavedvalue
	{
		/// <remarks/>
		any,

		/// <remarks/>
		none,
	}

	/// <remarks/>
	[XmlType(Namespace="urn:nhibernate-mapping-2.0")]
	[XmlRoot(Namespace="urn:nhibernate-mapping-2.0", IsNullable=false)]

	public class id
	{
		/// <remarks/>
		[XmlElement("meta")] public meta[] meta;

		/// <remarks/>
		[XmlElement("column")] public column[] column;

		/// <remarks/>
		[RequiredTag(MinimumAmount=1)] public generator generator;

		/// <remarks/>
		[XmlAttribute()] public string name;

		/// <remarks/>
		[XmlAttribute()] public propertyAccess access;

		/// <remarks/>
		[XmlIgnore()] public bool accessSpecified;

		/// <remarks/>
		[XmlAttribute("column")] public string column1;

		/// <remarks/>
		[XmlAttribute()] public string type;

		/// <remarks/>
		[XmlAttribute(DataType="positiveInteger")] public string length;

		/// <remarks/>
		[XmlAttribute("unsaved-value")]
		[DefaultValue("null")] public string unsavedvalue = "null";
	}

	/// <remarks/>
	[XmlType(Namespace="urn:nhibernate-mapping-2.0")]
	[XmlRoot(Namespace="urn:nhibernate-mapping-2.0", IsNullable=false)]

	public class @class
	{
		/// <remarks/>
		[XmlElement("meta")] public meta[] meta;

		/// <remarks/>
		[XmlElement("jcs-cache")] public jcscache jcscache;

		/// <remarks/>
		[XmlElement("composite-id", typeof (compositeid))]
		[XmlElement("id", typeof (id))]
		[RequiredTag(MinimumAmount=1)] public object Item;

		/// <remarks/>
		public discriminator discriminator;

		/// <remarks/>
		[XmlElement("timestamp", typeof (timestamp))]
		[XmlElement("version", typeof (version))] public object Item1;

		/// <remarks/>
		[XmlElement("one-to-one", typeof (onetoone))]
		[XmlElement("bag", typeof (bag))]
		[XmlElement("map", typeof (map))]
		[XmlElement("property", typeof (property))]
		[XmlElement("component", typeof (component))]
		[XmlElement("many-to-one", typeof (manytoone))]
		[XmlElement("set", typeof (set))]
		[XmlElement("array", typeof (array))]
		[XmlElement("any", typeof (any))]
		[XmlElement("idbag", typeof (idbag))]
		[XmlElement("list", typeof (list))]
		[XmlElement("primitive-array", typeof (primitivearray))] public object[] Items;

		/// <remarks/>
		[XmlElement("subclass", typeof (subclass))]
		[XmlElement("joined-subclass", typeof (joinedsubclass))] public object[] Items1;

		/// <remarks/>
		[XmlAttribute()]
		[RequiredTag(MinimumAmount=1)] public string name;

		/// <remarks/>
		[XmlAttribute()] public string table;

		/// <remarks/>
		[XmlAttribute()] public string schema;

		/// <remarks/>
		[XmlAttribute()] public string proxy;

		/// <remarks/>
		[XmlAttribute("discriminator-value")] public string discriminatorvalue;

		/// <remarks/>
		[XmlAttribute()]
		[DefaultValue(true)] public bool mutable = true;

		/// <remarks/>
		[XmlAttribute()]
		[DefaultValue(polymorphismType.@implicit)] public polymorphismType polymorphism = polymorphismType.@implicit;

		/// <remarks/>
		[XmlAttribute()] public string where;

		/// <remarks/>
		[XmlAttribute()] public string persister;

		/// <remarks/>
		[XmlAttribute("dynamic-update")]
		[DefaultValue(false)] public bool dynamicupdate = false;

		/// <remarks/>
		[XmlAttribute("dynamic-insert")]
		[DefaultValue(false)] public bool dynamicinsert = false;
	}

	/// <remarks/>
	[XmlType(Namespace="urn:nhibernate-mapping-2.0")]
	public enum polymorphismType
	{
		/// <remarks/>
		@implicit,

		/// <remarks/>
		@explicit,
	}

	/// <remarks/>
	[XmlType(Namespace="urn:nhibernate-mapping-2.0")]
	[XmlRoot(Namespace="urn:nhibernate-mapping-2.0", IsNullable=false)]

	public class import
	{
		/// <remarks/>
		[XmlAttribute()]
		[RequiredTag(MinimumAmount=1)] public string @class;

		/// <remarks/>
		[XmlAttribute()] public string rename;
	}
}