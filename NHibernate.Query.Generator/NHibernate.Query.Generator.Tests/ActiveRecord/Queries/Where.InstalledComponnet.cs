//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.42
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Query {
    
    
    public partial class Where {
        
        /// Query for member _root_query_InstalledComponnet
        static Root_Query_InstalledComponnet _root_query_InstalledComponnet = new Root_Query_InstalledComponnet();
        
        /// Query for member InstalledComponnet
        public static Root_Query_InstalledComponnet InstalledComponnet {
            get {
                return _root_query_InstalledComponnet;
            }
        }
        
        /// Query for member Query_InstalledComponnet
        public partial class Query_InstalledComponnet<T1> : Query.QueryBuilder<T1>
         {
            
            /// Query for member .ctor
            public Query_InstalledComponnet(string name, string associationPath) : 
                    base(name, associationPath) {
            }
            
            /// Query for member .ctor
            public Query_InstalledComponnet(string name, string associationPath, bool backTrackAssociationOnEquality) : 
                    base(name, associationPath, backTrackAssociationOnEquality) {
            }
            
            /// Query for member 
            public virtual Query.QueryBuilder<T1> Id {
                get {
                    string temp = associationPath;
                    return new Query.QueryBuilder<T1>("Id", temp);
                }
            }
            
            /// Query for member 
            public virtual Query_Componnet<T1> Component {
                get {
                    string temp = associationPath;
                    temp = ((temp + ".") 
                                + "Component");
                    return new Query_Componnet<T1>("Component", temp, true);
                }
            }
        }
        
        /// Query for member Root_Query_InstalledComponnet
        public partial class Root_Query_InstalledComponnet : Query_InstalledComponnet<NHibernate.Query.Generator.Tests.ActiveRecord.InstalledComponnet> {
            
            /// Query for member .ctor
            public Root_Query_InstalledComponnet() : 
                    base("this", null) {
            }
        }
    }
    
    public partial class OrderBy {
        
        /// Query for member InstalledComponnet
        public partial class InstalledComponnet {
            
            /// Query for member Id
            public static Query.OrderByClause Id {
                get {
                    return new Query.OrderByClause("Id");
                }
            }
        }
    }
    
    public partial class ProjectBy {
    }
    
    public partial class GroupBy {
    }
}
