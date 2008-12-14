using System.IO;

namespace Rhino.ServiceBus.Impl
{
    public interface ITransportMessage
    {
        Stream BodyStream { get; set; }
    }
}