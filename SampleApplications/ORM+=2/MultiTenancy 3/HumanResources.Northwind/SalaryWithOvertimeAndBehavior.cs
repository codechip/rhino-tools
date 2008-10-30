using System;
using HumanResources.Model;
using System.Linq;

namespace HumanResources.Northwind
{
    public class SalaryWithOvertimeAndBehavior : Salary
    {
        public virtual decimal OvertimeHourlySalary { get; set; }

        public override decimal CalculateSalaryFor(System.Collections.Generic.IEnumerable<TimesheetEntry> timesheets)
        {
            decimal baseSalary = base.CalculateSalaryFor(timesheets);
            double overtimeHours = timesheets
                .Where(x => x.Duration > 9)
                .Sum(x => x.Duration);
            return baseSalary + ((decimal)overtimeHours * OvertimeHourlySalary);
        }
    }
}