using Rhino.Igloo;
using RhinoIglooSample.Test.Model;

namespace RhinoIglooSample.Test.Controller
{
    public class UserControllerUsingInterface : BaseController
    {
        bool found;

        public UserControllerUsingInterface(IContext context) : base(context)
        {
        }

        public bool FoundUser
        {
            get { return found; }
        }

        [InjectEntity]
        public IUser UserUsingGet
        {
            set { found = true; }
        }

        [InjectEntity]
        public IUser UserUsingFetchingStrategy
        {
            set { found = true; }
        }
    }
}
