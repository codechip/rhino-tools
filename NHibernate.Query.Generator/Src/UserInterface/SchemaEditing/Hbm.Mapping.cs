namespace NHibernate.Mapping.Hbm {
    using Ayende.NHibernateQueryAnalyzer.SchemaEditing;
    
    
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:nhibernate-mapping-2.0")]
    [System.Xml.Serialization.XmlRootAttribute("hibernate-mapping", Namespace="urn:nhibernate-mapping-2.0", IsNullable=false)]
    public class hibernatemapping {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("meta")]
        public meta[] meta;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("import")]
        public import[] import;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("class", typeof(@class))]
        [System.Xml.Serialization.XmlElementAttribute("subclass", typeof(subclass))]
        [System.Xml.Serialization.XmlElementAttribute("joined-subclass", typeof(joinedsubclass))]
        public object[] Items;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("query")]
        public query[] query;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("sql-query")]
        public sqlquery[] sqlquery;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string schema;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("default-cascade")]
        [System.ComponentModel.DefaultValueAttribute(cascadeStyle.none)]
        public cascadeStyle defaultcascade = cascadeStyle.none;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("default-access")]
        [System.ComponentModel.DefaultValueAttribute("property")]
        public string defaultaccess = "property";
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("auto-import")]
        [System.ComponentModel.DefaultValueAttribute(true)]
        public bool autoimport = true;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string @namespace;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string assembly;
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:nhibernate-mapping-2.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="urn:nhibernate-mapping-2.0", IsNullable=false)]
    public class meta {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [RequiredTag(MinimumAmount=1)]
        public string attribute;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(true)]
        public bool inherit = true;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string[] Text;
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(TypeName="sql-querySynchronize", Namespace="urn:nhibernate-mapping-2.0")]
    public class sqlquerySynchronize {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string table;
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(TypeName="sql-queryReturn", Namespace="urn:nhibernate-mapping-2.0")]
    public class sqlqueryReturn {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string alias;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string @class;
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(TypeName="sql-query", Namespace="urn:nhibernate-mapping-2.0")]
    public class sqlquery {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("return")]
        public sqlqueryReturn[] @return;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("synchronize")]
        public sqlquerySynchronize[] synchronize;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string[] Text;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name;
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:nhibernate-mapping-2.0")]
    public class query {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value;
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:nhibernate-mapping-2.0")]
    [System.Xml.Serialization.XmlRootAttribute("joined-subclass", Namespace="urn:nhibernate-mapping-2.0", IsNullable=false)]
    public class joinedsubclass {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("meta")]
        public meta[] meta;
        
        /// <remarks/>
        [RequiredTag(MinimumAmount=1)]
        public key key;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("one-to-one", typeof(onetoone))]
        [System.Xml.Serialization.XmlElementAttribute("bag", typeof(bag))]
        [System.Xml.Serialization.XmlElementAttribute("map", typeof(map))]
        [System.Xml.Serialization.XmlElementAttribute("property", typeof(property))]
        [System.Xml.Serialization.XmlElementAttribute("component", typeof(component))]
        [System.Xml.Serialization.XmlElementAttribute("many-to-one", typeof(manytoone))]
        [System.Xml.Serialization.XmlElementAttribute("set", typeof(set))]
        [System.Xml.Serialization.XmlElementAttribute("array", typeof(array))]
        [System.Xml.Serialization.XmlElementAttribute("any", typeof(any))]
        [System.Xml.Serialization.XmlElementAttribute("idbag", typeof(idbag))]
        [System.Xml.Serialization.XmlElementAttribute("list", typeof(list))]
        [System.Xml.Serialization.XmlElementAttribute("primitive-array", typeof(primitivearray))]
        public object[] Items;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("joined-subclass")]
        public joinedsubclass[] joinedsubclass1;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [RequiredTag(MinimumAmount=1)]
        public string name;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string proxy;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool lazy;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool lazySpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("dynamic-update")]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool dynamicupdate = false;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("dynamic-insert")]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool dynamicinsert = false;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string extends;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string schema;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string table;
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:nhibernate-mapping-2.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="urn:nhibernate-mapping-2.0", IsNullable=false)]
    public class key {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("column")]
        public column[] column;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("column")]
        public string column1;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("foreign-key")]
        public string foreignkey;
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:nhibernate-mapping-2.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="urn:nhibernate-mapping-2.0", IsNullable=false)]
    public class column {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [RequiredTag(MinimumAmount=1)]
        public string name;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType="positiveInteger")]
        public string length;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("not-null")]
        public bool notnull;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool notnullSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool unique;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool uniqueSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("unique-key")]
        public string uniquekey;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("sql-type")]
        public string sqltype;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string index;
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:nhibernate-mapping-2.0")]
    [System.Xml.Serialization.XmlRootAttribute("one-to-one", Namespace="urn:nhibernate-mapping-2.0", IsNullable=false)]
    public class onetoone {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("meta")]
        public meta[] meta;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [RequiredTag(MinimumAmount=1)]
        public string name;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string access;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string @class;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public cascadeStyle cascade;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool cascadeSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("outer-join")]
        [System.ComponentModel.DefaultValueAttribute(outerJoinStrategy.auto)]
        public outerJoinStrategy outerjoin = outerJoinStrategy.auto;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool constrained = false;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("foreign-key")]
        public string foreignkey;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("property-ref")]
        public string propertyref;
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:nhibernate-mapping-2.0")]
    public enum cascadeStyle {
        
        /// <remarks/>
        all,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("all-delete-orphan")]
        alldeleteorphan,
        
        /// <remarks/>
        none,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("save-update")]
        saveupdate,
        
        /// <remarks/>
        delete,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("delete-orphan")]
        deleteorphan,
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:nhibernate-mapping-2.0")]
    public enum outerJoinStrategy {
        
        /// <remarks/>
        auto,
        
        /// <remarks/>
        @true,
        
        /// <remarks/>
        @false,
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:nhibernate-mapping-2.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="urn:nhibernate-mapping-2.0", IsNullable=false)]
    public class bag {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("meta")]
        public meta[] meta;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("jcs-cache", typeof(cacheType))]
        [System.Xml.Serialization.XmlElementAttribute("cache", typeof(cacheType))]
        [System.Xml.Serialization.XmlChoiceIdentifierAttribute("ItemElementName")]
        public cacheType Item;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public ItemChoiceType4 ItemElementName;
        
        /// <remarks/>
        [RequiredTag(MinimumAmount=1)]
        public key key;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("many-to-many", typeof(manytomany))]
        [System.Xml.Serialization.XmlElementAttribute("composite-element", typeof(compositeelement))]
        [System.Xml.Serialization.XmlElementAttribute("many-to-any", typeof(manytoany))]
        [System.Xml.Serialization.XmlElementAttribute("element", typeof(element))]
        [System.Xml.Serialization.XmlElementAttribute("one-to-many", typeof(onetomany))]
        [RequiredTag(MinimumAmount=1)]
        public object Item1;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [RequiredTag(MinimumAmount=1)]
        public string name;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string access;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string table;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string schema;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool lazy = false;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("outer-join")]
        public outerJoinStrategy outerjoin;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool outerjoinSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public cascadeStyle cascade;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool cascadeSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string where;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool inverse = false;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string persister;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("order-by")]
        public string orderby;
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:nhibernate-mapping-2.0")]
    [System.Xml.Serialization.XmlRootAttribute("cache", Namespace="urn:nhibernate-mapping-2.0", IsNullable=false)]
    public class cacheType {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public cacheTypeUsage usage;
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:nhibernate-mapping-2.0")]
    public enum cacheTypeUsage {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("read-only")]
        @readonly,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("read-write")]
        readwrite,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("nonstrict-read-write")]
        nonstrictreadwrite,
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:nhibernate-mapping-2.0", IncludeInSchema=false)]
    public enum ItemChoiceType4 {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("jcs-cache")]
        jcscache,
        
        /// <remarks/>
        cache,
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:nhibernate-mapping-2.0")]
    [System.Xml.Serialization.XmlRootAttribute("many-to-many", Namespace="urn:nhibernate-mapping-2.0", IsNullable=false)]
    public class manytomany {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("meta")]
        public meta[] meta;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("column")]
        public column[] column;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [RequiredTag(MinimumAmount=1)]
        public string @class;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("column")]
        public string column1;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("foreign-key")]
        public string foreignkey;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("outer-join")]
        [System.ComponentModel.DefaultValueAttribute(outerJoinStrategy.auto)]
        public outerJoinStrategy outerjoin = outerJoinStrategy.auto;
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:nhibernate-mapping-2.0")]
    [System.Xml.Serialization.XmlRootAttribute("composite-element", Namespace="urn:nhibernate-mapping-2.0", IsNullable=false)]
    public class compositeelement {
        
        /// <remarks/>
        public parent parent;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("property", typeof(property))]
        [System.Xml.Serialization.XmlElementAttribute("many-to-one", typeof(manytoone))]
        [System.Xml.Serialization.XmlElementAttribute("nested-composite-element", typeof(nestedcompositeelement))]
        public object[] Items;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [RequiredTag(MinimumAmount=1)]
        public string @class;
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:nhibernate-mapping-2.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="urn:nhibernate-mapping-2.0", IsNullable=false)]
    public class parent {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [RequiredTag(MinimumAmount=1)]
        public string name;
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:nhibernate-mapping-2.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="urn:nhibernate-mapping-2.0", IsNullable=false)]
    public class property {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("meta")]
        public meta[] meta;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("column")]
        public column[] column;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [RequiredTag(MinimumAmount=1)]
        public string name;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string access;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string type;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("column")]
        public string column1;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType="positiveInteger")]
        public string length;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("not-null")]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool notnull = false;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool unique = false;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(true)]
        public bool update = true;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(true)]
        public bool insert = true;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string formula;
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:nhibernate-mapping-2.0")]
    [System.Xml.Serialization.XmlRootAttribute("many-to-one", Namespace="urn:nhibernate-mapping-2.0", IsNullable=false)]
    public class manytoone {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("meta")]
        public meta[] meta;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("column")]
        public column[] column;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [RequiredTag(MinimumAmount=1)]
        public string name;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string access;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string @class;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("column")]
        public string column1;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("not-null")]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool notnull = false;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool unique = false;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public cascadeStyle cascade;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool cascadeSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("outer-join")]
        [System.ComponentModel.DefaultValueAttribute(outerJoinStrategy.auto)]
        public outerJoinStrategy outerjoin = outerJoinStrategy.auto;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(true)]
        public bool update = true;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(true)]
        public bool insert = true;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("foreign-key")]
        public string foreignkey;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("property-ref")]
        public string propertyref;
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:nhibernate-mapping-2.0")]
    [System.Xml.Serialization.XmlRootAttribute("nested-composite-element", Namespace="urn:nhibernate-mapping-2.0", IsNullable=false)]
    public class nestedcompositeelement {
        
        /// <remarks/>
        public parent parent;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("property", typeof(property))]
        [System.Xml.Serialization.XmlElementAttribute("many-to-one", typeof(manytoone))]
        [System.Xml.Serialization.XmlElementAttribute("nested-composite-element", typeof(nestedcompositeelement))]
        public object[] Items;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [RequiredTag(MinimumAmount=1)]
        public string @class;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [RequiredTag(MinimumAmount=1)]
        public string name;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string access;
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:nhibernate-mapping-2.0")]
    [System.Xml.Serialization.XmlRootAttribute("many-to-any", Namespace="urn:nhibernate-mapping-2.0", IsNullable=false)]
    public class manytoany {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("column")]
        [RequiredTag(MinimumAmount=1)]
        public column[] column;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("id-type")]
        [RequiredTag(MinimumAmount=1)]
        public string idtype;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("meta-type")]
        public string metatype;
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:nhibernate-mapping-2.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="urn:nhibernate-mapping-2.0", IsNullable=false)]
    public class element {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("column")]
        public column[] column;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("column")]
        public string column1;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [RequiredTag(MinimumAmount=1)]
        public string type;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType="positiveInteger")]
        public string length;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("not-null")]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool notnull = false;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool unique = false;
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:nhibernate-mapping-2.0")]
    [System.Xml.Serialization.XmlRootAttribute("one-to-many", Namespace="urn:nhibernate-mapping-2.0", IsNullable=false)]
    public class onetomany {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [RequiredTag(MinimumAmount=1)]
        public string @class;
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:nhibernate-mapping-2.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="urn:nhibernate-mapping-2.0", IsNullable=false)]
    public class map {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("meta")]
        public meta[] meta;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("jcs-cache", typeof(cacheType))]
        [System.Xml.Serialization.XmlElementAttribute("cache", typeof(cacheType))]
        [System.Xml.Serialization.XmlChoiceIdentifierAttribute("ItemElementName")]
        public cacheType Item;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public ItemChoiceType1 ItemElementName;
        
        /// <remarks/>
        [RequiredTag(MinimumAmount=1)]
        public key key;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("index", typeof(index))]
        [System.Xml.Serialization.XmlElementAttribute("index-many-to-many", typeof(indexmanytomany))]
        [System.Xml.Serialization.XmlElementAttribute("composite-index", typeof(compositeindex))]
        [System.Xml.Serialization.XmlElementAttribute("index-many-to-any", typeof(indexmanytoany))]
        [RequiredTag(MinimumAmount=1)]
        public object Item1;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("many-to-many", typeof(manytomany))]
        [System.Xml.Serialization.XmlElementAttribute("composite-element", typeof(compositeelement))]
        [System.Xml.Serialization.XmlElementAttribute("many-to-any", typeof(manytoany))]
        [System.Xml.Serialization.XmlElementAttribute("element", typeof(element))]
        [System.Xml.Serialization.XmlElementAttribute("one-to-many", typeof(onetomany))]
        [RequiredTag(MinimumAmount=1)]
        public object Item2;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [RequiredTag(MinimumAmount=1)]
        public string name;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string access;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string table;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string schema;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool lazy = false;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("outer-join")]
        public outerJoinStrategy outerjoin;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool outerjoinSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public cascadeStyle cascade;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool cascadeSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string where;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool inverse = false;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string persister;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("order-by")]
        public string orderby;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute("unsorted")]
        public string sort = "unsorted";
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:nhibernate-mapping-2.0", IncludeInSchema=false)]
    public enum ItemChoiceType1 {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("jcs-cache")]
        jcscache,
        
        /// <remarks/>
        cache,
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:nhibernate-mapping-2.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="urn:nhibernate-mapping-2.0", IsNullable=false)]
    public class index {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("column")]
        public column[] column;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("column")]
        public string column1;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string type;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType="positiveInteger")]
        public string length;
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:nhibernate-mapping-2.0")]
    [System.Xml.Serialization.XmlRootAttribute("index-many-to-many", Namespace="urn:nhibernate-mapping-2.0", IsNullable=false)]
    public class indexmanytomany {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("column")]
        public column[] column;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [RequiredTag(MinimumAmount=1)]
        public string @class;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("column")]
        public string column1;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("foreign-key")]
        public string foreignkey;
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:nhibernate-mapping-2.0")]
    [System.Xml.Serialization.XmlRootAttribute("composite-index", Namespace="urn:nhibernate-mapping-2.0", IsNullable=false)]
    public class compositeindex {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("key-property", typeof(keyproperty))]
        [System.Xml.Serialization.XmlElementAttribute("key-many-to-one", typeof(keymanytoone))]
        [RequiredTag(MinimumAmount=1)]
        public object[] Items;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [RequiredTag(MinimumAmount=1)]
        public string @class;
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:nhibernate-mapping-2.0")]
    [System.Xml.Serialization.XmlRootAttribute("key-property", Namespace="urn:nhibernate-mapping-2.0", IsNullable=false)]
    public class keyproperty {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("column")]
        public column[] column;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [RequiredTag(MinimumAmount=1)]
        public string name;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string access;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string type;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("column")]
        public string column1;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType="positiveInteger")]
        public string length;
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:nhibernate-mapping-2.0")]
    [System.Xml.Serialization.XmlRootAttribute("key-many-to-one", Namespace="urn:nhibernate-mapping-2.0", IsNullable=false)]
    public class keymanytoone {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("column")]
        public column[] column;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [RequiredTag(MinimumAmount=1)]
        public string name;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string access;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string @class;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("column")]
        public string column1;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("foreign-key")]
        public string foreignkey;
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:nhibernate-mapping-2.0")]
    [System.Xml.Serialization.XmlRootAttribute("index-many-to-any", Namespace="urn:nhibernate-mapping-2.0", IsNullable=false)]
    public class indexmanytoany {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("column")]
        [RequiredTag(MinimumAmount=1)]
        public column[] column;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("id-type")]
        [RequiredTag(MinimumAmount=1)]
        public string idtype;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("meta-type")]
        public string metatype;
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:nhibernate-mapping-2.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="urn:nhibernate-mapping-2.0", IsNullable=false)]
    public class component {
        
        /// <remarks/>
        public parent parent;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("one-to-one", typeof(onetoone))]
        [System.Xml.Serialization.XmlElementAttribute("bag", typeof(bag))]
        [System.Xml.Serialization.XmlElementAttribute("map", typeof(map))]
        [System.Xml.Serialization.XmlElementAttribute("property", typeof(property))]
        [System.Xml.Serialization.XmlElementAttribute("component", typeof(component))]
        [System.Xml.Serialization.XmlElementAttribute("many-to-one", typeof(manytoone))]
        [System.Xml.Serialization.XmlElementAttribute("set", typeof(set))]
        [System.Xml.Serialization.XmlElementAttribute("array", typeof(array))]
        [System.Xml.Serialization.XmlElementAttribute("any", typeof(any))]
        [System.Xml.Serialization.XmlElementAttribute("list", typeof(list))]
        [System.Xml.Serialization.XmlElementAttribute("primitive-array", typeof(primitivearray))]
        public object[] Items;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [RequiredTag(MinimumAmount=1)]
        public string name;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string access;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string @class;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(true)]
        public bool update = true;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(true)]
        public bool insert = true;
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:nhibernate-mapping-2.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="urn:nhibernate-mapping-2.0", IsNullable=false)]
    public class set {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("meta")]
        public meta[] meta;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("jcs-cache", typeof(cacheType))]
        [System.Xml.Serialization.XmlElementAttribute("cache", typeof(cacheType))]
        [System.Xml.Serialization.XmlChoiceIdentifierAttribute("ItemElementName")]
        public cacheType Item;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public ItemChoiceType2 ItemElementName;
        
        /// <remarks/>
        [RequiredTag(MinimumAmount=1)]
        public key key;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("many-to-many", typeof(manytomany))]
        [System.Xml.Serialization.XmlElementAttribute("composite-element", typeof(compositeelement))]
        [System.Xml.Serialization.XmlElementAttribute("many-to-any", typeof(manytoany))]
        [System.Xml.Serialization.XmlElementAttribute("element", typeof(element))]
        [System.Xml.Serialization.XmlElementAttribute("one-to-many", typeof(onetomany))]
        [RequiredTag(MinimumAmount=1)]
        public object Item1;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [RequiredTag(MinimumAmount=1)]
        public string name;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string access;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string table;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string schema;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool lazy = false;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("outer-join")]
        public outerJoinStrategy outerjoin;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool outerjoinSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public cascadeStyle cascade;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool cascadeSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string where;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool inverse = false;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string persister;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("order-by")]
        public string orderby;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute("unsorted")]
        public string sort = "unsorted";
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:nhibernate-mapping-2.0", IncludeInSchema=false)]
    public enum ItemChoiceType2 {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("jcs-cache")]
        jcscache,
        
        /// <remarks/>
        cache,
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:nhibernate-mapping-2.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="urn:nhibernate-mapping-2.0", IsNullable=false)]
    public class array {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("meta")]
        public meta[] meta;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("jcs-cache", typeof(cacheType))]
        [System.Xml.Serialization.XmlElementAttribute("cache", typeof(cacheType))]
        [System.Xml.Serialization.XmlChoiceIdentifierAttribute("ItemElementName")]
        public cacheType Item;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public ItemChoiceType5 ItemElementName;
        
        /// <remarks/>
        [RequiredTag(MinimumAmount=1)]
        public key key;
        
        /// <remarks/>
        [RequiredTag(MinimumAmount=1)]
        public index index;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("many-to-many", typeof(manytomany))]
        [System.Xml.Serialization.XmlElementAttribute("composite-element", typeof(compositeelement))]
        [System.Xml.Serialization.XmlElementAttribute("many-to-any", typeof(manytoany))]
        [System.Xml.Serialization.XmlElementAttribute("element", typeof(element))]
        [System.Xml.Serialization.XmlElementAttribute("one-to-many", typeof(onetomany))]
        [RequiredTag(MinimumAmount=1)]
        public object Item1;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [RequiredTag(MinimumAmount=1)]
        public string name;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string access;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string table;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string schema;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("element-class")]
        public string elementclass;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public cascadeStyle cascade;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool cascadeSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string where;
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:nhibernate-mapping-2.0", IncludeInSchema=false)]
    public enum ItemChoiceType5 {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("jcs-cache")]
        jcscache,
        
        /// <remarks/>
        cache,
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:nhibernate-mapping-2.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="urn:nhibernate-mapping-2.0", IsNullable=false)]
    public class any {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("column")]
        [RequiredTag(MinimumAmount=1)]
        public column[] column;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [RequiredTag(MinimumAmount=1)]
        public string name;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string access;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(cascadeStyle.none)]
        public cascadeStyle cascade = cascadeStyle.none;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("meta-type")]
        public string metatype;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("id-type")]
        [RequiredTag(MinimumAmount=1)]
        public string idtype;
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:nhibernate-mapping-2.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="urn:nhibernate-mapping-2.0", IsNullable=false)]
    public class list {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("meta")]
        public meta[] meta;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("jcs-cache", typeof(cacheType))]
        [System.Xml.Serialization.XmlElementAttribute("cache", typeof(cacheType))]
        [System.Xml.Serialization.XmlChoiceIdentifierAttribute("ItemElementName")]
        public cacheType Item;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public ItemChoiceType3 ItemElementName;
        
        /// <remarks/>
        [RequiredTag(MinimumAmount=1)]
        public key key;
        
        /// <remarks/>
        [RequiredTag(MinimumAmount=1)]
        public index index;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("many-to-many", typeof(manytomany))]
        [System.Xml.Serialization.XmlElementAttribute("composite-element", typeof(compositeelement))]
        [System.Xml.Serialization.XmlElementAttribute("many-to-any", typeof(manytoany))]
        [System.Xml.Serialization.XmlElementAttribute("element", typeof(element))]
        [System.Xml.Serialization.XmlElementAttribute("one-to-many", typeof(onetomany))]
        [RequiredTag(MinimumAmount=1)]
        public object Item1;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [RequiredTag(MinimumAmount=1)]
        public string name;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string access;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string table;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string schema;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool lazy = false;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("outer-join")]
        public outerJoinStrategy outerjoin;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool outerjoinSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public cascadeStyle cascade;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool cascadeSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string where;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool inverse = false;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string persister;
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:nhibernate-mapping-2.0", IncludeInSchema=false)]
    public enum ItemChoiceType3 {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("jcs-cache")]
        jcscache,
        
        /// <remarks/>
        cache,
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:nhibernate-mapping-2.0")]
    [System.Xml.Serialization.XmlRootAttribute("primitive-array", Namespace="urn:nhibernate-mapping-2.0", IsNullable=false)]
    public class primitivearray {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("meta")]
        public meta[] meta;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("jcs-cache", typeof(cacheType))]
        [System.Xml.Serialization.XmlElementAttribute("cache", typeof(cacheType))]
        [System.Xml.Serialization.XmlChoiceIdentifierAttribute("ItemElementName")]
        public cacheType Item;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public ItemChoiceType6 ItemElementName;
        
        /// <remarks/>
        [RequiredTag(MinimumAmount=1)]
        public key key;
        
        /// <remarks/>
        [RequiredTag(MinimumAmount=1)]
        public index index;
        
        /// <remarks/>
        [RequiredTag(MinimumAmount=1)]
        public element element;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [RequiredTag(MinimumAmount=1)]
        public string name;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string access;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string table;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string schema;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string where;
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:nhibernate-mapping-2.0", IncludeInSchema=false)]
    public enum ItemChoiceType6 {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("jcs-cache")]
        jcscache,
        
        /// <remarks/>
        cache,
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:nhibernate-mapping-2.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="urn:nhibernate-mapping-2.0", IsNullable=false)]
    public class idbag {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("meta")]
        public meta[] meta;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("jcs-cache", typeof(cacheType))]
        [System.Xml.Serialization.XmlElementAttribute("cache", typeof(cacheType))]
        [System.Xml.Serialization.XmlChoiceIdentifierAttribute("ItemElementName")]
        public cacheType Item;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public ItemChoiceType7 ItemElementName;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("collection-id")]
        [RequiredTag(MinimumAmount=1)]
        public collectionid collectionid;
        
        /// <remarks/>
        [RequiredTag(MinimumAmount=1)]
        public key key;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("many-to-many", typeof(manytomany))]
        [System.Xml.Serialization.XmlElementAttribute("composite-element", typeof(compositeelement))]
        [System.Xml.Serialization.XmlElementAttribute("many-to-any", typeof(manytoany))]
        [System.Xml.Serialization.XmlElementAttribute("element", typeof(element))]
        [RequiredTag(MinimumAmount=1)]
        public object Item1;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [RequiredTag(MinimumAmount=1)]
        public string name;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string access;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string table;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string schema;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool lazy = false;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public cascadeStyle cascade;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool cascadeSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("order-by")]
        public string orderby;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string where;
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:nhibernate-mapping-2.0", IncludeInSchema=false)]
    public enum ItemChoiceType7 {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("jcs-cache")]
        jcscache,
        
        /// <remarks/>
        cache,
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:nhibernate-mapping-2.0")]
    [System.Xml.Serialization.XmlRootAttribute("collection-id", Namespace="urn:nhibernate-mapping-2.0", IsNullable=false)]
    public class collectionid {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("meta")]
        public meta[] meta;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("column")]
        [RequiredTag(MinimumAmount=1)]
        public column[] column;
        
        /// <remarks/>
        [RequiredTag(MinimumAmount=1)]
        public generator generator;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("column")]
        public string column1;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [RequiredTag(MinimumAmount=1)]
        public string type;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType="positiveInteger")]
        public string length;
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:nhibernate-mapping-2.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="urn:nhibernate-mapping-2.0", IsNullable=false)]
    public class generator {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("param")]
        public param[] param;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [RequiredTag(MinimumAmount=1)]
        public string @class;
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:nhibernate-mapping-2.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="urn:nhibernate-mapping-2.0", IsNullable=false)]
    public class param {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [RequiredTag(MinimumAmount=1)]
        public string name;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string[] Text;
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:nhibernate-mapping-2.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="urn:nhibernate-mapping-2.0", IsNullable=false)]
    public class subclass {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("meta")]
        public meta[] meta;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("one-to-one", typeof(onetoone))]
        [System.Xml.Serialization.XmlElementAttribute("bag", typeof(bag))]
        [System.Xml.Serialization.XmlElementAttribute("map", typeof(map))]
        [System.Xml.Serialization.XmlElementAttribute("property", typeof(property))]
        [System.Xml.Serialization.XmlElementAttribute("component", typeof(component))]
        [System.Xml.Serialization.XmlElementAttribute("many-to-one", typeof(manytoone))]
        [System.Xml.Serialization.XmlElementAttribute("set", typeof(set))]
        [System.Xml.Serialization.XmlElementAttribute("array", typeof(array))]
        [System.Xml.Serialization.XmlElementAttribute("any", typeof(any))]
        [System.Xml.Serialization.XmlElementAttribute("idbag", typeof(idbag))]
        [System.Xml.Serialization.XmlElementAttribute("list", typeof(list))]
        [System.Xml.Serialization.XmlElementAttribute("primitive-array", typeof(primitivearray))]
        public object[] Items;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("subclass")]
        public subclass[] subclass1;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [RequiredTag(MinimumAmount=1)]
        public string name;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string proxy;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool lazy;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool lazySpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("dynamic-update")]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool dynamicupdate = false;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("dynamic-insert")]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool dynamicinsert = false;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string extends;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("discriminator-value")]
        public string discriminatorvalue;
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:nhibernate-mapping-2.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="urn:nhibernate-mapping-2.0", IsNullable=false)]
    public class timestamp {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [RequiredTag(MinimumAmount=1)]
        public string name;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string access;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string column;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("unsaved-value")]
        [System.ComponentModel.DefaultValueAttribute("undefined")]
        public string unsavedvalue = "undefined";
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:nhibernate-mapping-2.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="urn:nhibernate-mapping-2.0", IsNullable=false)]
    public class version {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [RequiredTag(MinimumAmount=1)]
        public string name;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string access;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string column;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute("Int32")]
        public string type = "Int32";
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("unsaved-value")]
        [System.ComponentModel.DefaultValueAttribute("undefined")]
        public string unsavedvalue = "undefined";
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:nhibernate-mapping-2.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="urn:nhibernate-mapping-2.0", IsNullable=false)]
    public class discriminator {
        
        /// <remarks/>
        public column column;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("column")]
        public string column1;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute("String")]
        public string type = "String";
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("not-null")]
        [System.ComponentModel.DefaultValueAttribute(true)]
        public bool notnull = true;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType="positiveInteger")]
        public string length;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool force = false;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(true)]
        public bool insert = true;
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:nhibernate-mapping-2.0")]
    [System.Xml.Serialization.XmlRootAttribute("composite-id", Namespace="urn:nhibernate-mapping-2.0", IsNullable=false)]
    public class compositeid {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("key-property", typeof(keyproperty))]
        [System.Xml.Serialization.XmlElementAttribute("key-many-to-one", typeof(keymanytoone))]
        [RequiredTag(MinimumAmount=1)]
        public object[] Items;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string @class;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string access;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("unsaved-value")]
        [System.ComponentModel.DefaultValueAttribute(unsavedValueType.none)]
        public unsavedValueType unsavedvalue = unsavedValueType.none;
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:nhibernate-mapping-2.0")]
    public enum unsavedValueType {
        
        /// <remarks/>
        any,
        
        /// <remarks/>
        none,
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:nhibernate-mapping-2.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="urn:nhibernate-mapping-2.0", IsNullable=false)]
    public class id {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("meta")]
        public meta[] meta;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("column")]
        public column[] column;
        
        /// <remarks/>
        [RequiredTag(MinimumAmount=1)]
        public generator generator;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string access;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("column")]
        public string column1;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string type;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType="positiveInteger")]
        public string length;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("unsaved-value")]
        [System.ComponentModel.DefaultValueAttribute("null")]
        public string unsavedvalue = "null";
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:nhibernate-mapping-2.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="urn:nhibernate-mapping-2.0", IsNullable=false)]
    public class @class {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("meta")]
        public meta[] meta;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("jcs-cache", typeof(cacheType))]
        [System.Xml.Serialization.XmlElementAttribute("cache", typeof(cacheType))]
        [System.Xml.Serialization.XmlChoiceIdentifierAttribute("ItemElementName")]
        public cacheType Item;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public ItemChoiceType ItemElementName;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("composite-id", typeof(compositeid))]
        [System.Xml.Serialization.XmlElementAttribute("id", typeof(id))]
        [RequiredTag(MinimumAmount=1)]
        public object Item1;
        
        /// <remarks/>
        public discriminator discriminator;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("timestamp", typeof(timestamp))]
        [System.Xml.Serialization.XmlElementAttribute("version", typeof(version))]
        public object Item2;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("one-to-one", typeof(onetoone))]
        [System.Xml.Serialization.XmlElementAttribute("bag", typeof(bag))]
        [System.Xml.Serialization.XmlElementAttribute("map", typeof(map))]
        [System.Xml.Serialization.XmlElementAttribute("property", typeof(property))]
        [System.Xml.Serialization.XmlElementAttribute("component", typeof(component))]
        [System.Xml.Serialization.XmlElementAttribute("many-to-one", typeof(manytoone))]
        [System.Xml.Serialization.XmlElementAttribute("set", typeof(set))]
        [System.Xml.Serialization.XmlElementAttribute("array", typeof(array))]
        [System.Xml.Serialization.XmlElementAttribute("any", typeof(any))]
        [System.Xml.Serialization.XmlElementAttribute("idbag", typeof(idbag))]
        [System.Xml.Serialization.XmlElementAttribute("list", typeof(list))]
        [System.Xml.Serialization.XmlElementAttribute("primitive-array", typeof(primitivearray))]
        public object[] Items;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("subclass", typeof(subclass))]
        [System.Xml.Serialization.XmlElementAttribute("joined-subclass", typeof(joinedsubclass))]
        public object[] Items1;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [RequiredTag(MinimumAmount=1)]
        public string name;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string proxy;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool lazy;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool lazySpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("dynamic-update")]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool dynamicupdate = false;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("dynamic-insert")]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool dynamicinsert = false;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string table;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string schema;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("discriminator-value")]
        public string discriminatorvalue;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(true)]
        public bool mutable = true;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(polymorphismType.@implicit)]
        public polymorphismType polymorphism = polymorphismType.@implicit;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string persister;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string where;
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:nhibernate-mapping-2.0", IncludeInSchema=false)]
    public enum ItemChoiceType {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("jcs-cache")]
        jcscache,
        
        /// <remarks/>
        cache,
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:nhibernate-mapping-2.0")]
    public enum polymorphismType {
        
        /// <remarks/>
        @implicit,
        
        /// <remarks/>
        @explicit,
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:nhibernate-mapping-2.0")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="urn:nhibernate-mapping-2.0", IsNullable=false)]
    public class import {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [RequiredTag(MinimumAmount=1)]
        public string @class;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string rename;
    }
}
