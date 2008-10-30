using System.Collections.Generic;
using System.Linq;

namespace HumanResources.Model
{
    public class Salary
    {
        public virtual long Id { get; set; }
        public virtual string Name { get; set;  }
        public virtual decimal HourlyRate { get; set; }

        public virtual decimal CalculateSalaryFor(IEnumerable<TimesheetEntry> timesheet)
        {
            double totalHours = timesheet.Sum(e => (e.End - e.Start).TotalHours);
            return (decimal)totalHours * HourlyRate;
        }
    }
}