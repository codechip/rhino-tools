using System;
using System.Collections.Generic;
using System.Text;

namespace PayRollSystem.Common
{
    public abstract class Employee
    {
        int id;
        string title;
        string name;

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

        public virtual string Title
        {
            get { return title; }
            set { title = value; }
        }

        public virtual int HoursPerWeek
        {
            get { return 45; }
        }

        /// <summary>
        /// Calculate the salary of the employee, based on the amount of hours 
        /// worked in a certain duration (usually calculated per week basis.
        /// </summary>
        public abstract decimal CalculateSalary(int hoursWorked, TimeSpan duration);

        protected virtual bool HasWorkedOvertime(int hoursWorked, TimeSpan duration)
        {
            int totalHoursShouldHaveWorked = GetTotalHoursShouldHaveWorked(duration);
            return totalHoursShouldHaveWorked < hoursWorked;
        }

        protected virtual int GetHoursWorkedOvertime(int hoursWorked, TimeSpan duration)
        {
            return hoursWorked - GetTotalHoursShouldHaveWorked(duration);
        }

        private int GetTotalHoursShouldHaveWorked(TimeSpan duration)
        {
            int numberOfWeeks = (int)duration.TotalDays / 7;
            int totalHoursWithOvertime = numberOfWeeks * HoursPerWeek;

            return totalHoursWithOvertime;
        }
    }
}
