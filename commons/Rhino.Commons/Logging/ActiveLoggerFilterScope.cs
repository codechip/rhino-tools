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
using log4net.Appender;
using log4net.Core;
using log4net.Filter;
using log4net.Repository;

namespace Rhino.Commons.Logging
{
    /// <summary>
    /// Filter out all log entries made by loggers except for the activeLogger supplied
    /// within the scope of a <see langword="using" /> statement
    /// </summary>
    /// <remarks>
    /// Be aware that after disposing of an instance of this class,
    /// ALL registered appenders will no longer be listening to
    /// log entries.
    /// </remarks>
    public class ActiveLoggerFilterScope : IDisposable
    {
        public string scopeKey = "ActiveLoggerFilterScope-" + Guid.NewGuid();


        public ActiveLoggerFilterScope(Type activeLogger) : this(activeLogger.FullName) {}

        public ActiveLoggerFilterScope(string activeLogger)
        {
            ILoggerRepository repository = LogManager.GetRepository();
            repository.Threshold = Level.All;
            repository.Properties[scopeKey] = activeLogger;

            foreach (IAppender appender in LogManager.GetRepository().GetAppenders())
            {
                AppenderSkeleton appenderSkeleton = appender as AppenderSkeleton;

                if (appenderSkeleton != null)
                {
                    // only inject ActiveLoggerFilter once
                    if (appenderSkeleton.FilterHead == null ||
                        appenderSkeleton.FilterHead.GetType() != typeof(ActiveLoggerFilter))
                    {
                        // inject ActiveLoggerFilter into the head of the filter list
                        IFilter filter = new ActiveLoggerFilter(scopeKey);
                        filter.ActivateOptions();
                        filter.Next = appenderSkeleton.FilterHead; // ???
                        appenderSkeleton.ClearFilters(); // ???
                        appenderSkeleton.AddFilter(filter); // ???
                    }
                }
            }
        }

        public void Dispose()
        {
            ILoggerRepository repository = LogManager.GetRepository();
            repository.Properties.Remove(scopeKey);
            repository.Threshold = Level.Off;
            //make sure to remove our ActiveLoggerFilter
            //this is required so that ActiveLoggerFilterScope can be used multiple times
            RemoveAllFiltersFromAllAppenders();
        }


        private void RemoveAllFiltersFromAllAppenders()
        {
            foreach (IAppender appender in LogManager.GetRepository().GetAppenders())
            {
                AppenderSkeleton appenderSkeleton = appender as AppenderSkeleton;
                if (appenderSkeleton != null)
                {
                    appenderSkeleton.ClearFilters();
                }
            }
        }



        private class ActiveLoggerFilter : FilterSkeleton
        {
            private string repositoryPropertiesKey = null;

            public ActiveLoggerFilter(string repositoryPropertiesKey)
            {
                this.repositoryPropertiesKey = repositoryPropertiesKey;
            }

            public override FilterDecision Decide(LoggingEvent loggingEvent)
            {
                string activeLogger = (string) loggingEvent.Repository.Properties[repositoryPropertiesKey];

                if (activeLogger != null && activeLogger != loggingEvent.LoggerName)
                {
                    return FilterDecision.Deny;
                }
                else
                {
                    return FilterDecision.Neutral;
                }
            }
        }
    }
}
