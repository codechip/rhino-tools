namespace Rhino.DivanDB.DSL
{
	using Rhino.DSL;

	public static class Factory
	{
		private static DslFactory factory;

		public static DslFactory Instance
		{
			get
			{
				if(factory==null)
				{
					factory = new DslFactory();
					factory.Register<IndexerBase>(new IndexerDslEngine());
				}
				return factory;
			}
		}
	}
}