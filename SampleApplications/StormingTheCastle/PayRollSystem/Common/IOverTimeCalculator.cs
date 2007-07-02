using System;
using System.Collections.Generic;
using System.Text;

namespace PayRollSystem.Common
{
    public interface IOverTimeCalculator
    {
        int GetAmountOfOverTimeInHours(int hoursWorked, TimeSpan duration);
    }
}
