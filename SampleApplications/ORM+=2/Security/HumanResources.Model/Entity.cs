using System;

namespace HumanResources.Model
{
    public class Entity
    {
        public Entity()
        {
            SecurityKey = Guid.NewGuid();
        }

        public virtual long Id { get; set; }
        public virtual Guid SecurityKey { get; set; }
    }
}