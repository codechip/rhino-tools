using System;
using System.IO;
using System.Xml;
using NHibernate;
using NHibernate.Cfg;
using Settings=Rhino.Commons.Properties.Settings;

namespace Rhino.Commons
{
	public static class UnitOfWork
	{
		public const string CurrentUnitOfWorkKey = "CurrentUnitOfWork.Key";

        private static IUnitOfWork globalNonThreadSafeUnitOfwork;
	    
	    /// <summary>
	    /// Mostly intended to support mocking of the unit of work.
	    /// NOT thread safe!
	    /// </summary>
	    public static void RegisterGlobalUnitOfWork(IUnitOfWork global)
	    {
            globalNonThreadSafeUnitOfwork = global;
	    }
	    
		public static IUnitOfWork Start()
		{
			return Start(UnitOfWorkNestingOptions.ReturnExistingOrCreateUnitOfWork);
		}
		
		/// <summary>
		/// Start a Unit of Work
		/// is called
		/// </summary>
		/// <returns>
		/// An IUnitOfwork object that can be used to work with the current UoW.
		/// </returns>
		public static IUnitOfWork Start(UnitOfWorkNestingOptions nestingOptions)
		{
            if (globalNonThreadSafeUnitOfwork != null)
                return globalNonThreadSafeUnitOfwork;
			IUnitOfWorkImplementor existing = (IUnitOfWorkImplementor)Local.Data[CurrentUnitOfWorkKey];
			if(nestingOptions == UnitOfWorkNestingOptions.ReturnExistingOrCreateUnitOfWork && 
			   existing != null)
			{
				existing.IncremementUsages();
				return existing;
			}
			IUnitOfWorkImplementor unitOfWorkImplementor = IoC.Resolve<IUnitOfWorkFactory>().Create(existing);
			Local.Data[CurrentUnitOfWorkKey] = unitOfWorkImplementor;
			return existing;
		}

	    /// <summary>
		/// The current unit of work.
		/// </summary>
		public static IUnitOfWork Current
		{
			get
			{
				IUnitOfWork unitOfWork = (IUnitOfWork)Local.Data[CurrentUnitOfWorkKey];
				if (unitOfWork == null)
					throw new InvalidOperationException("You are not in a unit of work");
				return unitOfWork;
			}
		}

		/// <summary>
		/// Called internally to clear the current UoW and move to the previous one.
		/// </summary>
		internal static void DisposeUnitOfWork(IUnitOfWorkImplementor unitOfWork)
		{
			IUnitOfWorkImplementor previous = unitOfWork.Previous;
			Local.Data[CurrentUnitOfWorkKey] = previous;
		}
	}
}
