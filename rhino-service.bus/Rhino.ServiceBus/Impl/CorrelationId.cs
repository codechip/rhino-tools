using System;

namespace Rhino.ServiceBus.Impl
{
    public class CorrelationId
    {
        public static CorrelationId Empty = new CorrelationId
        {
            Id = Guid.Empty,
            Version = 0
        };

        public Guid Id { get; set; }
        public int Version { get; set; }

        public override string ToString()
        {
            return Id + "\\" + Version;
        }

        public CorrelationId Increment()
        {
            return new CorrelationId()
            {
                Id = Id,
                Version = Version + 1
            };
        }

        public static CorrelationId New()
        {
            return new CorrelationId
            {
                Id = Guid.NewGuid(),
                Version = 1
            };
        }

        public static CorrelationId Parse(string correlationId)
        {
            if(string.IsNullOrEmpty(correlationId))
                return null;

            var parts = correlationId.Split('\\');
            try
            {
                return new CorrelationId
                {
                    Id = new Guid(parts[0]),
                    Version = int.Parse(parts[1])
                };
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Could not parse correlation id: " + correlationId, e);
            }
        }

        public static implicit operator string(CorrelationId id)
        {
            if (id == null)
                return null;
            return id.ToString();
        }
    }
}