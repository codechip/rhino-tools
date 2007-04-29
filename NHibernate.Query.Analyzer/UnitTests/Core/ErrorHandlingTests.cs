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
using System.Collections;
using System.IO;
using Ayende.NHibernateQueryAnalyzer.Exceptions;
using Ayende.NHibernateQueryAnalyzer.Model;
using MbUnit.Framework;

namespace Ayende.NHibernateQueryAnalyzer.UnitTests.Core
{

	[TestFixture]
	public class ErrorHandlingTests
	{
		Project prj;
		private static readonly string fileDoesNotExists = "does not exists.hbm.xml";
		private static readonly string fileWithBadExtention = 
			Environment.ExpandEnvironmentVariables(@"%windir%\win.ini");

		private string query = "from TestProject";

		[SetUp]
		public void SetUp()
		{
			prj = new Project("My Test Project");
		}

		[TearDown]
		public void TestCleanup()
		{
			prj.Dispose();
		}
		
		[Test]
		public void DoesntThrowOnAddFileThatDoesNotExist()
		{
			prj.AddFile(fileDoesNotExists);
		}

		[Test]
		[ExpectedException(typeof(FileLoadingException))]
		public void ThrowsWhenBuildingProjectWithMissingFiles()
		{
			prj.AddFile(fileDoesNotExists);
			try
			{
				prj.BuildProject();
			}
			catch (Exception e)
			{
				Assert.AreEqual("File " +Path.GetFullPath(fileDoesNotExists) +" does not exists",e.Message);
				throw;
			}
		}

		[Test]
		[ExpectedException(typeof(UnknownFileTypeException))]
		public void ThrowsWhenEncounterUnknownFileType()
		{
			prj.AddFile(fileWithBadExtention);

			try
			{
				prj.BuildProject();
			}
			catch (UnknownFileTypeException e)
			{
				Assert.AreEqual(fileWithBadExtention,e.Message);
				throw;
			}
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException),"Can't run queries if the project is not built.")]
		public void CantRunHqlUntilProjectIsBuilt()
		{
			prj.RunHql(query);
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException),"Can't run queries if the project is not built.")]
		public void CantRunHqlAsSqlUntilProjectIsBuilt()
		{
			prj.RunHqlAsRawSql(query);
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException),"Can't translate queries if the project was not built.")]
		public void CantTranslateQueryUntilProjectIsBuild()
		{
			prj.HqlToSql(query,new Hashtable());
		}

	}
}
