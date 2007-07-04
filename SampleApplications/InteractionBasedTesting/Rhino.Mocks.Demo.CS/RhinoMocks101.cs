using System;
using System.Net;
using System.Threading;
using MbUnit.Framework;
using Rhino.Mocks.Constraints;
using Rhino.Mocks.Interfaces;

namespace Rhino.Mocks.Demo
{
	[TestFixture]
	public class RhinoMocks101Fixture
	{
		private MockRepository mocks;
		private ISmsSender smsSender;
		private User user;
		private IUserRepository userRepository;
		private bool loadCalled;

		[SetUp]
		public void TestInitialize()
		{
			mocks = new MockRepository();
			smsSender = mocks.CreateMock<ISmsSender>();

			user = new User("sally", "1234");

			userRepository = mocks.Stub<IUserRepository>();
			SetupResult.For(userRepository.GetByUserName("sally"))
				.Return(user);
		}

		[TearDown]
		public void TestCleanup()
		{
			mocks.VerifyAll();
		}


		[Test]
		public void CanSendSMSWhenUserRequestForgottenPasswordReminder()
		{
			smsSender.SendSMS(null, null);
			LastCall.Constraints(Is.Equal("1234"),
			                     Text.StartsWith("new password: "))
													 .Message("Should send sms");
			mocks.ReplayAll();

			LoginController controller = new LoginController(userRepository, smsSender);
			controller.ForgottenPassword("sally");
		}


		[Test]
		public void WillRecordFailuresOfSendingSMS()
		{
			smsSender.SendSMS(null, null);
			LastCall.IgnoreArguments().Throw(new WebException("blah"));

			mocks.ReplayAll();

			LoginController controller = new LoginController(userRepository, smsSender);
			controller.ForgottenPassword("sally");

			mocks.VerifyAll();

			Assert.AreEqual(1, controller.FailuresToSendSms.Count);
		}

		[Test]
		public void SendSmsThatTakesMoreThanSpecifiedTimeFlagsAnError()
		{
			smsSender.SendSMS(null, null);
/*
			LastCall.IgnoreArguments().Do((SendSMSDelegate) delegate
			{
				Thread.Sleep(750);
			});
*/
			LastCall.IgnoreArguments();


			mocks.ReplayAll();

			LoginController controller = new LoginController(userRepository, smsSender);
			controller.MaxDurationForSendingSMS = TimeSpan.FromSeconds(-1);
			controller.ForgottenPassword("sally");

			mocks.VerifyAll();

			Assert.AreEqual(1, controller.SmsSendTookTooLong.Count);
		}

		public delegate void SendSMSDelegate(string phone, string msg);

		[Test]
		public void PreferedCustomerWillGetDiscountOnOrderWhoseTotalGreaterThan1000Dollars()
		{
			Order order = mocks.PartialMock<Order>();

			Expect.Call(order.GetSalesHistory()).Return(1500m);

			mocks.ReplayAll();

			order.Total = 100;
			decimal total = order.GetTotal();
			Assert.AreEqual(90, total );
		}

		[Test]
		public void Events()
		{
			IView view = mocks.CreateMock<IView>();
			view.Load += null;
			IEventRaiser eventRaiser = 
				LastCall.IgnoreArguments().GetEventRaiser();
			mocks.ReplayAll();
			view.Load+=new EventHandler(view_Load);

			eventRaiser .Raise(this, EventArgs.Empty);

			Assert.IsTrue(loadCalled);
		}

		private void view_Load(object sender, EventArgs e)
		{
			this.loadCalled = true;
		}
	}

	public interface IView
	{
		event EventHandler Load;
	}
}