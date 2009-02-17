using NHibernate;
using Rhino.Commons;

namespace RhinoIglooSample.Test.Model
{
    public class StrategyForInterface : IFetchingStrategy<IUser>
    {
        public ICriteria Apply(ICriteria criteria)
        {
            return criteria;
        }
    }
}
