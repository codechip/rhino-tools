using System;

namespace HumanResources.Model
{
    public class TimesheetEntry
    {
        public virtual long Id { get; set; }
        public virtual DateTime Start { get; set; }
        public virtual DateTime End { get; set; }
        public virtual Employee Employee { get; set; }

        public virtual double Duration
        {
            get { return (End - Start).TotalHours; }
        }
    }
}