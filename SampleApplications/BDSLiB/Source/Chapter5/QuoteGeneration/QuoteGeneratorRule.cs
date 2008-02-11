namespace Chapter5.QuoteGeneration
{
    using System.Collections.Generic;

    public abstract class QuoteGeneratorRule
    {
        private RequirementsInformation information;

        protected QuoteGeneratorRule(RequirementsInformation information)
        {
            this.information = information;
        }

        public List<SystemModule> Modules = new List<SystemModule>();
        private SystemModule currentModule;
        public delegate void Action();

        public void specification(string moduleName, Action action)
        {
            currentModule = new SystemModule(moduleName);
            Modules.Add(currentModule);
            action();    
        }

        public void requires(string moduleName)
        {
            currentModule.Requirements.Add(moduleName);
        }

        public void users_per_machine(int count)
        {
            currentModule.UsersPerMachine = count;
        }

        public void same_machine_as(string moduleName)
        {
            currentModule.OnSameMachineWith.Add(moduleName);
        }

        public abstract void Evaluate();
    }
}