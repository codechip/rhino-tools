namespace Rhino.Queues.Storage.Disk
{
	public class Operation
	{
		public Operation(OperationType type, int fileNumber, int start, int length, byte[] data)
		{
			Data = data;
			Type = type;
			FileNumber = fileNumber;
			Start = start;
			Length = length;
		}

		public byte[] Data { get; set; }
		public OperationType Type { get; set; }
		public int FileNumber { get; set; }
		public int Start { get; set; }
		public int Length { get; set; }
	}
}