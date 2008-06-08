using System;
using System.IO;

namespace NMemcached.Commands
{
	public interface ICommand
	{
		string Name { get; }
		void SetContext(Stream stream);
		bool Init(params string[] args);
		void Execute();

		event Action FinishedExecuting;
	}
}