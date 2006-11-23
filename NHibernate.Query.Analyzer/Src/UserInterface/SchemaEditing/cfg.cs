namespace NHibernate.Mapping.Cfg {
    using Ayende.NHibernateQueryAnalyzer.SchemaEditing;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Ayende.NHibernateQueryAnalyzer.Utilities", "0.0.0.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
    [System.Xml.Serialization.XmlRootAttribute("hibernate-configuration", Namespace="urn:nhibernate-configuration-2.2", IsNullable=false)]
    public partial class hibernateconfiguration : object, System.ComponentModel.INotifyPropertyChanged {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("session-factory")]
        [RequiredTag(MinimumAmount=1)]
        public sessionfactory sessionfactory;
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Ayende.NHibernateQueryAnalyzer.Utilities", "0.0.0.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
    [System.Xml.Serialization.XmlRootAttribute("session-factory", Namespace="urn:nhibernate-configuration-2.2", IsNullable=false)]
    public partial class sessionfactory : object, System.ComponentModel.INotifyPropertyChanged {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("property")]
        public property[] property;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("mapping")]
        public mapping[] mapping;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("class-cache", typeof(classCache))]
        [System.Xml.Serialization.XmlElementAttribute("collection-cache", typeof(collectionCache))]
        [System.Xml.Serialization.XmlElementAttribute("jcs-class-cache", typeof(classCache))]
        [System.Xml.Serialization.XmlElementAttribute("jcs-collection-cache", typeof(collectionCache))]
        [System.Xml.Serialization.XmlChoiceIdentifierAttribute("ItemsElementName")]
        public object[] Items;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("ItemsElementName")]
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public ItemsChoiceType[] ItemsElementName;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name;
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Ayende.NHibernateQueryAnalyzer.Utilities", "0.0.0.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="urn:nhibernate-configuration-2.2", IsNullable=false)]
    public partial class property : object, System.ComponentModel.INotifyPropertyChanged {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [RequiredTag(MinimumAmount=1)]
        public string name;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string[] Text;
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Ayende.NHibernateQueryAnalyzer.Utilities", "0.0.0.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:nhibernate-configuration-2.2")]
    [System.Xml.Serialization.XmlRootAttribute("jcs-collection-cache", Namespace="urn:nhibernate-configuration-2.2", IsNullable=false)]
    public partial class collectionCache : object, System.ComponentModel.INotifyPropertyChanged {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string collection;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string region;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public cacheUsage usage;
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Ayende.NHibernateQueryAnalyzer.Utilities", "0.0.0.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:nhibernate-configuration-2.2")]
    public enum cacheUsage {
        
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
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Ayende.NHibernateQueryAnalyzer.Utilities", "0.0.0.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:nhibernate-configuration-2.2")]
    [System.Xml.Serialization.XmlRootAttribute("jcs-class-cache", Namespace="urn:nhibernate-configuration-2.2", IsNullable=false)]
    public partial class classCache : object, System.ComponentModel.INotifyPropertyChanged {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string @class;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string region;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public cacheUsage usage;
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Ayende.NHibernateQueryAnalyzer.Utilities", "0.0.0.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="urn:nhibernate-configuration-2.2", IsNullable=false)]
    public partial class mapping : object, System.ComponentModel.INotifyPropertyChanged {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string resource;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string file;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string assembly;
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Ayende.NHibernateQueryAnalyzer.Utilities", "0.0.0.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:nhibernate-configuration-2.2", IncludeInSchema=false)]
    public enum ItemsChoiceType {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("class-cache")]
        classcache,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("collection-cache")]
        collectioncache,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("jcs-class-cache")]
        jcsclasscache,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("jcs-collection-cache")]
        jcscollectioncache,
    }
}
