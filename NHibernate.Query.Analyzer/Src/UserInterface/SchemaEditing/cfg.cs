namespace NHibernate.Mapping.Cfg {
    using Ayende.NHibernateQueryAnalyzer.SchemaEditing;
    
    
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:nhibernate-configuration-2.2")]
    [System.Xml.Serialization.XmlRootAttribute("hibernate-configuration", Namespace="urn:nhibernate-configuration-2.2", IsNullable=false)]
    public class hibernateconfiguration {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("session-factory")]
        [RequiredTag(MinimumAmount=1)]
        public sessionfactory sessionfactory;
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:nhibernate-configuration-2.2")]
    [System.Xml.Serialization.XmlRootAttribute("session-factory", Namespace="urn:nhibernate-configuration-2.2", IsNullable=false)]
    public class sessionfactory {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("property")]
        public property[] property;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("mapping")]
        public mapping[] mapping;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("jcs-collection-cache", typeof(jcscollectioncache))]
        [System.Xml.Serialization.XmlElementAttribute("jcs-class-cache", typeof(jcsclasscache))]
        public object[] Items;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name;
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:nhibernate-configuration-2.2")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="urn:nhibernate-configuration-2.2", IsNullable=false)]
    public class property {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [RequiredTag(MinimumAmount=1)]
        public string name;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string[] Text;
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:nhibernate-configuration-2.2")]
    [System.Xml.Serialization.XmlRootAttribute("jcs-collection-cache", Namespace="urn:nhibernate-configuration-2.2", IsNullable=false)]
    public class jcscollectioncache {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [RequiredTag(MinimumAmount=1)]
        public string collection;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string region;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [RequiredTag(MinimumAmount=1)]
        public jcscollectioncacheUsage usage;
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:nhibernate-configuration-2.2")]
    public enum jcscollectioncacheUsage {
        
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
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:nhibernate-configuration-2.2")]
    [System.Xml.Serialization.XmlRootAttribute("jcs-class-cache", Namespace="urn:nhibernate-configuration-2.2", IsNullable=false)]
    public class jcsclasscache {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [RequiredTag(MinimumAmount=1)]
        public string @class;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string region;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [RequiredTag(MinimumAmount=1)]
        public jcsclasscacheUsage usage;
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:nhibernate-configuration-2.2")]
    public enum jcsclasscacheUsage {
        
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
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:nhibernate-configuration-2.2")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="urn:nhibernate-configuration-2.2", IsNullable=false)]
    public class mapping {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string resource;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string file;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string assembly;
    }
}
