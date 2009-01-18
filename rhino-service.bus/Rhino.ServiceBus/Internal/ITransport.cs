using System;
using System.IO;
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

        event Action<CurrentMessageInformation> MessageSent;

        event Func<CurrentMessageInformation,bool> AdministrativeMessageArrived;
        
        event Action<CurrentMessageInformation> MessageArrived;

        event Action<CurrentMessageInformation, Exception> MessageSerializationException;
        
        event Action<CurrentMessageInformation, Exception> MessageProcessingFailure;

        event Action<CurrentMessageInformation, Exception> MessageProcessingCompleted;

        event Action<CurrentMessageInformation, Exception> AdministrativeMessageProcessingCompleted;

        void Discard(object msg);

    	void Send(Uri uri, DateTime processAgainAt, object[] msgs);

      }
}