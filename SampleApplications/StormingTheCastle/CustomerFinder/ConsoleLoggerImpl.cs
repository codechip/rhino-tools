namespace CustomerFinder
{
    public class ConsoleLoggerImpl : ILogger
    {
        public void Log(string msg)
        {
            System.Console.WriteLine(msg);
        }
    }
}