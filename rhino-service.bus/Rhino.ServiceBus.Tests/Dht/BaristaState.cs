using Rhino.ServiceBus.Sagas;

namespace Rhino.ServiceBus.Tests.Dht
{
    public class BaristaState : IVersionedSagaState
    {
        public bool DrinkIsReady { get; set; }

        public bool GotPayment { get; set; }

        public string Drink { get; set; }

        #region IVersionedSagaState Members

        public int Version { get; set; }

        public int[] ParentVersions { get; set; }

        #endregion
    }
}