using System;
using System.Collections.Generic;
using System.Text;
using BookStore.UI;

namespace BookStore.CmdUI
{

    public class ConsoleSelectOptionsView : CommonConsoleView, ISelectOptionsView
    {
        IList<string> commandNames = new List<string>();
        IDictionary<string, Command> commandNameToCommand = new Dictionary<string, Command>();

        public void AddCommand(string commandName, Command cmd)
        {
            commandNames.Add(commandName);
            commandNameToCommand[commandName] = cmd;
        }

        public void Display()
        {
            while (true)
            {
                for (int i = 0; i < commandNames.Count; i++)
                {
                    Console.WriteLine("{0}) {1}", i + 1, commandNames[i]);
                }
                Console.WriteLine();
                Console.WriteLine("0) Quit");

                string input = Console.ReadLine();
                int index;
                if (int.TryParse(input, out index) == false)
                {
                    Console.WriteLine("You must enter numeric value");
                    continue;
                }
                index -= 1;//move it to 0 based indexing.

                if (index == -1)
                    break;

                if (index < 0 || index >= commandNames.Count)
                {
                    Console.WriteLine("You must enter numeric value between: 0 - {0}",commandNames.Count);
                    continue;
                }
                string cmdName = commandNames[index];
                Command cmd = commandNameToCommand[cmdName];

                Console.Clear();
             
                cmd();
            }
        }
    }
}
