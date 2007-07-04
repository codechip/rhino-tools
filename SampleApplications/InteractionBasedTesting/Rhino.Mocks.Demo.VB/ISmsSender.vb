Public Interface ISmsSender
	Sub SendSMS(ByVal phone As String, ByVal message As String)
End Interface