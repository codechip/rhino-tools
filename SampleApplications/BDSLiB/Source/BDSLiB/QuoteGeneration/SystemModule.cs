namespace Chapter5.QuoteGeneration
{
    using System.Collections.Generic;

    public class SystemModule
    {
        private readonly string name;
        public List<string> Requirements = new List<string>();
        public int UsersPerMachine;
        public List<string> OnSameMachineWith = new List<string>();

        public SystemModule(string name)
        {
            this.name = name;
        }

        public string Name
        {
            get { return name; }
        }
    }
}