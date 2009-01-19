namespace Rhino.ServiceBus.Sagas
{
    public interface ISagaStateMerger<TState>
        where TState : IVersionedSagaState
    {
        TState Merge(TState[] states);
    }
}