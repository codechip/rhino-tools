namespace Binsor.Presentation.Framework.Data
{
	using System;
    using System.Windows.Input;

	public class MenuItemData
	{
		private string header;
        string commandName;
        private ICommand command;
		private string parent;
		private string name;

        public string CommandName
        {
            get { return commandName; }
            set 
            {
                if (string.IsNullOrEmpty(name))
                    Name = value;
                commandName = value; 
            }
        }

		public string Name
		{
			get { return name; }
			set { name = value; }
		}

        public string Header
		{
			get { return header; }
			set { header = value; }
		}

        public ICommand Command
		{
			get 
            {
                if (command == null && commandName != null)
                {
                    command = Rhino.Commons.IoC.Resolve<ICommand>(commandName);
                }
                return command; 
            }
			set { command = value; }
		}

		public string Parent
		{
			get { return parent; }
			set { parent = value; }
		}
	}
}
