namespace Rhino.ServiceBus.Sagas
{
    public interface IVersionedSagaState
    {
        int Version { get; set; }
        int[] ParentVersions { get; set; }
    }
}