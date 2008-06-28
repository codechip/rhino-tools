namespace Rhino.Commons.Test.Facilities.MutlipleUnitOfWorkArtifacts
{
    public class DomainObjectFromDatabase1
    {
        private int id;
        private string name;

        protected DomainObjectFromDatabase1() 
        { 
        }

        public DomainObjectFromDatabase1(string name)
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