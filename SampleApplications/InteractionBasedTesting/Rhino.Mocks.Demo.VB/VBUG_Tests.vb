Imports Rhino.Mocks.Constraints
Imports MbUnit.Framework

<TestFixture()> _
Public Class VBUG_Tests

	<Test()> _
	Public Sub SendSMS_WillSendSms_ToAllMembersInGroup()
		Dim mocks As New MockRepository
		Dim smsSender As ISmsSender = mocks.CreateMock(Of ISmsSender)()
		Dim peopleFinder As IPeopleFinder = mocks.Stub(Of IPeopleFinder)()
		Dim expectedNames As String() = {"oren", "yosi"}

		SetupResult.[For](peopleFinder.GetPeopleInGroup("VBUG")) _
		 .[Return](expectedNames)

		smsSender.SendSMS(Nothing, Nothing)
		LastCall.Constraints([Is].Equal("yosi"), Text.StartsWith("new meeting in Raanana at "))

		smsSender.SendSMS(Nothing, Nothing)
		LastCall.Constraints([Is].Equal("oren"), Text.StartsWith("new meeting in Raanana at "))

		mocks.ReplayAll()

		Dim notifier As New MeetingNotifier("VBUG", peopleFinder, smsSender)
		notifier.NotifyMeetingTime(System.DateTime.Today, "Raanana")

		mocks.VerifyAll()
	End Sub

End Class
