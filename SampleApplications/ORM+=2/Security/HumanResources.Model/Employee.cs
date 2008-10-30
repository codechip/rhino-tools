using System.Linq;
using System.Text;
using Rhino.Security;

namespace HumanResources.Model
{
    public class Employee : Entity, IUser
    {
        public virtual string Name { get; set; }
        public virtual Salary Salary { get; set; }

        public virtual SecurityInfo SecurityInfo
        {
            get { return new SecurityInfo(Name, Id); }
        }
    }
}
