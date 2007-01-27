using Castle.Core;
using Castle.Facilities.AutomaticTransactionManagement;
using Castle.Services.Transaction;
using Rhino.Commons.Facilities;

namespace Rhino.Commons
{
	public class RhinoTransactionFacility : TransactionFacility
	{
		protected override void Init()
		{
			base.Init();//set the inspector for the transactional components
			Kernel.ComponentCreated += Kernel_ComponentCreated;
			SetUpTransactionManager();
		}

		private void SetUpTransactionManager()
		{
			if (!Kernel.HasComponent(typeof(ITransactionManager)))
			{
				Kernel.AddComponent("rhino.transaction.manager",
									typeof(ITransactionManager), typeof(DefaultTransactionManager));
			}
		}

		private void OnNewTransaction(ITransaction transaction, TransactionMode transactionMode, IsolationMode isolationMode, bool distributedTransaction)
		{
			transaction.Enlist(new RhinoTransactionResourceAdapter(transactionMode));
		}

		private void Kernel_ComponentCreated(ComponentModel model, object instance)
		{
			if (model.Service != null && model.Service == typeof(ITransactionManager))
			{
				ITransactionManager txMgr = (ITransactionManager) instance;
				txMgr.TransactionCreated += new TransactionCreationInfoDelegate(OnNewTransaction);
			}	
		}
	}
}
