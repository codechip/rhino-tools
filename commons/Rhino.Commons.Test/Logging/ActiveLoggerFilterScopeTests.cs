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
using log4net;
using log4net.Config;
using MbUnit.Framework;
using Rhino.Commons.Logging;

namespace Rhino.Commons.Test.Logging
{
    [TestFixture]
    public class ActiveLoggerFilterScopeTests
    {
        ILog loggerA;
        ILog loggerB;
        InMemoryMessageAppender appender;


        [SetUp]
        public void TestInitialize()
        {
            appender = new InMemoryMessageAppender();
            //all loggers created within these test will inherit our appender we register here
            BasicConfigurator.Configure(appender);

            loggerA = LogManager.GetLogger("LoggerA");
            loggerB = LogManager.GetLogger(GetType());
        }


        [TearDown]
        public void TestCleanup()
        {
            LogManager.ResetConfiguration();
        }


        [Test]
        public void VerifyTestSetup_CanLogWithOurLoggers()
        {
            //making sure that our test loggers are working

            loggerA.Info("First message");
            loggerB.Info("Second message");
        	Assert.AreEqual(2, appender.Messages.Count,"Failed to log messages to appender" );
            Assert.AreEqual("First message", appender.Messages[0]);
            Assert.AreEqual("Second message", appender.Messages[1]);
        }


        [Test]
        public void WillListenToSpecifiedLoggerWhileFilteringOutAllOtherLoggers()
        {
            using (new ActiveLoggerFilterScope(loggerB.Logger.Name))
            {
                loggerA.Info("First message");
                loggerB.Info("Second message");
            }

            Assert.AreEqual(1, appender.Messages.Count);
            Assert.AreEqual("Second message", appender.Messages[0]);
        }


        [Test]
        public void AllLoggersWillBeDisabledOnceDisposed()
        {
            using (new ActiveLoggerFilterScope(loggerA.Logger.Name))
            {
            }

            loggerA.Info("First message");
            loggerB.Info("Second message");
            LogManager.GetLogger("NewLogger").Info("Message from NewLogger");

            Assert.AreEqual(0, appender.Messages.Count);
        }


        [Test]
        public void CanBeUsedMultipleTimesInYourCode()
        {
            using (new ActiveLoggerFilterScope(loggerB.Logger.Name))
            {
                loggerA.Info("First message");
                loggerB.Info("Second message");
            }

            using (new ActiveLoggerFilterScope(loggerB.Logger.Name))
            {
                LogManager.GetLogger("NewLogger").Info("Third message");
                loggerB.Info("Forth message");
            }

            loggerB.Info("Fith message");

            Assert.AreEqual(2, appender.Messages.Count);
            Assert.AreEqual("Second message", appender.Messages[0]);
            Assert.AreEqual("Forth message", appender.Messages[1]);
        }
    }
}
