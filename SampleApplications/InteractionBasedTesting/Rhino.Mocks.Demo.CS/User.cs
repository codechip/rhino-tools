namespace Rhino.Mocks.Demo
{
	public class User
	{
		private readonly string name;
		private readonly string phone;

		public string Name
		{
			get { return name; }
		}

		public string Phone
		{
			get { return phone; }
		}

		public User(string name, string phone)
		{
			this.name = name;
			this.phone = phone;
		}
	}
}