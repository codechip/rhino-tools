using Ayende.NHibernateQueryAnalyzer.UserInterface.Interfaces;
using Ayende.NHibernateQueryAnalyzer.Utilities;
using log4net;

namespace Ayende.NHibernateQueryAnalyzer.UserInterface
{
	public class ThreadedCommandExecutioner
	{
		private ILog logger = LogManager.GetLogger(typeof (ThreadedCommandExecutioner));
		private readonly IQueue queue;
		private bool runThread;

		public ThreadedCommandExecutioner(IQueue queue)
		{
			this.queue = queue;
			runThread = true;
		}

		public void Run()
		{
			ICommand cmd;
			while (RunThread)
			{
				cmd = (ICommand) queue.Dequeue();
				if (logger.IsDebugEnabled)
				{
					logger.Debug("Running command: " + cmd.Name + ". Command Info: " + cmd.Info);
				}
				cmd.Execute();
			}
		}

		public bool RunThread
		{
			get { return runThread; }
			set { runThread = value; }
		}
	}

}