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


namespace NHibernate.Mapping.Hbm {
    using Ayende.NHibernateQueryAnalyzer.SchemaEditing;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Ayende.NHibernateQueryAnalyzer.Utilities", "0.0.0.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class filter : object, System.ComponentModel.INotifyPropertyChanged {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [RequiredTag(MinimumAmount=1)]
        public string name;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string condition;
        
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
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:nhibernate-mapping-2.2")]
    [System.Xml.Serialization.XmlRootAttribute("sql-insert", Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class customSQL : object, System.ComponentModel.INotifyPropertyChanged {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public customSQLCheck check;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool checkSpecified;
        
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
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:nhibernate-mapping-2.2")]
    public enum customSQLCheck {
        
        /// <remarks/>
        none,
        
        /// <remarks/>
        rowcount,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Ayende.NHibernateQueryAnalyzer.Utilities", "0.0.0.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class loader : object, System.ComponentModel.INotifyPropertyChanged {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("query-ref")]
        [RequiredTag(MinimumAmount=1)]
        public string queryref;
        
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
    [System.Xml.Serialization.XmlRootAttribute(Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class resultset : object, System.ComponentModel.INotifyPropertyChanged {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("load-collection", typeof(loadcollection))]
        [System.Xml.Serialization.XmlElementAttribute("return", typeof(@return))]
        [System.Xml.Serialization.XmlElementAttribute("return-join", typeof(returnjoin))]
        [System.Xml.Serialization.XmlElementAttribute("return-scalar", typeof(returnscalar))]
        public object[] Items;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [RequiredTag(MinimumAmount=1)]
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
    [System.Xml.Serialization.XmlRootAttribute("load-collection", Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class loadcollection : object, System.ComponentModel.INotifyPropertyChanged {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("return-property")]
        public returnproperty[] Items;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [RequiredTag(MinimumAmount=1)]
        public string alias;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [RequiredTag(MinimumAmount=1)]
        public string role;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("lock-mode")]
        [System.ComponentModel.DefaultValueAttribute(lockMode.read)]
        public lockMode lockmode;
        
        public loadcollection() {
            this.lockmode = lockMode.read;
        }
        
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
    [System.Xml.Serialization.XmlRootAttribute("return-property", Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class returnproperty : object, System.ComponentModel.INotifyPropertyChanged {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("return-column")]
        public returncolumn[] returncolumn;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [RequiredTag(MinimumAmount=1)]
        public string name;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string column;
        
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
    [System.Xml.Serialization.XmlRootAttribute("return-column", Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class returncolumn : object, System.ComponentModel.INotifyPropertyChanged {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [RequiredTag(MinimumAmount=1)]
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
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:nhibernate-mapping-2.2")]
    public enum lockMode {
        
        /// <remarks/>
        none,
        
        /// <remarks/>
        read,
        
        /// <remarks/>
        upgrade,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("upgrade-nowait")]
        upgradenowait,
        
        /// <remarks/>
        write,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Ayende.NHibernateQueryAnalyzer.Utilities", "0.0.0.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class @return : object, System.ComponentModel.INotifyPropertyChanged {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("return-discriminator")]
        public returndiscriminator returndiscriminator;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("return-property")]
        public returnproperty[] returnproperty;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string alias;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string @class;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("lock-mode")]
        [System.ComponentModel.DefaultValueAttribute(lockMode.read)]
        public lockMode lockmode;
        
        public @return() {
            this.lockmode = lockMode.read;
        }
        
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
    [System.Xml.Serialization.XmlRootAttribute("return-discriminator", Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class returndiscriminator : object, System.ComponentModel.INotifyPropertyChanged {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [RequiredTag(MinimumAmount=1)]
        public string column;
        
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
    [System.Xml.Serialization.XmlRootAttribute("return-join", Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class returnjoin : object, System.ComponentModel.INotifyPropertyChanged {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("return-property")]
        public returnproperty[] Items;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [RequiredTag(MinimumAmount=1)]
        public string alias;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [RequiredTag(MinimumAmount=1)]
        public string property;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("lock-mode")]
        [System.ComponentModel.DefaultValueAttribute(lockMode.read)]
        public lockMode lockmode;
        
        public returnjoin() {
            this.lockmode = lockMode.read;
        }
        
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
    [System.Xml.Serialization.XmlRootAttribute("return-scalar", Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class returnscalar : object, System.ComponentModel.INotifyPropertyChanged {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [RequiredTag(MinimumAmount=1)]
        public string column;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [RequiredTag(MinimumAmount=1)]
        public string type;
        
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
    [System.Xml.Serialization.XmlRootAttribute("hibernate-mapping", Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class hibernatemapping : object, System.ComponentModel.INotifyPropertyChanged {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("meta")]
        public meta[] meta;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("import")]
        public import[] import;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("class", typeof(@class))]
        [System.Xml.Serialization.XmlElementAttribute("joined-subclass", typeof(joinedsubclass))]
        [System.Xml.Serialization.XmlElementAttribute("subclass", typeof(subclass))]
        public object[] Items;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("resultset")]
        public resultset[] resultset;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("query", typeof(query))]
        [System.Xml.Serialization.XmlElementAttribute("sql-query", typeof(sqlquery))]
        public object[] Items1;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("filter-def")]
        public filterdef[] filterdef;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("database-object")]
        public databaseobject[] databaseobject;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string schema;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("default-cascade")]
        [System.ComponentModel.DefaultValueAttribute(cascadeStyle.none)]
        public cascadeStyle defaultcascade;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("default-access")]
        [System.ComponentModel.DefaultValueAttribute("property")]
        public string defaultaccess;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("auto-import")]
        [System.ComponentModel.DefaultValueAttribute(true)]
        public bool autoimport;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string @namespace;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string assembly;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("default-lazy")]
        [System.ComponentModel.DefaultValueAttribute(true)]
        public bool defaultlazy;
        
        public hibernatemapping() {
            this.defaultcascade = cascadeStyle.none;
            this.defaultaccess = "property";
            this.autoimport = true;
            this.defaultlazy = true;
        }
        
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
    [System.Xml.Serialization.XmlRootAttribute(Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class meta : object, System.ComponentModel.INotifyPropertyChanged {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [RequiredTag(MinimumAmount=1)]
        public string attribute;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(true)]
        public bool inherit;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string[] Text;
        
        public meta() {
            this.inherit = true;
        }
        
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
    [System.Xml.Serialization.XmlRootAttribute(Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class import : object, System.ComponentModel.INotifyPropertyChanged {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [RequiredTag(MinimumAmount=1)]
        public string @class;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string rename;
        
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
    [System.Xml.Serialization.XmlRootAttribute(Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class @class : object, System.ComponentModel.INotifyPropertyChanged {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("meta")]
        public meta[] meta;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("cache", typeof(cacheType))]
        [System.Xml.Serialization.XmlElementAttribute("jcs-cache", typeof(cacheType))]
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
        [System.Xml.Serialization.XmlElementAttribute("any", typeof(any))]
        [System.Xml.Serialization.XmlElementAttribute("array", typeof(array))]
        [System.Xml.Serialization.XmlElementAttribute("bag", typeof(bag))]
        [System.Xml.Serialization.XmlElementAttribute("component", typeof(component))]
        [System.Xml.Serialization.XmlElementAttribute("dynamic-component", typeof(dynamiccomponent))]
        [System.Xml.Serialization.XmlElementAttribute("idbag", typeof(idbag))]
        [System.Xml.Serialization.XmlElementAttribute("list", typeof(list))]
        [System.Xml.Serialization.XmlElementAttribute("many-to-one", typeof(manytoone))]
        [System.Xml.Serialization.XmlElementAttribute("map", typeof(map))]
        [System.Xml.Serialization.XmlElementAttribute("one-to-one", typeof(onetoone))]
        [System.Xml.Serialization.XmlElementAttribute("primitive-array", typeof(primitivearray))]
        [System.Xml.Serialization.XmlElementAttribute("property", typeof(property))]
        [System.Xml.Serialization.XmlElementAttribute("set", typeof(set))]
        public object[] Items;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("joined-subclass", typeof(joinedsubclass))]
        [System.Xml.Serialization.XmlElementAttribute("subclass", typeof(subclass))]
        public object[] Items1;
        
        /// <remarks/>
        public loader loader;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("sql-insert")]
        public customSQL sqlinsert;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("sql-update")]
        public customSQL sqlupdate;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("sql-delete")]
        public customSQL sqldelete;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("filter")]
        public filter[] filter;
        
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
        public bool dynamicupdate;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("dynamic-insert")]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool dynamicinsert;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("select-before-update")]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool selectbeforeupdate;
        
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
        public bool mutable;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(polymorphismType.@implicit)]
        public polymorphismType polymorphism;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string persister;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string @where;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("batch-size", DataType="positiveInteger")]
        [System.ComponentModel.DefaultValueAttribute("1")]
        public string batchsize;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("optimistic-lock")]
        [System.ComponentModel.DefaultValueAttribute(optimisticLockMode.version)]
        public optimisticLockMode optimisticlock;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string check;
        
        public @class() {
            this.dynamicupdate = false;
            this.dynamicinsert = false;
            this.selectbeforeupdate = false;
            this.mutable = true;
            this.polymorphism = polymorphismType.@implicit;
            this.batchsize = "1";
            this.optimisticlock = optimisticLockMode.version;
        }
        
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
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:nhibernate-mapping-2.2")]
    [System.Xml.Serialization.XmlRootAttribute("cache", Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class cacheType : object, System.ComponentModel.INotifyPropertyChanged {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string region;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public cacheTypeUsage usage;
        
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
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
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
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Ayende.NHibernateQueryAnalyzer.Utilities", "0.0.0.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:nhibernate-mapping-2.2", IncludeInSchema=false)]
    public enum ItemChoiceType {
        
        /// <remarks/>
        cache,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("jcs-cache")]
        jcscache,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Ayende.NHibernateQueryAnalyzer.Utilities", "0.0.0.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
    [System.Xml.Serialization.XmlRootAttribute("composite-id", Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class compositeid : object, System.ComponentModel.INotifyPropertyChanged {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("key-many-to-one", typeof(keymanytoone))]
        [System.Xml.Serialization.XmlElementAttribute("key-property", typeof(keyproperty))]
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
        public unsavedValueType unsavedvalue;
        
        public compositeid() {
            this.unsavedvalue = unsavedValueType.none;
        }
        
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
    [System.Xml.Serialization.XmlRootAttribute("key-many-to-one", Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class keymanytoone : object, System.ComponentModel.INotifyPropertyChanged {
        
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
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public restrictedLaziness lazy;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool lazySpecified;
        
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
    [System.Xml.Serialization.XmlRootAttribute(Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class column : object, System.ComponentModel.INotifyPropertyChanged {
        
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
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string check;
        
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
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:nhibernate-mapping-2.2")]
    public enum restrictedLaziness {
        
        /// <remarks/>
        @false,
        
        /// <remarks/>
        proxy,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Ayende.NHibernateQueryAnalyzer.Utilities", "0.0.0.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
    [System.Xml.Serialization.XmlRootAttribute("key-property", Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class keyproperty : object, System.ComponentModel.INotifyPropertyChanged {
        
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
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:nhibernate-mapping-2.2")]
    public enum unsavedValueType {
        
        /// <remarks/>
        any,
        
        /// <remarks/>
        none,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Ayende.NHibernateQueryAnalyzer.Utilities", "0.0.0.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class id : object, System.ComponentModel.INotifyPropertyChanged {
        
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
        public string unsavedvalue;
        
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
    [System.Xml.Serialization.XmlRootAttribute(Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class generator : object, System.ComponentModel.INotifyPropertyChanged {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("param")]
        public param[] param;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [RequiredTag(MinimumAmount=1)]
        public string @class;
        
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
    [System.Xml.Serialization.XmlRootAttribute(Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class param : object, System.ComponentModel.INotifyPropertyChanged {
        
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
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class discriminator : object, System.ComponentModel.INotifyPropertyChanged {
        
        /// <remarks/>
        public column column;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("column")]
        public string column1;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute("String")]
        public string type;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("not-null")]
        [System.ComponentModel.DefaultValueAttribute(true)]
        public bool notnull;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType="positiveInteger")]
        public string length;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool force;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(true)]
        public bool insert;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string formula;
        
        public discriminator() {
            this.type = "String";
            this.notnull = true;
            this.force = false;
            this.insert = true;
        }
        
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
    [System.Xml.Serialization.XmlRootAttribute(Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class timestamp : object, System.ComponentModel.INotifyPropertyChanged {
        
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
        public string unsavedvalue;
        
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
    [System.Xml.Serialization.XmlRootAttribute(Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class version : object, System.ComponentModel.INotifyPropertyChanged {
        
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
        public string type;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("unsaved-value")]
        public string unsavedvalue;
        
        public version() {
            this.type = "Int32";
        }
        
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
    [System.Xml.Serialization.XmlRootAttribute(Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class any : object, System.ComponentModel.INotifyPropertyChanged {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("meta")]
        public meta[] meta;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("meta-value")]
        public metavalue[] metavalue;
        
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
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [RequiredTag(MinimumAmount=1)]
        public string name;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string access;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(true)]
        public bool insert;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(true)]
        public bool update;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(cascadeStyle.none)]
        public cascadeStyle cascade;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string index;
        
        public any() {
            this.insert = true;
            this.update = true;
            this.cascade = cascadeStyle.none;
        }
        
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
    [System.Xml.Serialization.XmlRootAttribute("meta-value", Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class metavalue : object, System.ComponentModel.INotifyPropertyChanged {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [RequiredTag(MinimumAmount=1)]
        public string value;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [RequiredTag(MinimumAmount=1)]
        public string @class;
        
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
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:nhibernate-mapping-2.2")]
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
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Ayende.NHibernateQueryAnalyzer.Utilities", "0.0.0.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class array : object, System.ComponentModel.INotifyPropertyChanged {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("meta")]
        public meta[] meta;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("cache", typeof(cacheType))]
        [System.Xml.Serialization.XmlElementAttribute("jcs-cache", typeof(cacheType))]
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
        [System.Xml.Serialization.XmlElementAttribute("composite-element", typeof(compositeelement))]
        [System.Xml.Serialization.XmlElementAttribute("element", typeof(element))]
        [System.Xml.Serialization.XmlElementAttribute("many-to-any", typeof(manytoany))]
        [System.Xml.Serialization.XmlElementAttribute("many-to-many", typeof(manytomany))]
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
        public string @where;
        
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
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:nhibernate-mapping-2.2", IncludeInSchema=false)]
    public enum ItemChoiceType5 {
        
        /// <remarks/>
        cache,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("jcs-cache")]
        jcscache,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Ayende.NHibernateQueryAnalyzer.Utilities", "0.0.0.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class key : object, System.ComponentModel.INotifyPropertyChanged {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("column")]
        public column[] column;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("column")]
        public string column1;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("foreign-key")]
        public string foreignkey;
        
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
    [System.Xml.Serialization.XmlRootAttribute(Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class index : object, System.ComponentModel.INotifyPropertyChanged {
        
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
    [System.Xml.Serialization.XmlRootAttribute("composite-element", Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class compositeelement : object, System.ComponentModel.INotifyPropertyChanged {
        
        /// <remarks/>
        public parent parent;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("many-to-one", typeof(manytoone))]
        [System.Xml.Serialization.XmlElementAttribute("nested-composite-element", typeof(nestedcompositeelement))]
        [System.Xml.Serialization.XmlElementAttribute("property", typeof(property))]
        public object[] Items;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [RequiredTag(MinimumAmount=1)]
        public string @class;
        
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
    [System.Xml.Serialization.XmlRootAttribute(Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class parent : object, System.ComponentModel.INotifyPropertyChanged {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [RequiredTag(MinimumAmount=1)]
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
    [System.Xml.Serialization.XmlRootAttribute("many-to-one", Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class manytoone : object, System.ComponentModel.INotifyPropertyChanged {
        
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
        public bool notnull;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool unique;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public cascadeStyle cascade;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool cascadeSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("outer-join")]
        public outerJoinStrategy outerjoin;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool outerjoinSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public fetchMode fetch;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool fetchSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(true)]
        public bool update;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(true)]
        public bool insert;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("foreign-key")]
        public string foreignkey;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("property-ref")]
        public string propertyref;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("not-found")]
        public notFoundMode notfound;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool notfoundSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public laziness lazy;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool lazySpecified;
        
        public manytoone() {
            this.notnull = false;
            this.unique = false;
            this.update = true;
            this.insert = true;
        }
        
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
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:nhibernate-mapping-2.2")]
    public enum outerJoinStrategy {
        
        /// <remarks/>
        auto,
        
        /// <remarks/>
        @true,
        
        /// <remarks/>
        @false,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Ayende.NHibernateQueryAnalyzer.Utilities", "0.0.0.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:nhibernate-mapping-2.2")]
    public enum fetchMode {
        
        /// <remarks/>
        select,
        
        /// <remarks/>
        join,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Ayende.NHibernateQueryAnalyzer.Utilities", "0.0.0.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:nhibernate-mapping-2.2")]
    public enum notFoundMode {
        
        /// <remarks/>
        ignore,
        
        /// <remarks/>
        exception,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Ayende.NHibernateQueryAnalyzer.Utilities", "0.0.0.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:nhibernate-mapping-2.2")]
    public enum laziness {
        
        /// <remarks/>
        @false,
        
        /// <remarks/>
        proxy,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Ayende.NHibernateQueryAnalyzer.Utilities", "0.0.0.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
    [System.Xml.Serialization.XmlRootAttribute("nested-composite-element", Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class nestedcompositeelement : object, System.ComponentModel.INotifyPropertyChanged {
        
        /// <remarks/>
        public parent parent;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("many-to-one", typeof(manytoone))]
        [System.Xml.Serialization.XmlElementAttribute("nested-composite-element", typeof(nestedcompositeelement))]
        [System.Xml.Serialization.XmlElementAttribute("property", typeof(property))]
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
    [System.Xml.Serialization.XmlRootAttribute(Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class property : object, System.ComponentModel.INotifyPropertyChanged {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("meta")]
        public meta[] meta;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("column")]
        public column[] column;
        
        /// <remarks/>
        public type type;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [RequiredTag(MinimumAmount=1)]
        public string name;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string access;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("type")]
        public string type1;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("column")]
        public string column1;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType="positiveInteger")]
        public string length;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("not-null")]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool notnull;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool unique;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(true)]
        public bool update;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(true)]
        public bool insert;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("optimistic-lock")]
        [System.ComponentModel.DefaultValueAttribute(true)]
        public bool optimisticlock;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string formula;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string index;
        
        public property() {
            this.notnull = false;
            this.unique = false;
            this.update = true;
            this.insert = true;
            this.optimisticlock = true;
        }
        
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
    [System.Xml.Serialization.XmlRootAttribute(Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class type : object, System.ComponentModel.INotifyPropertyChanged {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("param")]
        public param[] param;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [RequiredTag(MinimumAmount=1)]
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
    [System.Xml.Serialization.XmlRootAttribute(Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class element : object, System.ComponentModel.INotifyPropertyChanged {
        
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
        public bool notnull;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool unique;
        
        public element() {
            this.notnull = false;
            this.unique = false;
        }
        
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
    [System.Xml.Serialization.XmlRootAttribute("many-to-any", Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class manytoany : object, System.ComponentModel.INotifyPropertyChanged {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("meta-value")]
        public metavalue[] metavalue;
        
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
    [System.Xml.Serialization.XmlRootAttribute("many-to-many", Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class manytomany : object, System.ComponentModel.INotifyPropertyChanged {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("meta")]
        public meta[] meta;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("column")]
        public column[] column;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("filter")]
        public filter[] filter;
        
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
        public outerJoinStrategy outerjoin;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool outerjoinSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public fetchMode fetch;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool fetchSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("not-found")]
        public notFoundMode notfound;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool notfoundSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string @where;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public restrictedLaziness lazy;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool lazySpecified;
        
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
    [System.Xml.Serialization.XmlRootAttribute("one-to-many", Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class onetomany : object, System.ComponentModel.INotifyPropertyChanged {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [RequiredTag(MinimumAmount=1)]
        public string @class;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("not-found")]
        public notFoundMode notfound;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool notfoundSpecified;
        
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
    [System.Xml.Serialization.XmlRootAttribute(Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class bag : object, System.ComponentModel.INotifyPropertyChanged {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("meta")]
        public meta[] meta;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("cache", typeof(cacheType))]
        [System.Xml.Serialization.XmlElementAttribute("jcs-cache", typeof(cacheType))]
        [System.Xml.Serialization.XmlChoiceIdentifierAttribute("ItemElementName")]
        public cacheType Item;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public ItemChoiceType4 ItemElementName;
        
        /// <remarks/>
        [RequiredTag(MinimumAmount=1)]
        public key key;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("composite-element", typeof(compositeelement))]
        [System.Xml.Serialization.XmlElementAttribute("element", typeof(element))]
        [System.Xml.Serialization.XmlElementAttribute("many-to-any", typeof(manytoany))]
        [System.Xml.Serialization.XmlElementAttribute("many-to-many", typeof(manytomany))]
        [System.Xml.Serialization.XmlElementAttribute("one-to-many", typeof(onetomany))]
        [RequiredTag(MinimumAmount=1)]
        public object Item1;
        
        /// <remarks/>
        public loader loader;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("sql-insert")]
        public customSQL sqlinsert;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("sql-update")]
        public customSQL sqlupdate;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("sql-delete")]
        public customSQL sqldelete;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("sql-delete-all")]
        public customSQL sqldeleteall;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("filter")]
        public filter[] filter;
        
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
        public bool lazy;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool lazySpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("outer-join")]
        public outerJoinStrategy outerjoin;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool outerjoinSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public collectionFetchMode fetch;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool fetchSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public cascadeStyle cascade;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool cascadeSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string @where;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool inverse;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string persister;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("batch-size", DataType="positiveInteger")]
        [System.ComponentModel.DefaultValueAttribute("1")]
        public string batchsize;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string check;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("collection-type")]
        public string collectiontype;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool generic;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool genericSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("order-by")]
        public string orderby;
        
        public bag() {
            this.inverse = false;
            this.batchsize = "1";
        }
        
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
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:nhibernate-mapping-2.2", IncludeInSchema=false)]
    public enum ItemChoiceType4 {
        
        /// <remarks/>
        cache,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("jcs-cache")]
        jcscache,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Ayende.NHibernateQueryAnalyzer.Utilities", "0.0.0.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:nhibernate-mapping-2.2")]
    public enum collectionFetchMode {
        
        /// <remarks/>
        select,
        
        /// <remarks/>
        join,
        
        /// <remarks/>
        subselect,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Ayende.NHibernateQueryAnalyzer.Utilities", "0.0.0.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class component : object, System.ComponentModel.INotifyPropertyChanged {
        
        /// <remarks/>
        public parent parent;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("any", typeof(any))]
        [System.Xml.Serialization.XmlElementAttribute("array", typeof(array))]
        [System.Xml.Serialization.XmlElementAttribute("bag", typeof(bag))]
        [System.Xml.Serialization.XmlElementAttribute("component", typeof(component))]
        [System.Xml.Serialization.XmlElementAttribute("dynamic-component", typeof(dynamiccomponent))]
        [System.Xml.Serialization.XmlElementAttribute("list", typeof(list))]
        [System.Xml.Serialization.XmlElementAttribute("many-to-one", typeof(manytoone))]
        [System.Xml.Serialization.XmlElementAttribute("map", typeof(map))]
        [System.Xml.Serialization.XmlElementAttribute("one-to-one", typeof(onetoone))]
        [System.Xml.Serialization.XmlElementAttribute("primitive-array", typeof(primitivearray))]
        [System.Xml.Serialization.XmlElementAttribute("property", typeof(property))]
        [System.Xml.Serialization.XmlElementAttribute("set", typeof(set))]
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
        public bool update;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(true)]
        public bool insert;
        
        public component() {
            this.update = true;
            this.insert = true;
        }
        
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
    [System.Xml.Serialization.XmlRootAttribute("dynamic-component", Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class dynamiccomponent : object, System.ComponentModel.INotifyPropertyChanged {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("any", typeof(any))]
        [System.Xml.Serialization.XmlElementAttribute("array", typeof(array))]
        [System.Xml.Serialization.XmlElementAttribute("bag", typeof(bag))]
        [System.Xml.Serialization.XmlElementAttribute("component", typeof(component))]
        [System.Xml.Serialization.XmlElementAttribute("dynamic-component", typeof(dynamiccomponent))]
        [System.Xml.Serialization.XmlElementAttribute("list", typeof(list))]
        [System.Xml.Serialization.XmlElementAttribute("many-to-one", typeof(manytoone))]
        [System.Xml.Serialization.XmlElementAttribute("map", typeof(map))]
        [System.Xml.Serialization.XmlElementAttribute("one-to-one", typeof(onetoone))]
        [System.Xml.Serialization.XmlElementAttribute("primitive-array", typeof(primitivearray))]
        [System.Xml.Serialization.XmlElementAttribute("property", typeof(property))]
        [System.Xml.Serialization.XmlElementAttribute("set", typeof(set))]
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
        [System.ComponentModel.DefaultValueAttribute(true)]
        public bool update;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(true)]
        public bool insert;
        
        public dynamiccomponent() {
            this.update = true;
            this.insert = true;
        }
        
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
    [System.Xml.Serialization.XmlRootAttribute(Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class list : object, System.ComponentModel.INotifyPropertyChanged {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("meta")]
        public meta[] meta;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("cache", typeof(cacheType))]
        [System.Xml.Serialization.XmlElementAttribute("jcs-cache", typeof(cacheType))]
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
        [System.Xml.Serialization.XmlElementAttribute("composite-element", typeof(compositeelement))]
        [System.Xml.Serialization.XmlElementAttribute("element", typeof(element))]
        [System.Xml.Serialization.XmlElementAttribute("many-to-any", typeof(manytoany))]
        [System.Xml.Serialization.XmlElementAttribute("many-to-many", typeof(manytomany))]
        [System.Xml.Serialization.XmlElementAttribute("one-to-many", typeof(onetomany))]
        [RequiredTag(MinimumAmount=1)]
        public object Item1;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("filter")]
        public filter[] filter;
        
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
        public bool lazy;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool lazySpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("outer-join")]
        public outerJoinStrategy outerjoin;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool outerjoinSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public collectionFetchMode fetch;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool fetchSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public cascadeStyle cascade;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool cascadeSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string @where;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool inverse;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string persister;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("batch-size", DataType="positiveInteger")]
        [System.ComponentModel.DefaultValueAttribute("1")]
        public string batchsize;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string check;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("collection-type")]
        public string collectiontype;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool generic;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool genericSpecified;
        
        public list() {
            this.inverse = false;
            this.batchsize = "1";
        }
        
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
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:nhibernate-mapping-2.2", IncludeInSchema=false)]
    public enum ItemChoiceType3 {
        
        /// <remarks/>
        cache,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("jcs-cache")]
        jcscache,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Ayende.NHibernateQueryAnalyzer.Utilities", "0.0.0.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class map : object, System.ComponentModel.INotifyPropertyChanged {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("meta")]
        public meta[] meta;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("cache", typeof(cacheType))]
        [System.Xml.Serialization.XmlElementAttribute("jcs-cache", typeof(cacheType))]
        [System.Xml.Serialization.XmlChoiceIdentifierAttribute("ItemElementName")]
        public cacheType Item;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public ItemChoiceType1 ItemElementName;
        
        /// <remarks/>
        [RequiredTag(MinimumAmount=1)]
        public key key;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("composite-index", typeof(compositeindex))]
        [System.Xml.Serialization.XmlElementAttribute("index", typeof(index))]
        [System.Xml.Serialization.XmlElementAttribute("index-many-to-any", typeof(indexmanytoany))]
        [System.Xml.Serialization.XmlElementAttribute("index-many-to-many", typeof(indexmanytomany))]
        [RequiredTag(MinimumAmount=1)]
        public object Item1;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("composite-element", typeof(compositeelement))]
        [System.Xml.Serialization.XmlElementAttribute("element", typeof(element))]
        [System.Xml.Serialization.XmlElementAttribute("many-to-any", typeof(manytoany))]
        [System.Xml.Serialization.XmlElementAttribute("many-to-many", typeof(manytomany))]
        [System.Xml.Serialization.XmlElementAttribute("one-to-many", typeof(onetomany))]
        [RequiredTag(MinimumAmount=1)]
        public object Item2;
        
        /// <remarks/>
        public loader loader;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("sql-insert")]
        public customSQL sqlinsert;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("sql-update")]
        public customSQL sqlupdate;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("sql-delete")]
        public customSQL sqldelete;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("sql-delete-all")]
        public customSQL sqldeleteall;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("filter")]
        public filter[] filter;
        
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
        public bool lazy;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool lazySpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("outer-join")]
        public outerJoinStrategy outerjoin;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool outerjoinSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public collectionFetchMode fetch;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool fetchSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public cascadeStyle cascade;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool cascadeSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string @where;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool inverse;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string persister;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("batch-size", DataType="positiveInteger")]
        [System.ComponentModel.DefaultValueAttribute("1")]
        public string batchsize;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string check;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("collection-type")]
        public string collectiontype;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool generic;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool genericSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("order-by")]
        public string orderby;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute("unsorted")]
        public string sort;
        
        public map() {
            this.inverse = false;
            this.batchsize = "1";
            this.sort = "unsorted";
        }
        
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
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:nhibernate-mapping-2.2", IncludeInSchema=false)]
    public enum ItemChoiceType1 {
        
        /// <remarks/>
        cache,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("jcs-cache")]
        jcscache,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Ayende.NHibernateQueryAnalyzer.Utilities", "0.0.0.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
    [System.Xml.Serialization.XmlRootAttribute("composite-index", Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class compositeindex : object, System.ComponentModel.INotifyPropertyChanged {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("key-many-to-one", typeof(keymanytoone))]
        [System.Xml.Serialization.XmlElementAttribute("key-property", typeof(keyproperty))]
        [RequiredTag(MinimumAmount=1)]
        public object[] Items;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [RequiredTag(MinimumAmount=1)]
        public string @class;
        
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
    [System.Xml.Serialization.XmlRootAttribute("index-many-to-any", Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class indexmanytoany : object, System.ComponentModel.INotifyPropertyChanged {
        
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
    [System.Xml.Serialization.XmlRootAttribute("index-many-to-many", Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class indexmanytomany : object, System.ComponentModel.INotifyPropertyChanged {
        
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
    [System.Xml.Serialization.XmlRootAttribute("one-to-one", Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class onetoone : object, System.ComponentModel.INotifyPropertyChanged {
        
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
        public outerJoinStrategy outerjoin;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool outerjoinSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public fetchMode fetch;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool fetchSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool constrained;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("foreign-key")]
        public string foreignkey;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("property-ref")]
        public string propertyref;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public laziness lazy;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool lazySpecified;
        
        public onetoone() {
            this.constrained = false;
        }
        
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
    [System.Xml.Serialization.XmlRootAttribute("primitive-array", Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class primitivearray : object, System.ComponentModel.INotifyPropertyChanged {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("meta")]
        public meta[] meta;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("cache", typeof(cacheType))]
        [System.Xml.Serialization.XmlElementAttribute("jcs-cache", typeof(cacheType))]
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
        public string @where;
        
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
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:nhibernate-mapping-2.2", IncludeInSchema=false)]
    public enum ItemChoiceType6 {
        
        /// <remarks/>
        cache,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("jcs-cache")]
        jcscache,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Ayende.NHibernateQueryAnalyzer.Utilities", "0.0.0.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class set : object, System.ComponentModel.INotifyPropertyChanged {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("meta")]
        public meta[] meta;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("cache", typeof(cacheType))]
        [System.Xml.Serialization.XmlElementAttribute("jcs-cache", typeof(cacheType))]
        [System.Xml.Serialization.XmlChoiceIdentifierAttribute("ItemElementName")]
        public cacheType Item;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public ItemChoiceType2 ItemElementName;
        
        /// <remarks/>
        [RequiredTag(MinimumAmount=1)]
        public key key;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("composite-element", typeof(compositeelement))]
        [System.Xml.Serialization.XmlElementAttribute("element", typeof(element))]
        [System.Xml.Serialization.XmlElementAttribute("many-to-any", typeof(manytoany))]
        [System.Xml.Serialization.XmlElementAttribute("many-to-many", typeof(manytomany))]
        [System.Xml.Serialization.XmlElementAttribute("one-to-many", typeof(onetomany))]
        [RequiredTag(MinimumAmount=1)]
        public object Item1;
        
        /// <remarks/>
        public loader loader;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("sql-insert")]
        public customSQL sqlinsert;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("sql-update")]
        public customSQL sqlupdate;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("sql-delete")]
        public customSQL sqldelete;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("sql-delete-all")]
        public customSQL sqldeleteall;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("filter")]
        public filter[] filter;
        
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
        public bool lazy;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool lazySpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("outer-join")]
        public outerJoinStrategy outerjoin;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool outerjoinSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public collectionFetchMode fetch;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool fetchSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public cascadeStyle cascade;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool cascadeSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string @where;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool inverse;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string persister;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("batch-size", DataType="positiveInteger")]
        [System.ComponentModel.DefaultValueAttribute("1")]
        public string batchsize;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string check;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("collection-type")]
        public string collectiontype;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool generic;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool genericSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("order-by")]
        public string orderby;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute("unsorted")]
        public string sort;
        
        public set() {
            this.inverse = false;
            this.batchsize = "1";
            this.sort = "unsorted";
        }
        
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
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:nhibernate-mapping-2.2", IncludeInSchema=false)]
    public enum ItemChoiceType2 {
        
        /// <remarks/>
        cache,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("jcs-cache")]
        jcscache,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Ayende.NHibernateQueryAnalyzer.Utilities", "0.0.0.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class idbag : object, System.ComponentModel.INotifyPropertyChanged {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("meta")]
        public meta[] meta;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("cache", typeof(cacheType))]
        [System.Xml.Serialization.XmlElementAttribute("jcs-cache", typeof(cacheType))]
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
        [System.Xml.Serialization.XmlElementAttribute("composite-element", typeof(compositeelement))]
        [System.Xml.Serialization.XmlElementAttribute("element", typeof(element))]
        [System.Xml.Serialization.XmlElementAttribute("many-to-any", typeof(manytoany))]
        [System.Xml.Serialization.XmlElementAttribute("many-to-many", typeof(manytomany))]
        [RequiredTag(MinimumAmount=1)]
        public object Item1;
        
        /// <remarks/>
        public loader loader;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("sql-insert")]
        public customSQL sqlinsert;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("sql-update")]
        public customSQL sqlupdate;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("sql-delete")]
        public customSQL sqldelete;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("sql-delete-all")]
        public customSQL sqldeleteall;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("filter")]
        public filter[] filter;
        
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
        public bool lazy;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool lazySpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("outer-join")]
        public outerJoinStrategy outerjoin;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool outerjoinSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public collectionFetchMode fetch;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool fetchSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public cascadeStyle cascade;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool cascadeSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string @where;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool inverse;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string persister;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("batch-size", DataType="positiveInteger")]
        [System.ComponentModel.DefaultValueAttribute("1")]
        public string batchsize;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string check;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("collection-type")]
        public string collectiontype;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool generic;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool genericSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("order-by")]
        public string orderby;
        
        public idbag() {
            this.inverse = false;
            this.batchsize = "1";
        }
        
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
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:nhibernate-mapping-2.2", IncludeInSchema=false)]
    public enum ItemChoiceType7 {
        
        /// <remarks/>
        cache,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("jcs-cache")]
        jcscache,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Ayende.NHibernateQueryAnalyzer.Utilities", "0.0.0.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
    [System.Xml.Serialization.XmlRootAttribute("collection-id", Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class collectionid : object, System.ComponentModel.INotifyPropertyChanged {
        
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
    [System.Xml.Serialization.XmlRootAttribute("joined-subclass", Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class joinedsubclass : object, System.ComponentModel.INotifyPropertyChanged {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("meta")]
        public meta[] meta;
        
        /// <remarks/>
        [RequiredTag(MinimumAmount=1)]
        public key key;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("any", typeof(any))]
        [System.Xml.Serialization.XmlElementAttribute("array", typeof(array))]
        [System.Xml.Serialization.XmlElementAttribute("bag", typeof(bag))]
        [System.Xml.Serialization.XmlElementAttribute("component", typeof(component))]
        [System.Xml.Serialization.XmlElementAttribute("dynamic-component", typeof(dynamiccomponent))]
        [System.Xml.Serialization.XmlElementAttribute("idbag", typeof(idbag))]
        [System.Xml.Serialization.XmlElementAttribute("list", typeof(list))]
        [System.Xml.Serialization.XmlElementAttribute("many-to-one", typeof(manytoone))]
        [System.Xml.Serialization.XmlElementAttribute("map", typeof(map))]
        [System.Xml.Serialization.XmlElementAttribute("one-to-one", typeof(onetoone))]
        [System.Xml.Serialization.XmlElementAttribute("primitive-array", typeof(primitivearray))]
        [System.Xml.Serialization.XmlElementAttribute("property", typeof(property))]
        [System.Xml.Serialization.XmlElementAttribute("set", typeof(set))]
        public object[] Items;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("joined-subclass")]
        public joinedsubclass[] joinedsubclass1;
        
        /// <remarks/>
        public loader loader;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("sql-insert")]
        public customSQL sqlinsert;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("sql-update")]
        public customSQL sqlupdate;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("sql-delete")]
        public customSQL sqldelete;
        
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
        public bool dynamicupdate;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("dynamic-insert")]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool dynamicinsert;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("select-before-update")]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool selectbeforeupdate;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string extends;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string schema;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string table;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string check;
        
