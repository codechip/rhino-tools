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
        
        /// Query Builder for member Componnet
        public static Query.QueryBuilder<NHibernate.Query.Generator.Tests.ActiveRecord.Componnet> Componnet {
            get {
                return new Query.QueryBuilder<NHibernate.Query.Generator.Tests.ActiveRecord.Componnet>();
            }
        }
    }
    
    public partial class Where {
        
        /// Where for member _root_Where_Componnet
        static Root_Where_Componnet _root_Where_Componnet = new Root_Where_Componnet();
        
        /// Where for member Componnet
        public static Root_Where_Componnet Componnet {
            get {
                return _root_Where_Componnet;
            }
        }
        
        /// Where for member Where_Componnet
        public partial class Where_Componnet<T1> : Query.WhereClause<T1>
         {
            
            /// Where for member .ctor
            public Where_Componnet(string name, string associationPath) : 
                    base(name, associationPath) {
            }
            
            /// Where for member .ctor
            public Where_Componnet(string name, string associationPath, bool backTrack) : 
                    base(name, associationPath, backTrack) {
            }
            
            /// Where for member 
            public virtual Query.WhereClauseProperty<T1> Version {
                get {
                    string temp = associationPath;
                    return new Query.WhereClauseProperty<T1>("Version", temp);
                }
            }
            
            /// Where for member 
            public virtual Query.WhereClause<T1> Id {
                get {
                    string temp = associationPath;
                    return new Query.WhereClause<T1>("Id", temp);
                }
            }
        }
        
        /// Where for member Root_Where_Componnet
        public partial class Root_Where_Componnet : Where_Componnet<NHibernate.Query.Generator.Tests.ActiveRecord.Componnet> {
            
            /// Where for member .ctor
            public Root_Where_Componnet() : 
                    base("this", null) {
            }
        }
    }
    
    public partial class ProjectBy {
        
        /// Projection for member _root_Projection_Componnet
        static Root_Projection_Componnet _root_Projection_Componnet = new Root_Projection_Componnet();
        
        /// Projection for member Componnet
        public static Root_Projection_Componnet Componnet {
            get {
                return _root_Projection_Componnet;
            }
        }
        
        /// Projection for member Projection_Componnet
        public partial class Projection_Componnet<T2> : Query.ProjectionEntity<T2>
         {
            
            /// Projection for member .ctor
            public Projection_Componnet(string name, string associationPath) : 
                    base(name, associationPath) {
            }
            
            /// Projection for member .ctor
            public Projection_Componnet(string name, string associationPath, bool backTrack) : 
                    base(name, associationPath, backTrack) {
            }
            
            /// Projection for member 
            public virtual Query.ProjectionClausePropertyNumeric<T2> Version {
                get {
                    string temp = associationPath;
                    return new Query.ProjectionClausePropertyNumeric<T2>("Version", temp);
                }
            }
            
            /// Projection for member 
            public virtual Query.ProjectionClausePropertyNumeric<T2> Id {
                get {
                    string temp = associationPath;
                    return new Query.ProjectionClausePropertyNumeric<T2>("Id", temp);
                }
            }
        }
        
        /// Projection for member Root_Projection_Componnet
        public partial class Root_Projection_Componnet : Projection_Componnet<NHibernate.Query.Generator.Tests.ActiveRecord.Componnet> {
            
            /// Projection for member .ctor
            public Root_Projection_Componnet() : 
                    base("this", null) {
            }
        }
    }
    
    public partial class OrderBy {
        
        /// OrderBy for member _root_OrderBy_Componnet
        static Root_OrderBy_Componnet _root_OrderBy_Componnet = new Root_OrderBy_Componnet();
        
        /// OrderBy for member Componnet
        public static Root_OrderBy_Componnet Componnet {
            get {
                return _root_OrderBy_Componnet;
            }
        }
        
        /// OrderBy for member OrderBy_Componnet
        public partial class OrderBy_Componnet<T3> : Query.QueryPart
         {
            
            /// OrderBy for member .ctor
            public OrderBy_Componnet(string name, string associationPath) : 
                    base(name, associationPath) {
            }
            
            /// OrderBy for member .ctor
            public OrderBy_Componnet(string name, string associationPath, bool backTrack) : 
                    base(name, associationPath, backTrack) {
            }
            
            /// OrderBy for member 
            public virtual Query.OrderByClauseProperty<T3> Version {
                get {
                    string temp = associationPath;
                    return new Query.OrderByClauseProperty<T3>("Version", temp);
                }
            }
            
            /// OrderBy for member 
            public virtual Query.OrderByClauseProperty<T3> Id {
                get {
                    string temp = associationPath;
                    return new Query.OrderByClauseProperty<T3>("Id", temp);
                }
            }
        }
        
        /// OrderBy for member Root_OrderBy_Componnet
        public partial class Root_OrderBy_Componnet : OrderBy_Componnet<NHibernate.Query.Generator.Tests.ActiveRecord.Componnet> {
            
            /// OrderBy for member .ctor
            public Root_OrderBy_Componnet() : 
                    base("this", null) {
            }
        }
    }
}
