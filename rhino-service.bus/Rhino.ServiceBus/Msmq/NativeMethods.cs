using System;
using System.Runtime.InteropServices;
using System.Transactions;

namespace Rhino.ServiceBus.Msmq
{
    public static class NativeMethods
    {
        public const int MQ_MOVE_ACCESS = 4;
        public const int MQ_DENY_NONE = 0;

        [DllImport("mqrt.dll", CharSet = CharSet.Unicode)]
        public static extern int MQOpenQueue(string formatName, int access, int shareMode, ref IntPtr hQueue);

        [DllImport("mqrt.dll")]
        public static extern int MQCloseQueue(IntPtr queue);

        [DllImport("mqrt.dll")]
        public static extern int MQMoveMessage(IntPtr sourceQueue, IntPtr targetQueue, long lookupID, IDtcTransaction transaction);
    }
}