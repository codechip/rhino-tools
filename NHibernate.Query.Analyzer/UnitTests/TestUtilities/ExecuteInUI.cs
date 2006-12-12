using System;
using Ayende.NHibernateQueryAnalyzer.UserInterface.Interfaces;

namespace Ayende.NHibernateQueryAnalyzer.UnitTests.TestUtilities
{
	/// <summary>
	/// Summary description for ExecuteInUI.
	/// </summary>
	public class ExecuteInUI
	{
		bool executeInUICalled = false;
		bool executeCommandCalled = false;

		public bool ExecuteInUICalled
		{
			get { return executeInUICalled; }
			set { executeInUICalled = value; }
		}

		public bool ExecuteCommandCalled
		{
			get { return executeCommandCalled; }
			set { executeCommandCalled = value; }
		}

		public void Reset()
		{
			executeCommandCalled = false;
			executeInUICalled = false;
		}
		
		public delegate bool ExecuteCommandDelegate(ICommand command);

		public delegate bool ExecuteInUIDelegate(Delegate d, object[] args);

		public bool ExecuteInUIThread(Delegate d, object[] args)
		{
			d.DynamicInvoke(args);
			executeInUICalled = true;
			return true;
		}

		public bool ExecuteCommand(ICommand command)
		{
			command.Execute();
			executeCommandCalled = true;
			return true;
		}
	}
}
