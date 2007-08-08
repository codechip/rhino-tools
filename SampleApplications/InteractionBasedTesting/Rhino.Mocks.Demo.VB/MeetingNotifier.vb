Public Class MeetingNotifier

	Private peopleFinder As IPeopleFinder
	Private smsSender As ISmsSender
	Private groupName As String
	Public Sub New(ByVal groupName As String, ByVal peopleFinder As IPeopleFinder, ByVal smsSender As ISmsSender)
		Me.peopleFinder = peopleFinder
		Me.smsSender = smsSender
		Me.groupName = groupName
	End Sub

	Public Sub NotifyMeetingTime(ByVal newMeeetingTime As System.DateTime, ByVal location As String)

		For Each name As String In peopleFinder.GetPeopleInGroup(groupName)
			Dim msgToSend As String = "new meeting in " & location & " at " & newMeeetingTime
			smsSender.SendSMS(name, msgToSend)
		Next

	End Sub

End Class
