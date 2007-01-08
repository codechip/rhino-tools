using System;
using Castle.Services.Transaction;
using Exesto.Model;
using Rhino.Commons;

namespace Exesto.Web.Services
{
	[Transactional]
	public class JustService
	{
		[Transaction(TransactionMode.Requires)]
		public virtual void Insert(bool shouldThrow)
		{
			Subject s = new Subject();
			s.Name = "abc";
			Repository<Subject>.Save(s);
			if (shouldThrow)
				throw new Exception("bummer");
		}
	}
}
