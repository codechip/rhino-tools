namespace Rhino.DistributedHashTable
{
	using System;
	using Castle.MicroKernel.SubSystems.Conversion;

	[Convertible]
	public class Node
	{
		public string Name { get; set; }
		public NodeUri Primary { get; set; }
		public NodeUri Secondary { get; set; }
		public NodeUri Tertiary { get; set; }

		public void ExecuteSync(Action<Uri, Uri> action)
		{
			try
			{
				action(Primary.Sync, Primary.Sync);
			}
			catch (Exception)
			{
				if (Secondary==null)
					throw;
				try
				{
					action(Secondary.Sync, Primary.Sync);
				}
				catch (Exception)
				{
					if (Tertiary==null)
						throw;
					action(Tertiary.Sync, Primary.Sync);
				}
			}
		}
	}
}