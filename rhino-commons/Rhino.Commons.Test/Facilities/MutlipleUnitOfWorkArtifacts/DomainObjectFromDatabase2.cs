namespace Rhino.Commons.Test.Facilities.MutlipleUnitOfWorkArtifacts
{
    public class DomainObjectFromDatabase2
    {
        private int id;
        private string name;

        protected DomainObjectFromDatabase2() 
        { 
        }

        public DomainObjectFromDatabase2(string name)
        {
            this.name = name;
        }

        public virtual int Id
        {
            get { return id; }
            set { id = value; }
        }

        public virtual string Name
        {
            get { return name; }
            set { name = value; }
        }
    }
}