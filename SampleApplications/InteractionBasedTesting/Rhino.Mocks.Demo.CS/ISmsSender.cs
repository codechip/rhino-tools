namespace Rhino.Mocks.Demo
{
	public interface ISmsSender
	{
		void SendSMS(string phone, string message);
	}
}