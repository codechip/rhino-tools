using System;
using System.Collections.Generic;
using System.Text;
using PayRollSystem.Common;

namespace PayRollSystem.Southsand
{
    public class SouthsandEmployee : Employee
    {
        decimal globalSalary;
        decimal overtimeHourlySalary;

        public virtual decimal GlobalSalary
        {
            get { return globalSalary; }
            set { globalSalary = value; }
        }

        public virtual decimal OvertimeHourlySalary
        {
            get { return overtimeHourlySalary; }
            set { overtimeHourlySalary = value; }
        }

        public override decimal CalculateSalary(int hoursWorked, TimeSpan duration)
        {
            decimal pay = GlobalSalary;
            if (HasWorkedOvertime(hoursWorked, duration))
            {
                int overtimeHours = GetHoursWorkedOvertime(hoursWorked, duration);
                pay += overtimeHours * OvertimeHourlySalary;
            }
            return pay;
        }
    }
}
