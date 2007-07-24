using Castle.ActiveRecord;
using Iesi.Collections.Generic;

namespace NHibernate.Query.Generator.Tests.ActiveRecord
{
    [ActiveRecord("Roles")]
    public class Role : ActiveRecordBase<Role>
    {
        private int id;
        private string name;
        private ISet<User> users = new HashedSet<User>();

        [PrimaryKey]
        public virtual int Id
        {
            get { return id; }
            set { id = value; }
        }

        [Property]
        public virtual string Name
        {
            get { return name; }
            set { name = value; }
        }

        [HasAndBelongsToMany(Table = "UsersRoles", ColumnKey = "RoleId", ColumnRef = "UserId")]
        public virtual ISet<User> Users
        {
            get { return users; }
            set { users = value; }
        }
    }
}