using System;

namespace HumanResources.Model
{
    public class TimesheetEntry : Entity
    {
        public virtual DateTime Start { get; set; }
        public virtual DateTime End { get; set; }
        public virtual Employee Employee { get; set; }
    }
}