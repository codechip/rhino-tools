using System;
using Rhino.ServiceBus.Impl;
using Rhino.ServiceBus.Internal;
using Xunit;

namespace Rhino.ServiceBus.Tests
{
    public class DefaultReflectionTests
    {
        private readonly IReflection reflection = new DefaultReflection();

        [Fact]
        public void Can_roundtrip_uri()
        {
            var typeName = reflection.GetAssemblyQualifiedNameWithoutVersion(new Uri("http://ayende.com"));
            var type = reflection.GetType(typeName);
            Assert.NotNull(type);
        }
    }
}