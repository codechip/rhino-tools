namespace Rhino.Queues.Commands
{
	public interface ICommandFactory
	{
		ICommand CreateCommand(SingleDestinationMessageBatch batch);
	}
}