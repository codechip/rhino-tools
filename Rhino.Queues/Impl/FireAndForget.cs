using System;
using System.Threading;
using log4net;

namespace Rhino.Queues.Impl
{
	/// <summary>
	/// This class is used to attempt recovery of critical operation 
	/// in the framework. If the operation fails to execute five times in a row,
	/// we declare red alert and raise a warning flag. 
	/// We do not abort because this is something that a framework should never do to its
	/// application.
	/// </summary>
	public class FireAndForget
	{
		public static event Action QueueAndMessageStateIsLikelyCorrupted = delegate { };

		public static Func<double, TimeSpan> GetWaitTimespan = TimeSpan.FromSeconds;

		private readonly ILog logger = LogManager.GetLogger(typeof (FireAndForget));
		readonly Timer timer;
		private readonly Action action;
		private int retryCount = 1;



		public FireAndForget(Action action)
		{
			this.action = action;
			timer = new Timer(OnExecuteTimer, null,
					GetWaitTimespan(1),
					TimeSpan.FromMilliseconds(-1));
		}

		private void OnExecuteTimer(object state)
		{
			try
			{
				action();
				timer.Dispose();
			}
			catch (Exception e )
			{
				logger.Error("Failed to execute action "+ action + " for the "+ retryCount + " time.", e);
				retryCount += 1;
				if(retryCount<6)
				{
					timer.Change(GetWaitTimespan(retryCount), TimeSpan.FromMilliseconds(-1));
					return;
				}
				logger.Fatal("Failed to execute action " + action + " for the " + retryCount + " time. This is the final attempt. Queues/Messages state is likely corrupted.", e);
				timer.Dispose();
				QueueAndMessageStateIsLikelyCorrupted();
			}
		}

		public static void Exceute(Action action)
		{
			new FireAndForget(action);
		}

	}
}