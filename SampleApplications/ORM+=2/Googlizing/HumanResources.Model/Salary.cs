using System.Collections.Generic;
using System.Linq;
using NHibernate.Search.Attributes;
using NHibernate.Search.Bridge.Builtin;

namespace HumanResources.Model
{
    [Indexed]
    public class Salary
    {
        [DocumentId]
        public virtual long Id { get; set; }
        
        [Field(Index.Tokenized)]
        public virtual string Name { get; set;  }
        
        [Field(Index.Tokenized)]
        [FieldBridge(typeof(StringBridge))]
        public virtual decimal HourlyRate { get; set; }

        public virtual decimal CalculateSalaryFor(IEnumerable<TimesheetEntry> timesheet)
        {
            double totalHours = timesheet.Sum(e => (e.End - e.Start).TotalHours);
            return (decimal)totalHours * HourlyRate;
        }
    }
}