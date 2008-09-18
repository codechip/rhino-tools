using MbUnit.Framework;
using Rhino.Testing.Utility;
using Rhino.Testing.Tests.AutoMocking;
using Rhino.Mocks;
using System;

namespace Rhino.Testing.Tests.Utility
{
    [TestFixture]
    public class BaseTestTests : BaseTest
    {
        [SetUp]
        public void BeforeEachTest()
        {
            InitializeContainer();
        }

        [TearDown]
        public void AfterEachTest()
        {
            DestroyContainer();
        }

        [Test]
        public void ShouldCreateSystemUnderTest()
        {
            ComponentBeingConfigured sut = CreateSUT<ComponentBeingConfigured>();
            Assert.IsNotNull(sut);
        }

        [Test]
        public void ShouldRecordAndPlayback()
        {
            ComponentBeingConfigured sut = CreateSUT<ComponentBeingConfigured>();
            using (Record)
            {
                Expect.Call(Mock<ICollectionOfServices>().SomethingToDispose).Return(Mock<IDisposable>());
                Expect.Call(delegate { Mock<IDisposable>().Dispose(); });
            }

            using (Playback)
            {
                sut.RunDispose();
            }
            
        }

        [Test]
        [ExpectedException(typeof(Exception))]
        public void ShouldReplayAllAndReturnPlayback()
        {
            ComponentBeingConfigured sut = CreateSUT<ComponentBeingConfigured>();

            using (PlaybackOnly)
            {
                sut.RunDispose();
            }

        }
    }
}
