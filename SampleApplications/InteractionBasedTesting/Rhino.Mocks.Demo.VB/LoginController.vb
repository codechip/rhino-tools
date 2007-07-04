Imports System
Imports System.Collections.Generic

Public Class LoginController
	Private ReadOnly userRepository As IUserRepository
	Private ReadOnly smsSender As ISmsSender
	Private m_failuresToSendSms As IList(Of Exception) = New List(Of Exception)()
	Private m_maxDurationForSendingSMS As TimeSpan = TimeSpan.FromMilliseconds(500)
	Private m_smsSendTookTooLong As IList(Of String) = New List(Of String)()

	Public Property SmsSendTookTooLong() As IList(Of String)
		Get
			Return m_smsSendTookTooLong
		End Get
		Set(ByVal value As IList(Of String))
			m_smsSendTookTooLong = Value
		End Set
	End Property

	Public Property MaxDurationForSendingSMS() As TimeSpan
		Get
			Return m_maxDurationForSendingSMS
		End Get
		Set(ByVal value As TimeSpan)
			m_maxDurationForSendingSMS = Value
		End Set
	End Property

	Public Sub New(ByVal userRepository As IUserRepository, ByVal smsSender As ISmsSender)
		Me.userRepository = userRepository
		Me.smsSender = smsSender
	End Sub

	Public ReadOnly Property FailuresToSendSms() As IList(Of Exception)
		Get
			Return m_failuresToSendSms
		End Get
	End Property

	Public Sub ForgottenPassword(ByVal userName As String)
		Console.WriteLine("Sending SMS to {0}", userRepository.GetByUserName(userName).Name)
		Dim user As User = userRepository.GetByUserName(userName)
		Try
			Dim start As DateTime = DateTime.Now
			smsSender.SendSMS(user.Phone, "new password: " & DateTime.Now.Ticks)
			Dim duration As TimeSpan = DateTime.Now - start
			If duration > m_maxDurationForSendingSMS Then
				Dim msg As String = String.Format("Took {0} to send SMS to {1}", duration, user.Name)
				m_smsSendTookTooLong.Add(msg)
			End If
		Catch e As Exception
			m_failuresToSendSms.Add(e)
		End Try
	End Sub
End Class
