using System;
using Castle.ActiveRecord;

namespace Rhino.Security.AR
{
  /// <summary>
  /// This is a trivial class that is used to make sure that Equals and GetHashCode
  /// are properly overloaded with the correct semantics. This is exteremely important
  /// if you are going to deal with objects outside the current Unit of Work.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public abstract class EqualityAndHashCodeProvider<T> : Rhino.Security.Model.EqualityAndHashCodeProvider<T>
    where T : EqualityAndHashCodeProvider<T>
  {

    /// <summary>
    /// Gets or sets the id of this entity
    /// </summary>
    /// <value>The id.</value>
    [PrimaryKey(Generator = PrimaryKeyType.GuidComb)]
    public override Guid Id
    {
      get { return base.Id; }
      set { base.Id = value; }
    }
  }
}