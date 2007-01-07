namespace NHibernate.Query.Generator
{
	/// <summary>
	/// This is used to decide how the association path should be generated on association
	/// properties.
	/// </summary>
	public enum AssociationBehavior
	{
		/// <summary>
		/// no association, use nulls
		/// </summary>
		DoNotAdd,
		/// <summary>
		/// get the association from the instance name
		/// </summary>
		AddAssociationFromName,
		/// <summary>
		/// Hard code the association (this is a root property).
		/// </summary>
		AddAssociationHardCoded
	}
}
