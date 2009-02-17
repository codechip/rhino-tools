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
using System.Collections.Generic;
using System.IO;
using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Repository.Hierarchy;
using MbUnit.Framework;
using Rhino.Commons.Logging;

namespace Rhino.Commons.Test
{
    using Util;

    [TestFixture]
    public class WithLogTests
    {
        [SetUp]
        public void TestInitialize()
        {
            //turn on log4net logging (and supress output to console)
            BasicConfigurator.Configure(new MemoryAppender());
        }


        [TearDown]
        public void TestCleanup()
        {
            LogManager.ResetConfiguration();
        }


        [Test]
        public void NoOpWillReturnEmptyCollection()
        {
            bool called = false;
            ICollection<string> logMessages = With.Log("SomeLogger", delegate
            {
                called = true;
            });

            Assert.IsTrue(called);
            Assert.IsTrue(logMessages.Count == 0);
        }


        [Test]
        public void WillReturnDebugLogEntryMessagesAndAbove()
        {
            ILog log = LogManager.GetLogger("SomeLog");

            string debugMessage = "Some debug message";
            string fatalMessage = "Some fatal message";
            IList<string> logMessages = With.Log(log.Logger.Name, delegate
            {
                log.Debug(debugMessage);
                log.Fatal(fatalMessage);
            });

            Assert.AreEqual(2, logMessages.Count);
            Assert.AreEqual(debugMessage, logMessages[0]);
            Assert.AreEqual(fatalMessage, logMessages[1]);
        }

        [Test]
        public void WillAcceptATypeAsTheLoggerParameter()
        {
            ILog log = LogManager.GetLogger(GetType());

            string debugMessage = "Some debug message";
            IList<string> logMessages = With.Log(GetType(), delegate
            {
                log.Debug(debugMessage);
            });

            Assert.AreEqual(1, logMessages.Count);
        }


        [Test]
        public void SafeToUseMultipleTimes()
        {
            ILog log = LogManager.GetLogger("SomeLog");

            With.Log(log.Logger.Name, delegate
            {
                log.Debug("Some debug message");
            });

            string debugMessage = "Another debug message";
            IList<string> logMessages = With.Log(log.Logger.Name, delegate
            {
                log.Debug(debugMessage);
            });

            Assert.AreEqual(1, logMessages.Count);
            Assert.AreEqual(debugMessage, logMessages[0]);
        }


        [Test]
        public void GotchaWillTurnOffLoggingAfterDispose()
        {
            ILog log = LogManager.GetLogger("SomeLog");

            With.Log(log.Logger.Name, delegate { });

            //wire up a new appender to listen to log entries
            InMemoryMessageAppender appender = new InMemoryMessageAppender();
            ((Logger)log.Logger).AddAppender(appender);

            log.Info("This messgae will go into the blackhole");

            Assert.IsTrue(appender.Messages.Count == 0);

        }
    }
}
