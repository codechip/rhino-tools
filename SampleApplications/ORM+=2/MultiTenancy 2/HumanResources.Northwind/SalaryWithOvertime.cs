using HumanResources.Model;

namespace HumanResources.Northwind
{
    public class SalaryWithOvertime : Salary
    {
        public virtual decimal OvertimeHourlySalary { get; set; } 
    }
}