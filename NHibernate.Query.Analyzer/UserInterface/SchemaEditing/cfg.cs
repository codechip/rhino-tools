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
