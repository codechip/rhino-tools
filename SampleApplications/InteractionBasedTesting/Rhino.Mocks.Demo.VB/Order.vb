Imports System
Imports System.Data.SqlClient
Imports System.Runtime.CompilerServices

Public Class Order
	Private m_total As Decimal

	Public Property Total() As Decimal
		Get
			Return m_total
		End Get
		Set(ByVal value As Decimal)
			m_total = value
		End Set
	End Property

	Public Function GetTotal() As Decimal
		Dim salesHistory As Decimal
		salesHistory = GetSalesHistory()
		If salesHistory >= 1000 Then
			If m_total >= 100 Then
				Return m_total - (m_total * 0.1D)
			ElseIf m_total >= 60 Then
				Return m_total - (m_total * 0.05D)
			End If
		End If
		Return m_total
	End Function

	Protected Friend Overridable Function GetSalesHistory() As Decimal
		Dim salesHistory As Decimal
		Using sqlConnection As New SqlConnection("dummy")
			sqlConnection.Open()
			salesHistory = DataAccess.GetSalesHistory(sqlConnection, Me)
		End Using
		Return salesHistory
	End Function
End Class

Public NotInheritable Class DataAccess
	Private Sub New()
	End Sub
	Shared random As New Random()
	Public Shared Function GetSalesHistory(ByVal connection As SqlConnection, ByVal order As Order) As Decimal
		Return random.[Next](10, 10000)
	End Function
End Class

