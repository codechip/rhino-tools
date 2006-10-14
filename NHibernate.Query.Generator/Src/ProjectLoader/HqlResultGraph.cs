using System;
using System.Collections;
using Ayende.NHibernateQueryAnalyzer.Utilities;
using Iesi.Collections;
using NHibernate;

namespace Ayende.NHibernateQueryAnalyzer.ProjectLoader
{
	/// <summary>
	/// Summary description for HqlResultGraph.
	/// </summary>
	public class HqlResultGraph : MarshalByRefObject, IDisposable
	{
		private IList graph;
		private ISession session;
		private IList remoteGraph;

		public HqlResultGraph(IList graph, ISession session)
		{
			this.graph = graph;
			this.session = session;
		}

		public IList Graph
		{
			get { return graph; }
		}

		public bool IsSessionOpen
		{
			get { return session != null & session.IsOpen; }
		}

		public IList RemoteGraph
		{
			get
			{
				if (remoteGraph == null)
				{
					remoteGraph = new ArrayList();
					foreach (object o in graph)
					{
						InitializeCollections(o);
						remoteGraph.Add(RemoteObject.Create(o));
					}
				}
				return remoteGraph;
			}
		}

		private void InitializeCollections(object obj)
		{
			if (obj != null)
			{
				foreach (object val in ReflectionUtil.GetPropertiesDictionary(obj).Values)
				{
					if(val is IList || val is IDictionary || val is ISet)
					{
						if (!NHibernateUtil.IsInitialized(val))
						{
							session.Lock(obj, LockMode.None);
							NHibernateUtil.Initialize(val);
						}
					}
				}
			}
		}

		public void Dispose()
		{
			if (IsSessionOpen)
				session.Dispose();
		}
	}
}