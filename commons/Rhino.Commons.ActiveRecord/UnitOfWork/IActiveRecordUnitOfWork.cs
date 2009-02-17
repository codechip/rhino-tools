using Castle.ActiveRecord;

namespace Rhino.Commons
{
    public interface IActiveRecordUnitOfWork : IUnitOfWorkImplementor
    {
        ISessionScope Scope { get; }
    }
}
