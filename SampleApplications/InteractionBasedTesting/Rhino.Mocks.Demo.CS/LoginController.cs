using System;
using System.Collections.Generic;

namespace Rhino.Mocks.Demo
{
	public class LoginController
	{
		private readonly IUserRepository userRepository;
		private readonly ISmsSender smsSender;
		private IList<Exception> failuresToSendSms = new List<Exception>();
		private TimeSpan maxDurationForSendingSMS = TimeSpan.FromMilliseconds(500);
		private IList<string> smsSendTookTooLong = new List<string>();

		public IList<string> SmsSendTookTooLong
		{
			get { return smsSendTookTooLong; }
			set { smsSendTookTooLong = value; }
		}

		public TimeSpan MaxDurationForSendingSMS
		{
			get { return maxDurationForSendingSMS; }
			set { maxDurationForSendingSMS = value; }
		}

		public LoginController(IUserRepository userRepository, ISmsSender smsSender)
		{
			this.userRepository = userRepository;
			this.smsSender = smsSender;
		}

		public IList<Exception> FailuresToSendSms
		{
			get { return failuresToSendSms; }
		}

		public void ForgottenPassword(string userName)
		{
			Console.WriteLine("Sending SMS to {0}", userRepository.GetByUserName(userName).Name);
			User user = userRepository.GetByUserName(userName);
			try
			{
				DateTime start = DateTime.Now;
				smsSender.SendSMS(user.Phone, "new password: " + DateTime.Now.Ticks);
				TimeSpan duration = DateTime.Now - start;
				if(duration> maxDurationForSendingSMS)
				{
					string msg = string.Format("Took {0} to send SMS to {1}", duration, user.Name);
					smsSendTookTooLong.Add(msg);
				}
			}
			catch (Exception e)
			{
				failuresToSendSms.Add(e);
			}
		}
	}
}