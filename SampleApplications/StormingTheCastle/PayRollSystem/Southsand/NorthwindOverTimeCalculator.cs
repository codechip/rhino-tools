using System;
using System.Collections.Generic;
using System.Text;
using PayRollSystem.Common;

namespace PayRollSystem.Southsand
{
    public class SouthsandOverTimeCalculator : IOverTimeCalculator
    {
        public int GetAmountOfOverTimeInHours(int hoursWorked, TimeSpan duration)
        {
            return 20;
        }
    }
}
