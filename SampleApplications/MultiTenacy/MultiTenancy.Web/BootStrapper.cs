using System;
using MultiTenancy.Web.Context;

namespace MultiTenancy.Web
{
    public class BootStrapper : AbstractBootStrapper
    {
        public override string TenantId
        {
            get { return "MultiTenancy.Web"; }
        }
    }
}