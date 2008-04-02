using Castle.ActiveRecord;
using Rhino.Security.Framework;

namespace Rhino.Security.AR
{
  [ActiveRecord]
  public class EntityType : NamedEntity<EntityType>, IEntityType
  {
  }
}