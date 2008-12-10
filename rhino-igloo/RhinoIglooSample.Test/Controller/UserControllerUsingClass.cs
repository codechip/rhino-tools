using Rhino.Igloo;
using RhinoIglooSample.Test.Model;

namespace RhinoIglooSample.Test.Controller
{
    public class UserControllerUsingClass: BaseController
    {
        private bool found;

        public UserControllerUsingClass(IContext context)
            : base(context)
        {
        }

        public bool FoundUser
        {
            get { return found; }
        }

        [InjectEntity]
        public User UserUsingGet
        {
            set { found = true; }
        }

        [InjectEntity]
        public User UserUsingFetchingStrategy
        {
            set { found = true; }
        }
    }
}
