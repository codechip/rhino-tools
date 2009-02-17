using System;
using System.Data.OleDb;
using MbUnit.Framework;
using Rhino.Commons;

namespace Rhino.Commons.Test.UoW
{
    [TestFixture]
    public class MultipleNHibernateUnitOfWorkFactoryTests
    {
        [Test, ExpectedException(typeof(NotSupportedException), MultipleNHibernateUnitOfWorkFactory.USER_PROVIDED_CONNECTION_EXCEPTION_MESSAGE)]
        public void Should_not_be_able_to_supply_a_user_defined_database_connection() 
        {
            CreateSUT().Create(new OleDbConnection(), null);
        }

        private IUnitOfWorkFactory CreateSUT()
        {
            return new MultipleNHibernateUnitOfWorkFactory();
        }
    }
}
