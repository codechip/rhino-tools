using System;
using System.Collections.Generic;
using System.Text;
using PayRollSystem.Common;

namespace PayRollSystem.Northwind
{
    public class NorthwindEmployee : Employee
    {
        decimal hourlySalary;
        decimal overtimeBonus;

        public virtual decimal HourlySalary
        {
            get { return hourlySalary; }
            set { hourlySalary = value; }
        }

        public virtual decimal OvertimeBonus
        {
            get { return overtimeBonus; }
            set { overtimeBonus = value; }
        }

        public override decimal CalculateSalary(int hoursWorked, TimeSpan duration)
        {
            decimal pay = HourlySalary * hoursWorked;

            if (HasWorkedOvertime(hoursWorked, duration))
                pay += OvertimeBonus;
            return pay;
        }
    }
}
