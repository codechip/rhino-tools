using System.Collections;
using Ayende.NHibernateQueryAnalyzer.ProjectLoader;
using NUnit.Framework;

namespace Ayende.NHibernateQueryAnalyzer.Tests.Remote
{
	/// <summary>
	/// Summary description for HqlResultGraphTests.
	/// </summary>
	[TestFixture]
	public class HqlResultGraphTests
	{
		[Test]
		public void RemoteGraph()
		{
			ArrayList list = new ArrayList();
			list.AddRange(new string[] {"First", "Second", "Third"});
			HqlResultGraph hrg = new HqlResultGraph(list, null);
			IList remote = hrg.RemoteGraph;
			Assert.AreEqual(list[0], ((RemoteObject) remote[0]).Value);
			Assert.AreEqual(list[1], ((RemoteObject) remote[1]).Value);
			Assert.AreEqual(list[2], ((RemoteObject) remote[2]).Value);
		}
	}
}