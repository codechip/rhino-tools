using System;
using System.Collections.Generic;
using System.Text;

namespace Chapter5.Security
{
    using System.Security.Principal;

    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                WindowsPrincipal principal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
                bool? allowed = Authorization.IsAllowed(principal, "/account/login");
                Console.WriteLine("Allowed login: {0}", allowed);
                Console.WriteLine(Authorization.WhyAllowed(principal, "/account/login"));

                Order order = new Order();
                order.TotalCost = 10000m;

                allowed = Authorization.IsAllowed(principal, "/order/approve", order);
                Console.WriteLine("Allowed login: {0}", allowed);
                Console.WriteLine(Authorization.WhyAllowed(principal, "/order/approve", order));
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e);
            }
        }
    }
}
