using System;
using Castle.ActiveRecord;
using Castle.Components.Validator;

namespace Rhino.Security.AR
{
  /// <summary>
  /// An entity with a name
  /// </summary>
  public class NamedEntity<T> : Rhino.Security.Model.NamedEntity<T>
    where T : NamedEntity<T>
  {
    [PrimaryKey(Generator = PrimaryKeyType.GuidComb)]
    public override Guid Id
    {
      get { return base.Id; }
      set { base.Id = value; }
    }

    /// <summary>
    /// Gets or sets the name of this entity.
    /// </summary>
    /// <value>The name.</value>
    /// <remarks>
    /// The name can be set only on creation, and is not changed
    /// afterward.
    /// </remarks>
    [Property(NotNull = true, Length = 255, Update = false, Unique = true)]
    [ValidateNonEmpty, ValidateLength(0, 255), ValidateIsUniqueInSameTransaction]
    public override string Name
    {
      get { return base.Name; }
      set { base.Name = value; }
    }

    /// <summary>
    /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
    /// </returns>
    public override string ToString()
    {
      return GetType().Name + ": " + Name;
    }
  }
}