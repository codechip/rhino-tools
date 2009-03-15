using System;
using MultiTenancy.Web.Context;

namespace Eastsea.Web
{
    public class BootStrapper : AbstractBootStrapper
    {
        public override string TenantId
        {
            get { return "Eastsea.Web"; }
        }
    }
}