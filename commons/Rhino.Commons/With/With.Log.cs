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
using log4net;
using log4net.Repository.Hierarchy;
using Rhino.Commons.Logging;

namespace Rhino.Commons.Util
{
    public static partial class With
    {
        /// <summary>
        /// Return the log entry messages created by the logger specified by
        /// <paramref name="loggerName"/> while executing the delegate <paramref
        /// name="operation"/>
        /// </summary>
        public static IList<string> Log(string loggerName, Proc operation)
        {
            Logger logger = (Logger)LogManager.GetLogger(loggerName).Logger;
            InMemoryMessageAppender appender = new InMemoryMessageAppender();
            try
            {
                logger.AddAppender(appender);
                logger.Level = logger.Hierarchy.LevelMap["DEBUG"];
                using (new ActiveLoggerFilterScope(loggerName))
                {
                    operation();
                }
                return appender.Messages;
            }
            finally
            {
                logger.RemoveAppender(appender);
            }
        }

        /// <summary>
        /// overload for <see cref="Log(string, Proc)"/>
        /// </summary>
        public static IList<string> Log(Type loggerType, Proc operation)
        {
            return Log(loggerType.FullName, operation);
        }

        /// <summary>
        /// overload for <see cref="Log(string, Proc)"/>
        /// </summary>
        public static IList<string> Log<T>(Proc operation)
        {
            return Log(typeof(T), operation);
        }
    }
}