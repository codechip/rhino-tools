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
        
        /// Query Builder for member DerivedBlog2
        public static Query.QueryBuilder<NHibernate.Query.Generator.Tests.ActiveRecord.DerivedBlog2> DerivedBlog2 {
            get {
                return new Query.QueryBuilder<NHibernate.Query.Generator.Tests.ActiveRecord.DerivedBlog2>();
            }
        }
    }
    
    public partial class Where {
        
        /// Where for member _root_Where_DerivedBlog2
        static Root_Where_DerivedBlog2 _root_Where_DerivedBlog2 = new Root_Where_DerivedBlog2();
        
        /// Where for member DerivedBlog2
        public static Root_Where_DerivedBlog2 DerivedBlog2 {
            get {
                return _root_Where_DerivedBlog2;
            }
        }
        
        /// Where for member Where_DerivedBlog2
        public partial class Where_DerivedBlog2<T1> : Query.WhereClause<T1>
         {
            
            /// Where for member .ctor
            public Where_DerivedBlog2(string name, string associationPath) : 
                    base(name, associationPath) {
            }
            
            /// Where for member .ctor
            public Where_DerivedBlog2(string name, string associationPath, bool backTrack) : 
                    base(name, associationPath, backTrack) {
            }
            
            /// Where for member 
            public virtual Query.WhereClauseProperty<T1> Name {
                get {
                    string temp = associationPath;
                    return new Query.WhereClauseProperty<T1>("Name", temp);
                }
            }
            
            /// Where for member 
            public virtual Query.WhereClauseProperty<T1> Attribute {
                get {
                    string temp = associationPath;
                    return new Query.WhereClauseProperty<T1>("Attribute", temp);
                }
            }
            
            /// Where for member 
            public virtual Query.WhereClause<T1> Id {
                get {
                    string temp = associationPath;
                    return new Query.WhereClause<T1>("Id", temp);
                }
            }
            
            /// Where for member 
            public virtual Where_User<T1> Author {
                get {
                    string temp = associationPath;
                    temp = ((temp + ".") 
                                + "Author");
                    return new Where_User<T1>("Author", temp, true);
                }
            }
        }
        
        /// Where for member Root_Where_DerivedBlog2
        public partial class Root_Where_DerivedBlog2 : Where_DerivedBlog2<NHibernate.Query.Generator.Tests.ActiveRecord.DerivedBlog2> {
            
            /// Where for member .ctor
            public Root_Where_DerivedBlog2() : 
                    base("this", null) {
            }
        }
    }
    
    public partial class ProjectBy {
        
        /// Projection for member _root_Projection_DerivedBlog2
        static Root_Projection_DerivedBlog2 _root_Projection_DerivedBlog2 = new Root_Projection_DerivedBlog2();
        
        /// Projection for member DerivedBlog2
        public static Root_Projection_DerivedBlog2 DerivedBlog2 {
            get {
                return _root_Projection_DerivedBlog2;
            }
        }
        
        /// Projection for member Projection_DerivedBlog2
        public partial class Projection_DerivedBlog2<T2> : Query.ProjectionEntity<T2>
         {
            
            /// Projection for member .ctor
            public Projection_DerivedBlog2(string name, string associationPath) : 
                    base(name, associationPath) {
            }
            
            /// Projection for member .ctor
            public Projection_DerivedBlog2(string name, string associationPath, bool backTrack) : 
                    base(name, associationPath, backTrack) {
            }
            
            /// Projection for member 
            public virtual Query.ProjectionClausePropertyNumeric<T2> Name {
                get {
                    string temp = associationPath;
                    return new Query.ProjectionClausePropertyNumeric<T2>("Name", temp);
                }
            }
            
            /// Projection for member 
            public virtual Query.ProjectionClausePropertyNumeric<T2> Attribute {
                get {
                    string temp = associationPath;
                    return new Query.ProjectionClausePropertyNumeric<T2>("Attribute", temp);
                }
            }
            
            /// Projection for member 
            public virtual Query.ProjectionClausePropertyNumeric<T2> Id {
                get {
                    string temp = associationPath;
                    return new Query.ProjectionClausePropertyNumeric<T2>("Id", temp);
                }
            }
            
            /// Projection for member 
            public virtual Projection_User<T2> Author {
                get {
                    string temp = associationPath;
                    temp = ((temp + ".") 
                                + "Author");
                    return new Projection_User<T2>("Author", temp, true);
                }
            }
        }
        
        /// Projection for member Root_Projection_DerivedBlog2
        public partial class Root_Projection_DerivedBlog2 : Projection_DerivedBlog2<NHibernate.Query.Generator.Tests.ActiveRecord.DerivedBlog2> {
            
            /// Projection for member .ctor
            public Root_Projection_DerivedBlog2() : 
                    base("this", null) {
            }
        }
    }
    
    public partial class OrderBy {
        
        /// OrderBy for member _root_OrderBy_DerivedBlog2
        static Root_OrderBy_DerivedBlog2 _root_OrderBy_DerivedBlog2 = new Root_OrderBy_DerivedBlog2();
        
        /// OrderBy for member DerivedBlog2
        public static Root_OrderBy_DerivedBlog2 DerivedBlog2 {
            get {
                return _root_OrderBy_DerivedBlog2;
            }
        }
        
        /// OrderBy for member OrderBy_DerivedBlog2
        public partial class OrderBy_DerivedBlog2<T3> : Query.QueryPart
         {
            
            /// OrderBy for member .ctor
            public OrderBy_DerivedBlog2(string name, string associationPath) : 
                    base(name, associationPath) {
            }
            
            /// OrderBy for member .ctor
            public OrderBy_DerivedBlog2(string name, string associationPath, bool backTrack) : 
                    base(name, associationPath, backTrack) {
            }
            
            /// OrderBy for member 
            public virtual Query.OrderByClauseProperty<T3> Name {
                get {
                    string temp = associationPath;
                    return new Query.OrderByClauseProperty<T3>("Name", temp);
                }
            }
            
            /// OrderBy for member 
            public virtual Query.OrderByClauseProperty<T3> Attribute {
                get {
                    string temp = associationPath;
                    return new Query.OrderByClauseProperty<T3>("Attribute", temp);
                }
            }
            
            /// OrderBy for member 
            public virtual Query.OrderByClauseProperty<T3> Id {
                get {
                    string temp = associationPath;
                    return new Query.OrderByClauseProperty<T3>("Id", temp);
                }
            }
            
            /// OrderBy for member 
            public virtual OrderBy_User<T3> Author {
                get {
                    string temp = associationPath;
                    temp = ((temp + ".") 
                                + "Author");
                    return new OrderBy_User<T3>("Author", temp, true);
                }
            }
        }
        
        /// OrderBy for member Root_OrderBy_DerivedBlog2
        public partial class Root_OrderBy_DerivedBlog2 : OrderBy_DerivedBlog2<NHibernate.Query.Generator.Tests.ActiveRecord.DerivedBlog2> {
            
            /// OrderBy for member .ctor
            public Root_OrderBy_DerivedBlog2() : 
                    base("this", null) {
            }
        }
    }
}