        public joinedsubclass() {
            this.dynamicupdate = false;
            this.dynamicinsert = false;
            this.selectbeforeupdate = false;
        }
        
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
    [System.Xml.Serialization.XmlRootAttribute(Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class subclass : object, System.ComponentModel.INotifyPropertyChanged {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("meta")]
        public meta[] meta;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("any", typeof(any))]
        [System.Xml.Serialization.XmlElementAttribute("array", typeof(array))]
        [System.Xml.Serialization.XmlElementAttribute("bag", typeof(bag))]
        [System.Xml.Serialization.XmlElementAttribute("component", typeof(component))]
        [System.Xml.Serialization.XmlElementAttribute("dynamic-component", typeof(dynamiccomponent))]
        [System.Xml.Serialization.XmlElementAttribute("idbag", typeof(idbag))]
        [System.Xml.Serialization.XmlElementAttribute("list", typeof(list))]
        [System.Xml.Serialization.XmlElementAttribute("many-to-one", typeof(manytoone))]
        [System.Xml.Serialization.XmlElementAttribute("map", typeof(map))]
        [System.Xml.Serialization.XmlElementAttribute("one-to-one", typeof(onetoone))]
        [System.Xml.Serialization.XmlElementAttribute("primitive-array", typeof(primitivearray))]
        [System.Xml.Serialization.XmlElementAttribute("property", typeof(property))]
        [System.Xml.Serialization.XmlElementAttribute("set", typeof(set))]
        public object[] Items;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("subclass")]
        public subclass[] subclass1;
        
