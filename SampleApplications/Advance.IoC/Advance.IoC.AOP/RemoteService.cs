using System.Threading;

namespace Advance.IoC.AOP
{
    public class RemoteService : IRemoteService
    {
        public void ShortOp()
        {
            
        }

        public int LongOp()
        {
            Thread.Sleep(1500);
            return 5;
        }
    }
}