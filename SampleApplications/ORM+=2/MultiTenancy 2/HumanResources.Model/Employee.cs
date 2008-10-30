using System.Linq;
using System.Text;

namespace HumanResources.Model
{
    public class Employee
    {
        public virtual long Id { get; set; }
        public virtual string Name { get; set; }
        public virtual Salary Salary { get; set; }
    }
}
