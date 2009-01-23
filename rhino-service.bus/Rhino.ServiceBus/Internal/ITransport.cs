using System;
using System.IO;
using Rhino.ServiceBus.Impl;

namespace Rhino.ServiceBus.Internal
{
    public interface ITransport : IDisposable
    {
        void Start();

        Uri Endpoint { get; }

        void Send(Uri uri, params object[] msgs);

        void Reply(params object[] messages);

        event Action<CurrentMessageInformation> MessageSent;

        event Func<CurrentMessageInformation,bool> AdministrativeMessageArrived;
        
        event Func<CurrentMessageInformation, bool> MessageArrived;

        event Action<CurrentMessageInformation, Exception> MessageSerializationException;
        
        event Action<CurrentMessageInformation, Exception> MessageProcessingFailure;

        event Action<CurrentMessageInformation, Exception> MessageProcessingCompleted;

        event Action<CurrentMessageInformation, Exception> AdministrativeMessageProcessingCompleted;

    	void Send(Uri uri, DateTime processAgainAt, object[] msgs);

      }
}