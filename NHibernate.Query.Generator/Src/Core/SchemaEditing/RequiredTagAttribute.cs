using System;

namespace Ayende.NHibernateQueryAnalyzer.SchemaEditing
{
	/// <summary>
	/// Indicate that this element is a required one
	/// </summary>
	[AttributeUsage(AttributeTargets.Field, AllowMultiple=false)]
	public class RequiredTagAttribute : Attribute
	{
		public int MinimumAmount = 1;

	}
}