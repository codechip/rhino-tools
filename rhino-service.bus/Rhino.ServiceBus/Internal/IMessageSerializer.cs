using System.IO;
using Rhino.ServiceBus.Impl;

namespace Rhino.ServiceBus.Internal
{
    public interface IMessageSerializer
    {
        void Serialize(object[] obj, ITransportMessage message);

        object[] Deserialize(ITransportMessage message);
    }
}