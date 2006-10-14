using System;

namespace Ayende.NHibernateQueryAnalyzer.SchemaEditing
{
	public interface IFieldReference
	{
		object Value { get; set; }

		string Name { get; }

		/// <summary>
		/// Return true if the inner value is an array with 
		/// elements or a non-null reference or is a default value.
		/// </summary>
		bool HasValue { get; }

		Type Type { get; }

		/// <summary>
		/// Whatever this is a required field.
		/// </summary>
		bool IsRequired { get; }
	}
}