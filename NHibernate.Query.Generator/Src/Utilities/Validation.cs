using System;
using System.Diagnostics;
using System.Reflection;

namespace Ayende.NHibernateQueryAnalyzer.Utilities
{
	/// <summary>
	/// Summary description for ValidationUtil.
	/// </summary>
	public sealed class Validation
	{
		public static void NotNull(params object[] args)
		{
			if (args == null)
				throw new ArgumentNullException("args");
			for (int i = 0; i < args.Length; i++)
			{
				if (args[i] == null)
				{
					StackTrace st = new StackTrace();
					MethodBase callingMethod = st.GetFrame(1).GetMethod();
					ParameterInfo[] parameters = callingMethod.GetParameters();
					if (parameters.Length > i)
						throw new ArgumentNullException(parameters[i].Name);
					else
						throw new ArgumentNullException();
				}
			}

		}
	}
}