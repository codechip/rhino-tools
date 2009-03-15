using System;
using MultiTenancy.Web.Context;

namespace Southsand.Web
{
    public class BootStrapper : AbstractBootStrapper
    {
        public override string TenantId
        {
            get { return "Southsand.Web"; }
        }
    }
}