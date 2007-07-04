namespace Rhino.Mocks.Demo
{
	public interface IUserRepository
	{
		User GetByUserName(string name);
	}
}