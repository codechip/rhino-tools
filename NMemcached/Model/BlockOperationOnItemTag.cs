using System;

namespace NMemcached.Model
{
	public class BlockOperationOnItemTag
	{
		private DateTime Value { get; set; }

		public BlockOperationOnItemTag(DateTime value)
		{
			Value = value;
		}
	}
}