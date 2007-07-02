using System;
using System.Collections.Generic;
using System.Text;
using PayRollSystem.Common;

namespace PayRollSystem.Northwind
{
    public class NorthwindOverTimeCalculator : IOverTimeCalculator
    {
        public int GetAmountOfOverTimeInHours(int hoursWorked, TimeSpan duration)
        {
            return 40;
        }
    }
}
