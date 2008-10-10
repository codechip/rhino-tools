namespace Rhino.Testing.Tests.AutoMocking
{
	public class ComponentWithComplexProperty
	{
		private ComplexProperty complexProperty;


		public ComplexProperty ComplexProperty
		{
			get { return complexProperty; }
			set { complexProperty = value; }
		}
	}

	public class ComplexProperty
	{
		private string constructorArgument1;
		private string constructorArgument2;

        public ComplexProperty()
        {
        }

		public ComplexProperty(string constructorArgument1, string constructorArgument2)
		{
			this.constructorArgument1 = constructorArgument1;
			this.constructorArgument2 = constructorArgument2;
		}
	}
}
