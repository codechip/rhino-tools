using System;
using Advance.IoC.Components.Services;

namespace Advance.IoC.Components.Controllers
{
    public class ForgotMyPassController : IController
    {
        private ICrowdParticipation crowdParticipation;

        public ForgotMyPassController(ICrowdParticipation crowdParticipation)
        {
            this.crowdParticipation = crowdParticipation;
        }

        public void Execute()
        {
            crowdParticipation.Participate();
            Console.WriteLine("Forgot my password called");
        }
    }
}