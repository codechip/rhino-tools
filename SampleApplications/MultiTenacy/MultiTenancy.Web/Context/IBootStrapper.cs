using System;
using Castle.Windsor;

namespace MultiTenancy.Web.Context
{
    public interface IBootStrapper
    {
        void Init(RootContext context);

        TenantContext CreateContext();
    }
}