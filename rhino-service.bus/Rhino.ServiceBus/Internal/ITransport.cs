using System;
using Rhino.ServiceBus.Impl;

namespace Rhino.ServiceBus.Internal
{
    public interface ITransport
    {
        void Start();

        void Stop();

        Uri Endpoint { get; }

        void Send(Uri uri, params object[] msgs);

        void Reply(params object[] messages);

        event Func<CurrentMessageInformation,DesiredMessageActionFromTransport> AdministrativeMessageArrived;
        event Action<CurrentMessageInformation> MessageArrived;
        event Action<CurrentMessageInformation, Exception> MessageProcessingFailure;
        event Action<CurrentMessageInformation> MessageProcessingCompleted;
    }
}