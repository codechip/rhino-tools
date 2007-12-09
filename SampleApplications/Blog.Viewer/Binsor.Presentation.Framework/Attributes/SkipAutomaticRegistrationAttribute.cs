namespace Binsor.Presentation.Framework.Attributes
{
	using System;

	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class SkipAutomaticRegistrationAttribute : Attribute
	{
		public SkipAutomaticRegistrationAttribute()
		{

		}

		public SkipAutomaticRegistrationAttribute(string whySkipRegistration)
		{

		}
	}
}