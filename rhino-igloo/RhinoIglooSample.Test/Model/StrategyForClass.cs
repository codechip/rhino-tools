using NHibernate;
using Rhino.Commons;

namespace RhinoIglooSample.Test.Model
{
    public class StrategyForClass : IFetchingStrategy<User>
    {
        public ICriteria Apply(ICriteria criteria)
        {
            return criteria;
        }
    }
}