        /// <remarks/>
        public loader loader;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("sql-insert")]
        public customSQL sqlinsert;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("sql-update")]
        public customSQL sqlupdate;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("sql-delete")]
        public customSQL sqldelete;
        
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
        public bool dynamicupdate;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("dynamic-insert")]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool dynamicinsert;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("select-before-update")]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool selectbeforeupdate;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string extends;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("discriminator-value")]
        public string discriminatorvalue;
        
        public subclass() {
            this.dynamicupdate = false;
            this.dynamicinsert = false;
            this.selectbeforeupdate = false;
        }
        
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
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:nhibernate-mapping-2.2")]
    public enum polymorphismType {
        
        /// <remarks/>
        @implicit,
        
        /// <remarks/>
        @explicit,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Ayende.NHibernateQueryAnalyzer.Utilities", "0.0.0.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:nhibernate-mapping-2.2")]
    public enum optimisticLockMode {
        
        /// <remarks/>
        none,
        
        /// <remarks/>
        version,
        
        /// <remarks/>
        dirty,
        
        /// <remarks/>
        all,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Ayende.NHibernateQueryAnalyzer.Utilities", "0.0.0.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class query : object, System.ComponentModel.INotifyPropertyChanged {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [RequiredTag(MinimumAmount=1)]
        public string name;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("flush-mode")]
        public flushMode flushmode;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool flushmodeSpecified;
        
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
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:nhibernate-mapping-2.2")]
    public enum flushMode {
        
        /// <remarks/>
        auto,
        
        /// <remarks/>
        never,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Ayende.NHibernateQueryAnalyzer.Utilities", "0.0.0.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
    [System.Xml.Serialization.XmlRootAttribute("sql-query", Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class sqlquery : object, System.ComponentModel.INotifyPropertyChanged {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("load-collection", typeof(loadcollection))]
        [System.Xml.Serialization.XmlElementAttribute("return", typeof(@return))]
        [System.Xml.Serialization.XmlElementAttribute("return-join", typeof(returnjoin))]
        [System.Xml.Serialization.XmlElementAttribute("return-scalar", typeof(returnscalar))]
        [System.Xml.Serialization.XmlElementAttribute("synchronize", typeof(synchronize))]
        public object[] Items;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string[] Text;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [RequiredTag(MinimumAmount=1)]
        public string name;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("resultset-ref")]
        public string resultsetref;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("flush-mode")]
        public flushMode flushmode;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool flushmodeSpecified;
        
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
    [System.Xml.Serialization.XmlRootAttribute(Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class synchronize : object, System.ComponentModel.INotifyPropertyChanged {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [RequiredTag(MinimumAmount=1)]
        public string table;
        
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
    [System.Xml.Serialization.XmlRootAttribute("filter-def", Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class filterdef : object, System.ComponentModel.INotifyPropertyChanged {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("filter-param")]
        [RequiredTag(MinimumAmount=1)]
        public filterparam[] filterparam;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string[] Text;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [RequiredTag(MinimumAmount=1)]
        public string name;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute("")]
        public string condition;
        
        public filterdef() {
            this.condition = "";
        }
        
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
    [System.Xml.Serialization.XmlRootAttribute("filter-param", Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class filterparam : object, System.ComponentModel.INotifyPropertyChanged {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [RequiredTag(MinimumAmount=1)]
        public string name;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [RequiredTag(MinimumAmount=1)]
        public string type;
        
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
    [System.Xml.Serialization.XmlRootAttribute("database-object", Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class databaseobject : object, System.ComponentModel.INotifyPropertyChanged {
        
        /// <remarks/>
        [RequiredTag(MinimumAmount=1)]
        public create create;
        
        /// <remarks/>
        [RequiredTag(MinimumAmount=1)]
        public drop drop;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("dialect-scope")]
        public dialectscope[] dialectscope;
        
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
    [System.Xml.Serialization.XmlRootAttribute(Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class create : object, System.ComponentModel.INotifyPropertyChanged {
        
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
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class drop : object, System.ComponentModel.INotifyPropertyChanged {
        
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
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
    [System.Xml.Serialization.XmlRootAttribute("dialect-scope", Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class dialectscope : object, System.ComponentModel.INotifyPropertyChanged {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [RequiredTag(MinimumAmount=1)]
        public string name;
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
