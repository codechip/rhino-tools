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
using Castle.ActiveRecord;
using Castle.MonoRail.Framework;
using Exesto.Model;
using log4net;
using log4net.Appender;
using log4net.Config;
using NHibernate.Tool.hbm2ddl;
using Rhino.Commons;

namespace Exesto.Web.Controllers
{
	[Layout("default")]
	public class ConstructController : Controller
	{
		public void Database()
		{
			MemoryAppender memoryAppender = new MemoryAppender();
			BasicConfigurator.Configure(LogManager.GetLogger(typeof (SchemaExport)).Logger.Repository, memoryAppender);
			ActiveRecordStarter.CreateSchema();
			PropertyBag["events"] = memoryAppender.GetEvents();
		}

		public void DummyData()
		{
			string[] names = {
				"accelerator", 
				"worm", 
				"Wittgenstein",
				"unprimed", 
			};
			DateTime start = DateTime.Now;
			With.Transaction(delegate
			{
				List<Subject> subjects = new List<Subject>();
				foreach (string name in names)
				{
					Subject subject = new Subject();
					subject.Name = name;
					subjects.Add(subject);
					Repository<Subject>.Save(subject);

					for (int i = 0; i < 50; i++)
					{
						Question q = new Question();
						q.Subject = subject;
						subject.Questions.Add(q);
						q.Title = "Question #" + i;
						q.Content = "Why?";
						q.Answer = "Because!";
						Repository<Question>.Save(q);
					}
				}
				PropertyBag["subjects"] = subjects;
			});
			PropertyBag["duration"] = DateTime.Now - start;
		}
	}
}