using System;
using System.Runtime.Serialization;

namespace Rhino.Testing.AutoMocking
{
  /// <summary>An exception indicating that a feature is not available in AAA mode and the classic Record/Playback mode must be used.</summary>
  public class NotAvailableInAaaModeException : NotImplementedException
  {
    public NotAvailableInAaaModeException() : this("This feature is not available in AAA, trying using Record/Playback mode with an instance of a MockRepository.")
    {
    }

    public NotAvailableInAaaModeException(string message) : base(message)
    {
    }

    public NotAvailableInAaaModeException(string message, Exception inner) : base(message, inner)
    {
    }

    protected NotAvailableInAaaModeException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
  }
}