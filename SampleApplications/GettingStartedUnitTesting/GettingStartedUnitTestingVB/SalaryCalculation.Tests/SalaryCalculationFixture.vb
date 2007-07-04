Imports MbUnit.Framework

<TestFixture()> _
Public Class SalaryCalculationFixture
    <Test()> _
    Public Sub CalculateSalary_NoHoursWorked_WillReturnZero()
        Dim calculator As New SalaryCalculator
        Dim result As Decimal

        result = calculator.Calculate()

        Assert.AreEqual(0, result)

    End Sub

    <Test()> _
    Public Sub CalculateSalary_HourWorkedButNoHourlySalarySet_WillReturnZero()
        Dim calculator As New SalaryCalculator
        Dim result As Decimal

        calculator.AddHoursWorked(New DateTime(2007, 7, 1), 8.5)

        result = calculator.Calculate()

        Assert.AreEqual(0, result)
    End Sub

End Class
