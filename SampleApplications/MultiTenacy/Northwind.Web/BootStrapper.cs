using System;
using MultiTenancy.Web.Context;

namespace Northwind.Web
{
    public class BootStrapper : AbstractBootStrapper
    {
        public override string TenantId
        {
            get { return "Northwind.Web"; }
        }
    }
}