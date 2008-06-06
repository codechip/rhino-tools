using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using NMemcached.Commands;
using NMemcached.Commands.Misc;
using NMemcached.Commands.Modifications;
using NMemcached.Commands.Retrieval;
using NMemcached.Commands.Storage;
using NMemcached.Extensions;
using NMemcached.Util;

namespace NMemcached
{
	public class ClientConnection : IDisposable
	{
		private readonly TcpClient client;
		private readonly Stream stream;
		private readonly Action onDispose;
		private readonly StreamWriter writer;

		public ClientConnection(TcpClient client, Stream stream, Action onDispose)
		{
			commandFactories = new Dictionary<string, Func<Stream, ICommand>>
			                   	{
			                   		//storage
			                   		{"add", s => new AddCommand(s)},
			                   		{"append", s => new AppendCommand(s)},
			                   		{"cas", s => new CasCommand(s)},
			                   		{"delete", s => new DeleteCommand(s)},
			                   		{"prepend", s => new PrependCommand(s)},
			                   		{"replace", s => new ReplaceCommand(s)},
			                   		{"set", s => new SetCommand(s)},

			                   		// retrieval
			                   		{"get", s => new GetCommand(s)},
			                   		{"gets", s => new GetsCommand(s)},

			                   		//modifications
			                   		{"incr", s => new IncrCommand(s)},
			                   		{"decr", s => new DecrCommand(s)},

			                   		//misc
			                   		{"flush_all", s => new FlushAllCommand(s)},
			                   		{"quit", s => new QuitCommand(s, Dispose)},
			                   		{"version", s => new VersionCommand(s)},
			                   	};
			this.client = client;
			this.stream = stream;
			this.onDispose = onDispose;

			writer = new StreamWriter(stream);
		}

		private readonly IDictionary<string, Func<Stream, ICommand>> commandFactories;

		public void ProcessNextCommand()
		{
			HandleDisconnectionExceptions(
				() => new LineReader(stream).Read(ExecuteCommand));
			
		}

		private void HandleDisconnectionExceptions(Action action)
		{
			try
			{
				action();
			}
			catch (IOException)
			{
				Dispose();
			}
			catch(SocketException)
			{
				Dispose();
			}
			catch(ObjectDisposedException)
			{
				Dispose();
			}
		}

		private void ExecuteCommand(string commandLine, Exception e)
		{
			HandleDisconnectionExceptions(delegate
			{
				if (e != null)
				{
					SendServerError(e);
					ProcessNextCommand();
				}
				if (commandLine.IsEmpty())
				{
					SendError();
					ProcessNextCommand();
					return;
				}
				string[] arguments = commandLine.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
				if (arguments.Length == 0)
				{
					SendError();
					ProcessNextCommand();
					return;
				}
				string commandName = arguments[0];

				Func<Stream, ICommand> factory;
				if (commandFactories.TryGetValue(commandName, out factory) == false)
				{
					SendError();
					ProcessNextCommand();
					return;
				}

				ICommand command = factory(stream);
				var commandArguments = new string[arguments.Length - 1];
				Array.Copy(arguments, 1, commandArguments, 0, arguments.Length - 1);

				if (command.Init(commandArguments) == false)
				{
					ProcessNextCommand();
					return;
				}

				command.FinishedExecuting += ProcessNextCommand;

				command.Execute();
			});
		}

		private void SendServerError(Exception e)
		{
			try
			{
				writer.WriteLine("SERVER_ERROR " + e.Message);
				writer.Flush();
			}
			catch 
			{
				// no much we can do here
			}
		}

		private void SendError()
		{
			try
			{
				writer.WriteLine("ERROR");
				writer.Flush();
			}
			catch
			{
				// no much we can do here
			}
		}

		public void Dispose()
		{
			writer.Dispose();
			stream.Dispose();
			client.Close();
			onDispose();
			GC.SuppressFinalize(this);
		}

		~ClientConnection()
		{
			Dispose();
		}
	}
}