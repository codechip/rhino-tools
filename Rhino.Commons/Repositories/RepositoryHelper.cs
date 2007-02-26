using System.Data;
using NHibernate;
using NHibernate.Expression;

namespace Rhino.Commons
{
	internal class RepositoryHelper<T>
	{
		public static void AddCaching(IQuery query)
		{
			if (With.Caching.ShouldForceCacheRefresh == false && With.Caching.Enabled)
			{
				query.SetCacheable(true);
				if (With.Caching.CurrentCacheRegion != null)
					query.SetCacheRegion(With.Caching.CurrentCacheRegion);
			}
			else if (With.Caching.ShouldForceCacheRefresh)
			{
				query.SetForceCacheRefresh(true);
			}
		}

		internal static IQuery CreateQuery(ISession session, string namedQuery, Parameter[] parameters)
		{
			IQuery query = session.GetNamedQuery(namedQuery);
			foreach (Parameter parameter in parameters)
			{
				if (parameter.Type == null)
					query.SetParameter(parameter.Name, parameter.Value);
				else
					query.SetParameter(parameter.Name, parameter.Value, parameter.Type);
			}
			AddCaching(query);
			return query;
		}

		public static ICriteria GetExecutableCriteria(ISession session, DetachedCriteria criteria, Order[] orders)
		{
			ICriteria executableCriteria;
			if (criteria != null)
			{
				executableCriteria = criteria.GetExecutableCriteria(session);
			}
			else
			{
				executableCriteria = session.CreateCriteria(typeof (T));
			}

			AddCaching(executableCriteria);
			if (orders != null)
			{
				foreach (Order order in orders)
				{
					executableCriteria.AddOrder(order);
				}
			}
			return executableCriteria;
		}

		public static void AddCaching(ICriteria crit)
		{
			if (With.Caching.ShouldForceCacheRefresh == false &&
			    With.Caching.Enabled)
			{
				crit.SetCacheable(true);
				if (With.Caching.CurrentCacheRegion != null)
					crit.SetCacheRegion(With.Caching.CurrentCacheRegion);
			}
		}

		public static ICriteria CreateCriteriaFromArray(ISession session, ICriterion[] criteria)
		{
			ICriteria crit = session.CreateCriteria(typeof (T));
			foreach (ICriterion criterion in criteria)
			{
				//allow some fancy antics like returning possible return 
				// or null to ignore the criteria
				if (criterion == null)
					continue;
				crit.Add(criterion);
			}
			AddCaching(crit);
			return crit;
		}

		public static void CreateDbDataParameters(IDbCommand command, Parameter[] parameters)
		{
			foreach (Parameter parameter in parameters)
			{
				IDbDataParameter sp_arg = command.CreateParameter();
				sp_arg.ParameterName = parameter.Name;
				sp_arg.Value = parameter.Value;
				command.Parameters.Add(sp_arg);
			}
		}
	}
}