//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.312
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Query {
    
    
    public partial class For {
        
        /// Query Builder for member WeirdClass
        public static Query.QueryBuilder<NHibernate.Query.Generator.Tests.ActiveRecord.WeirdClass> WeirdClass {
            get {
                return new Query.QueryBuilder<NHibernate.Query.Generator.Tests.ActiveRecord.WeirdClass>();
            }
        }
    }
    
    public partial class Where {
        
        /// Where for member _root_Where_WeirdClass
        static Root_Where_WeirdClass _root_Where_WeirdClass = new Root_Where_WeirdClass();
        
        /// Where for member WeirdClass
        public static Root_Where_WeirdClass WeirdClass {
            get {
                return _root_Where_WeirdClass;
            }
        }
        
        /// Where for member Where_WeirdClass
        public partial class Where_WeirdClass<T1> : Query.WhereClause<T1>
         {
            
            /// Where for member .ctor
            public Where_WeirdClass(string QpName, string QpAssociationPath) : 
                    base(QpName, QpAssociationPath) {
            }
            
            /// Where for member .ctor
            public Where_WeirdClass(string QpName, string QpAssociationPath, bool QpBackTrack) : 
                    base(QpName, QpAssociationPath, QpBackTrack) {
            }
            
            /// Where for member Address
            public virtual Where_Address<T1> Address {
                get {
                    return new Where_Address<T1>("Address", null);
                }
            }
            
            /// Where for member Key
            public virtual Where_Key<T1> Key {
                get {
                    return new Where_Key<T1>("Key", QpAssociationPath);
                }
            }
            
            /// Where for member Where_Address
            public partial class Where_Address<T2> : Where_WeirdClass<T2>
             {
                
                /// Where for member .ctor
                public Where_Address(string QpName, string QpAssociationPath) : 
                        base(QpName, QpAssociationPath) {
                }
                
                /// Where for member .ctor
                public Where_Address(string QpName, string QpAssociationPath, bool QpBackTrack) : 
                        base(QpName, QpAssociationPath, QpBackTrack) {
                }
                
                /// Where for member 
                public virtual Query.WhereClauseProperty<T1> Street {
                    get {
                        string temp = QpAssociationPath;
                        return new Query.WhereClauseProperty<T1>("Address.Street", temp);
                    }
                }
            }
            
            /// Where for member Where_Key
            public partial class Where_Key<T3> : Where_WeirdClass<T3>
             {
                
                /// Where for member .ctor
                public Where_Key(string QpName, string QpAssociationPath) : 
                        base(QpName, QpAssociationPath) {
                }
                
                /// Where for member .ctor
                public Where_Key(string QpName, string QpAssociationPath, bool QpBackTrack) : 
                        base(QpName, QpAssociationPath, QpBackTrack) {
                }
                
                /// Where for member 
                public virtual Query.WhereClauseProperty<T1> Department {
                    get {
                        string temp = QpAssociationPath;
                        return new Query.WhereClauseProperty<T1>("Key.Department", temp);
                    }
                }
                
                /// Where for member 
                public virtual Query.WhereClauseProperty<T1> Level {
                    get {
                        string temp = QpAssociationPath;
                        return new Query.WhereClauseProperty<T1>("Key.Level", temp);
                    }
                }
            }
        }
        
        /// Where for member Root_Where_WeirdClass
        public partial class Root_Where_WeirdClass : Where_WeirdClass<NHibernate.Query.Generator.Tests.ActiveRecord.WeirdClass> {
            
            /// Where for member .ctor
            public Root_Where_WeirdClass() : 
                    base("this", null) {
            }
        }
    }
    
    public partial class ProjectBy {
        
        /// Projection for member _root_Projection_WeirdClass
        static Root_Projection_WeirdClass _root_Projection_WeirdClass = new Root_Projection_WeirdClass();
        
        /// Projection for member WeirdClass
        public static Root_Projection_WeirdClass WeirdClass {
            get {
                return _root_Projection_WeirdClass;
            }
        }
        
        /// Projection for member Projection_WeirdClass
        public partial class Projection_WeirdClass<T4> : Query.ProjectionEntity<T4>
         {
            
            /// Projection for member .ctor
            public Projection_WeirdClass(string QpName, string QpAssociationPath) : 
                    base(QpName, QpAssociationPath) {
            }
            
            /// Projection for member .ctor
            public Projection_WeirdClass(string QpName, string QpAssociationPath, bool QpBackTrack) : 
                    base(QpName, QpAssociationPath, QpBackTrack) {
            }
            
            /// Projection for member Address
            public virtual Projection_Address<T4> Address {
                get {
                    return new Projection_Address<T4>("Address", null);
                }
            }
            
            /// Projection for member Key
            public virtual Projection_Key<T4> Key {
                get {
                    return new Projection_Key<T4>("Key", QpAssociationPath);
                }
            }
            
            /// Projection for member Projection_Address
            public partial class Projection_Address<T5> : Projection_WeirdClass<T5>
             {
                
                /// Projection for member .ctor
                public Projection_Address(string QpName, string QpAssociationPath) : 
                        base(QpName, QpAssociationPath) {
                }
                
                /// Projection for member .ctor
                public Projection_Address(string QpName, string QpAssociationPath, bool QpBackTrack) : 
                        base(QpName, QpAssociationPath, QpBackTrack) {
                }
                
                /// Projection for member 
                public virtual Query.ProjectionClauseProperty<T4> Street {
                    get {
                        string temp = QpAssociationPath;
                        return new Query.ProjectionClauseProperty<T4>("Address.Street", temp);
                    }
                }
            }
            
            /// Projection for member Projection_Key
            public partial class Projection_Key<T6> : Projection_WeirdClass<T6>
             {
                
                /// Projection for member .ctor
                public Projection_Key(string QpName, string QpAssociationPath) : 
                        base(QpName, QpAssociationPath) {
                }
                
                /// Projection for member .ctor
                public Projection_Key(string QpName, string QpAssociationPath, bool QpBackTrack) : 
                        base(QpName, QpAssociationPath, QpBackTrack) {
                }
                
                /// Projection for member 
                public virtual Query.ProjectionClauseProperty<T4> Department {
                    get {
                        string temp = QpAssociationPath;
                        return new Query.ProjectionClauseProperty<T4>("Key.Department", temp);
                    }
                }
                
                /// Projection for member 
                public virtual Query.ProjectionClausePropertyNumeric<T4> Level {
                    get {
                        string temp = QpAssociationPath;
                        return new Query.ProjectionClausePropertyNumeric<T4>("Key.Level", temp);
                    }
                }
            }
        }
        
        /// Projection for member Root_Projection_WeirdClass
        public partial class Root_Projection_WeirdClass : Projection_WeirdClass<NHibernate.Query.Generator.Tests.ActiveRecord.WeirdClass> {
            
            /// Projection for member .ctor
            public Root_Projection_WeirdClass() : 
                    base("this", null) {
            }
        }
    }
    
    public partial class OrderBy {
        
        /// OrderBy for member _root_OrderBy_WeirdClass
        static Root_OrderBy_WeirdClass _root_OrderBy_WeirdClass = new Root_OrderBy_WeirdClass();
        
        /// OrderBy for member WeirdClass
        public static Root_OrderBy_WeirdClass WeirdClass {
            get {
                return _root_OrderBy_WeirdClass;
            }
        }
        
        /// OrderBy for member OrderBy_WeirdClass
        public partial class OrderBy_WeirdClass<T7> : Query.QueryPart
         {
            
            /// OrderBy for member .ctor
            public OrderBy_WeirdClass(string QpName, string QpAssociationPath) : 
                    base(QpName, QpAssociationPath) {
            }
            
            /// OrderBy for member .ctor
            public OrderBy_WeirdClass(string QpName, string QpAssociationPath, bool QpBackTrack) : 
                    base(QpName, QpAssociationPath, QpBackTrack) {
            }
            
            /// OrderBy for member Address
            public virtual OrderBy_Address<T7> Address {
                get {
                    return new OrderBy_Address<T7>("Address", null);
                }
            }
            
            /// OrderBy for member Key
            public virtual OrderBy_Key<T7> Key {
                get {
                    return new OrderBy_Key<T7>("Key", QpAssociationPath);
                }
            }
            
            /// OrderBy for member OrderBy_Address
            public partial class OrderBy_Address<T8> : OrderBy_WeirdClass<T8>
             {
                
                /// OrderBy for member .ctor
                public OrderBy_Address(string QpName, string QpAssociationPath) : 
                        base(QpName, QpAssociationPath) {
                }
                
                /// OrderBy for member .ctor
                public OrderBy_Address(string QpName, string QpAssociationPath, bool QpBackTrack) : 
                        base(QpName, QpAssociationPath, QpBackTrack) {
                }
                
                /// OrderBy for member 
                public virtual Query.OrderByClauseProperty<T7> Street {
                    get {
                        string temp = QpAssociationPath;
                        return new Query.OrderByClauseProperty<T7>("Address.Street", temp);
                    }
                }
            }
            
            /// OrderBy for member OrderBy_Key
            public partial class OrderBy_Key<T9> : OrderBy_WeirdClass<T9>
             {
                
                /// OrderBy for member .ctor
                public OrderBy_Key(string QpName, string QpAssociationPath) : 
                        base(QpName, QpAssociationPath) {
                }
                
                /// OrderBy for member .ctor
                public OrderBy_Key(string QpName, string QpAssociationPath, bool QpBackTrack) : 
                        base(QpName, QpAssociationPath, QpBackTrack) {
                }
                
                /// OrderBy for member 
                public virtual Query.OrderByClauseProperty<T7> Department {
                    get {
                        string temp = QpAssociationPath;
                        return new Query.OrderByClauseProperty<T7>("Key.Department", temp);
                    }
                }
                
                /// OrderBy for member 
                public virtual Query.OrderByClauseProperty<T7> Level {
                    get {
                        string temp = QpAssociationPath;
                        return new Query.OrderByClauseProperty<T7>("Key.Level", temp);
                    }
                }
            }
        }
        
        /// OrderBy for member Root_OrderBy_WeirdClass
        public partial class Root_OrderBy_WeirdClass : OrderBy_WeirdClass<NHibernate.Query.Generator.Tests.ActiveRecord.WeirdClass> {
            
            /// OrderBy for member .ctor
            public Root_OrderBy_WeirdClass() : 
                    base("this", null) {
            }
        }
    }
}
