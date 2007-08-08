Public Class SalaryCalculator

	Private _totalHoursWorked As Decimal
	Private _hoursWorkedAtDay As New Dictionary(Of DateTime, Decimal)

	Public Property TotalHoursWorked() As Decimal
		Get
			Return _totalHoursWorked
		End Get
		Set(ByVal value As Decimal)
			_totalHoursWorked = value
		End Set
	End Property

	Public Function GetHoursWorkedAt(ByVal day As DateTime) As Decimal
		Return _hoursWorkedAtDay(day)
	End Function

	Public Sub AddHoursWorked(ByVal dayWorked As DateTime, ByVal hoursWorked As Decimal)
		_totalHoursWorked += hoursWorked
		_hoursWorkedAtDay.Add(dayworked, hoursWorked)
	End Sub

	Public Function Calculate() As Decimal
		Return 0
	End Function
End Class
