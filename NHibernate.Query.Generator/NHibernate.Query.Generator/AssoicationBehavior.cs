namespace NHibernate.Query.Generator
{
	/// <summary>
	/// This is used to decide how the assoication path should be generated on assoication
	/// properties.
	/// </summary>
	public enum AssoicationBehavior
	{
		/// <summary>
		/// no assoication, use nulls
		/// </summary>
		DoNotAdd,
		/// <summary>
		/// get the assoication from the instance name
		/// </summary>
		AddAssoicationFromName,
		/// <summary>
		/// Hard code the assoication (this is a root property).
		/// </summary>
		AddAssoicationHardCoded
	}
}