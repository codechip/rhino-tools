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
	Public Sub Calculate_CanAddHoursWorked_WillRetainHoursWorked()
		Dim calculator As New SalaryCalculator

		calculator.AddHoursWorked(DateTime.Today, 8)

		Assert.AreEqual(8, calculator.TotalHoursWorked)
	End Sub

	<Test()> _
	Public Sub Calculate_CanAddHoursWorked_CanGetHoursWorkedOnSpecificDay()
		Dim calculator As New SalaryCalculator

		calculator.AddHoursWorked(DateTime.Today, 8)

		Assert.AreEqual(8, calculator.GetHoursWorkedAt(DateTime.Today))
	End Sub


End Class
