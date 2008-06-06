using System;
using System.IO;

namespace NMemcached.Commands
{
	public interface ICommand
	{
		string Name { get; }
		bool Init(params string[] args);
		void Execute();

		event Action FinishedExecuting;
	}
}