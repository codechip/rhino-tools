namespace Binsor.Presentation.Framework.Attributes
{
	using System;

	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class SkipAutomaticRegistrationAttribute : Attribute
	{
	}
}