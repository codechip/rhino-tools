using System.IO;
using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;
using Rhino.ServiceBus.Impl;
using Rhino.ServiceBus.Internal;
using Xunit;

namespace Rhino.ServiceBus.Tests
{
    public class When_Security_Is_Not_Specified_In_Config
    {
        private static IWindsorContainer CreateContainer()
        {
            var container = new WindsorContainer(new XmlInterpreter());
            container.Kernel.AddFacility("rhino.esb", new RhinoServiceBusFacility());
            return container;
        }

        [Fact]
        public void Will_not_encrypt_wire_encrypted_string()
        {
            var container = CreateContainer();
            var serializer = container.Resolve<IMessageSerializer>();
            var memoryStream = new MemoryStream();
            serializer.Serialize(new[]
            {
                new When_Security_Is_Specified_In_Config.ClassWithSecretField {ShouldBeEncrypted = "abc"}
            }, memoryStream);
            memoryStream.Position = 0;
            var msg = new StreamReader(memoryStream).ReadToEnd();
            Assert.Equal(@"<?xml version=""1.0"" encoding=""utf-8""?>
<esb:messages xmlns:esb=""http://servicebus.hibernatingrhinos.com/2008/12/20/esb"" xmlns:tests.classwithsecretfield=""Rhino.ServiceBus.Tests.When_Security_Is_Specified_In_Config+ClassWithSecretField, Rhino.ServiceBus.Tests"" xmlns:datastructures.wireecryptedstring=""Rhino.ServiceBus.DataStructures.WireEcryptedString, Rhino.ServiceBus"" xmlns:string=""string"">
  <tests.classwithsecretfield:ClassWithSecretField>
    <datastructures.wireecryptedstring:ShouldBeEncrypted>
      <string:Value>abc</string:Value>
    </datastructures.wireecryptedstring:ShouldBeEncrypted>
  </tests.classwithsecretfield:ClassWithSecretField>
</esb:messages>", msg);
        }

        [Fact]
        public void Will_not_be_able_to_read_encypted_content()
        {
            var container = CreateContainer();
            var serializer = container.Resolve<IMessageSerializer>();
            var memoryStream = new MemoryStream();
            var writer = new StreamWriter(memoryStream);
            writer.Write(When_Security_Is_Specified_In_Config.encryptedMessage);
            writer.Flush();
            memoryStream.Position = 0;

            var msg = (When_Security_Is_Specified_In_Config.ClassWithSecretField)serializer.Deserialize(memoryStream)[0];

            Assert.Null(msg.ShouldBeEncrypted.Value);
        }
    }
}