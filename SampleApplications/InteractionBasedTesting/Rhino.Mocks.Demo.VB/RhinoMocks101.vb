Imports System
Imports System.Net
Imports MbUnit.Framework
Imports Rhino.Mocks.Constraints
Imports Rhino.Mocks.Interfaces

<TestFixture()> _
 Public Class RhinoMocks101Fixture
	Private mocks As MockRepository
	Private smsSender As ISmsSender
	Private user As User
	Private userRepository As IUserRepository
	Private loadCalled As Boolean

	<SetUp()> _
	Public Sub TestInitialize()
		mocks = New MockRepository()
		smsSender = mocks.CreateMock(Of ISmsSender)()

		user = New User("sally", "1234")

		userRepository = mocks.Stub(Of IUserRepository)()
		SetupResult.[For](userRepository.GetByUserName("sally")).[Return](user)
	End Sub

	<TearDown()> _
	Public Sub TestCleanup()
		mocks.VerifyAll()
	End Sub


	<Test()> _
	Public Sub CanSendSMSWhenUserRequestForgottenPasswordReminder()
		smsSender.SendSMS(Nothing, Nothing)
		LastCall.Constraints([Is].Equal("1234"), Text.StartsWith("new password: ")).Message("Should send sms")
		mocks.ReplayAll()

		Dim controller As New LoginController(userRepository, smsSender)
		controller.ForgottenPassword("sally")
	End Sub


	<Test()> _
	Public Sub WillRecordFailuresOfSendingSMS()
		smsSender.SendSMS(Nothing, Nothing)
		LastCall.IgnoreArguments().[Throw](New WebException("blah"))

		mocks.ReplayAll()

		Dim controller As New LoginController(userRepository, smsSender)
		controller.ForgottenPassword("sally")

		mocks.VerifyAll()

		Assert.AreEqual(1, controller.FailuresToSendSms.Count)
	End Sub

	<Test()> _
	Public Sub SendSmsThatTakesMoreThanSpecifiedTimeFlagsAnError()
		smsSender.SendSMS(Nothing, Nothing)
		'
		'			LastCall.IgnoreArguments().Do((SendSMSDelegate) delegate
		'			{
		'				Thread.Sleep(750);
		'			});
		'

		LastCall.IgnoreArguments()


		mocks.ReplayAll()

		Dim controller As New LoginController(userRepository, smsSender)
		controller.MaxDurationForSendingSMS = TimeSpan.FromSeconds(-1)
		controller.ForgottenPassword("sally")

		mocks.VerifyAll()

		Assert.AreEqual(1, controller.SmsSendTookTooLong.Count)
	End Sub

	Public Delegate Sub SendSMSDelegate(ByVal phone As String, ByVal msg As String)

	<Test()> _
	Public Sub PreferedCustomerWillGetDiscountOnOrderWhoseTotalGreaterThan1000Dollars()
		Dim order As Order = mocks.PartialMock(Of Order)()

		Expect.[Call](order.GetSalesHistory()).[Return](1500D)

		mocks.ReplayAll()

		order.Total = 100
		Dim total As Decimal = order.GetTotal()
		Assert.AreEqual(90, total)
	End Sub

	<Test()> _
	Public Sub Events()
		Dim view As IView = mocks.CreateMock(Of IView)()
		AddHandler view.Load, Nothing
		Dim eventRaiser As IEventRaiser = LastCall.IgnoreArguments().GetEventRaiser()
		mocks.ReplayAll()
		AddHandler view.Load, AddressOf view_Load

		eventRaiser.Raise(Me, EventArgs.Empty)

		Assert.IsTrue(loadCalled)
	End Sub

	Private Sub view_Load(ByVal sender As Object, ByVal e As EventArgs)
		Me.loadCalled = True
	End Sub
End Class

Public Interface IView
	Event Load As EventHandler
End Interface
