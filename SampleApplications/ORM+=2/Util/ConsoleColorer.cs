using System;

namespace Util
{
    public class ConsoleColorer : IDisposable
    {
        private System.ConsoleColor old;


        private static ConsoleColor GetColorForName(string name)
        {
            Array values = Enum.GetValues(typeof(ConsoleColor));
            return (ConsoleColor)values.GetValue(Math.Abs(name.GetHashCode()) % values.Length);
        }

        public ConsoleColorer(string name)
            : this(GetColorForName(name))
        {
        }

        public ConsoleColorer(ConsoleColor @new)
        {
            this.old = Console.ForegroundColor;
            //Console.ForegroundColor = @new;
        }

        public void Dispose()
        {
            Console.ForegroundColor = old;
        }
    }
}