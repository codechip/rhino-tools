Public Class User
	Private ReadOnly m_name As String
	Private ReadOnly m_phone As String

	Public ReadOnly Property Name() As String
		Get
			Return m_name
		End Get
	End Property

	Public ReadOnly Property Phone() As String
		Get
			Return m_phone
		End Get
	End Property

	Public Sub New(ByVal name As String, ByVal phone As String)
		Me.m_name = name
		Me.m_phone = phone
	End Sub
End Class
