using NHibernate.Search.Attributes;

namespace HumanResources.Model
{
    [Indexed]
    public class Employee
    {
        [DocumentId]
        public virtual long Id { get; set; }
        
        [Field(Index.Tokenized)]
        public virtual string Name { get; set; }
        
        public virtual Salary Salary { get; set; }
    }
}
