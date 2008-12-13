using System;
using Advance.IoC.Components.Services;

namespace Advance.IoC.Components.Controllers
{
    public class LoginController : IController
    {
        private readonly IAuthenticationService authenticationService;
        private readonly IStatisticsTracking statisticsTracking;

        public LoginController(IAuthenticationService authenticationService, IStatisticsTracking statisticsTracking)
        {
            this.authenticationService = authenticationService;
            this.statisticsTracking = statisticsTracking;
        }

        public void Execute()
        {
            bool loginSuccessul = false;
            while(loginSuccessul==false)
            {
                Console.Write("Enter username: ");
                string user = Console.ReadLine();
                Console.Write("Enter pass: ");
                string pass = Console.ReadLine();

                loginSuccessul = authenticationService.IsValidLogin(user, pass);
                if(loginSuccessul)
                {
                    statisticsTracking.LoginSuccessful();
                }
                else
                {
                    statisticsTracking.LoginFailed();
                }
            }
        }
    }
}