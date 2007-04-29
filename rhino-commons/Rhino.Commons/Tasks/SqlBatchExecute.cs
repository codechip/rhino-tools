#region license
// Copyright (c) 2005 - 2007 Ayende Rahien (ayende@ayende.com)
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//     * Neither the name of Ayende Rahien nor the names of its
//     contributors may be used to endorse or promote products derived from this
//     software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion


using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Rhino.Commons.Tasks
{
    public class SqlBatchExecute : Task
    {
        private ITaskItem[] scripts;
        private string connectionString;
        private bool useTransaction;
        private int commandTimeout = 600;

        [Required]
        public string ConnectionString
        {
            get { return connectionString; }
            set { connectionString = value; }
        }

        [Required]
        public ITaskItem[] Scripts
        {
            get { return scripts; }
            set { scripts = value; }
        }

        public bool UseTransaction
        {
            get { return useTransaction; }
            set{ useTransaction = value;}
        }

        private class CommandInfo
        {
            public string FileName;
            public string Command;
            public int Line;

            public CommandInfo(string fileName, string command, int line)
            {
                FileName = fileName;
                Command = command;
                Line = line;
            }
        }

        public override bool Execute()
        {
            if(scripts.Length==0)
            {
                Log.LogWarning("No scripts were defined!");
                return true;
            }
            List<CommandInfo> commands = new List<CommandInfo>();
            if(!BuildCommands(commands))
                return false;

            using(SqlConnection connection = new SqlConnection(ConnectionString))
            {
                try
                {
                    connection.Open();                    
                }
                catch(Exception e)
                {
                    Log.LogError(string.Format("Could not connect to [{0}] because {1}", connectionString, e));
                    return false;
                }
                int count=0;
                string currentFile = null;
                SqlTransaction transaction=null;
                if (UseTransaction)
                    transaction = connection.BeginTransaction();
                using(transaction)
                {
                    SqlCommand command = connection.CreateCommand();
                    command.CommandTimeout = CommandTimeout;
                    command.Connection = connection;
                    command.Transaction = transaction;
                    foreach (CommandInfo commandInfo in commands)
                    {
                        if(currentFile!=commandInfo.FileName)
                        {
                            Log.LogMessage("Now executing statements from "+commandInfo.FileName);
                            currentFile = commandInfo.FileName;
                        }
                        command.CommandText = commandInfo.Command;
                        try
                        {
                            count += 1;
                            command.ExecuteNonQuery();
                            if(count%100==0)
                            {
                                Log.LogMessage("Successfully executed {0} of {1} commands", count,commands.Count);
                            }
                        }
                        catch(Exception e)
                        {
                            string s = string.Format(
@"Could not execute command.
{0} {1}: {2}
Error: {3}", commandInfo.FileName, commandInfo.Line, commandInfo.Command, e);
                            Log.LogError(s);
                            return false;
                        }
                    }
                    if(transaction!=null)
                        transaction.Commit();
                }
            }
            return true;
        }

        public int CommandTimeout
        {
            get { return commandTimeout; }
            set{ commandTimeout = value;}
        }

        private bool BuildCommands(List<CommandInfo> commands)
        {
            foreach (ITaskItem item in scripts)
            {
                string[] fileText;
                try
                {
                    fileText = File.ReadAllLines(item.ItemSpec);
                }
                catch(Exception e)
                {
                    Log.LogError(string.Format("Could not open file {0} because: {1}", item.ItemSpec, e));
                    return false;
                }
                StringBuilder cmd = new StringBuilder();
                int lineNumber = 0;
                foreach (string line in fileText)
                {
                    lineNumber += 1;
                    bool endOfCommand = line.Trim().Equals("GO", StringComparison.InvariantCultureIgnoreCase);
                    if(endOfCommand && cmd.Length!=0 )
                    {
                        commands.Add(new CommandInfo(item.ItemSpec, cmd.ToString(), lineNumber));
                        cmd = new StringBuilder();
                        continue;
                    }
                    cmd.AppendLine(line);
                }
                if (cmd.Length != 0)
                    commands.Add(new CommandInfo(item.ItemSpec, cmd.ToString(), lineNumber));
            }
            return true;
        }
    }
}