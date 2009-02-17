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
using System.IO;
using System.Reflection;
using System.Text;

namespace Rhino.Commons.Helpers
{
	/// <summary>
	/// this class is here so I can avoid having a reference to the System.Data.SqlServerCe.dll if I don't need it.
	/// </summary>
	public static class SqlCEDbHelper
	{
		private static string engineTypeName = "System.Data.SqlServerCe.SqlCeEngine, System.Data.SqlServerCe";
		private static Type type;
		private static PropertyInfo localConnectionString;
		private static MethodInfo createDatabase;


	    public static void CreateDatabaseFile(string filename)
		{
			if (File.Exists(filename))
				File.Delete(filename);
			if (type == null)
			{
				type = Type.GetType(engineTypeName);
				if(type==null)
					throw new InvalidOperationException("Could not load SqlCeEngine, ensure that you have the System.Data.SqlServerCe assembly in the application base path");
				localConnectionString = type.GetProperty("LocalConnectionString");
				createDatabase = type.GetMethod("CreateDatabase");
			}
			object engine = Activator.CreateInstance(type);
			localConnectionString
				.SetValue(engine, string.Format("Data Source='{0}';", filename), null);
			createDatabase
				.Invoke(engine, new object[0]);
		}
	}
}