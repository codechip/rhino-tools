namespace Rhino.DivanDB
{
	using System;

	public class DocumentId
	{
		public DocumentId()
		{
			Id = null;
			Version = 1;
		}

		public Guid? Id { get; set;}
		public int Version { get; set; }
	}
}