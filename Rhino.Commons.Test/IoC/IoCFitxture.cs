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
using Castle.Windsor;
using NHibernate.Criterion;
using MbUnit.Framework;

namespace Rhino.Commons.Test.IoCTests
{
    using System.Data.SqlClient;

    [TestFixture]
    public class IoCFitxture
    {
        [TearDown]
        public void TestCleanup()
        {
            IoC.Initialize(null);
        }
        
        [Test]
        public void LocalContainerOverideGlobalOne()
        {
            WindsorContainer container = new WindsorContainer();
            IoC.Initialize(container);
            Assert.AreSame(container,IoC.Container);
            WindsorContainer localContainer = new WindsorContainer();
            using(IoC.UseLocalContainer(localContainer))
            {
                Assert.AreSame(localContainer, IoC.Container);
            }
            Assert.AreSame(container, IoC.Container);
        }

        [Test]
        public void WillNotThrowIfTryingToResolveNonExistingComponent()
        {
            WindsorContainer container = new WindsorContainer();
            IoC.Initialize(container);
            IDisposable disposable = IoC.TryResolve<IDisposable>();
            Assert.IsNull(disposable);
        }

        [Test]
        public void WillResolveComponentsInTryResolveIfRegistered()
        {
            WindsorContainer container = new WindsorContainer();
            IoC.Initialize(container);
            IoC.Container.AddComponent<IDisposable, SqlConnection>();
            IDisposable disposable = IoC.TryResolve<IDisposable>();
            Assert.IsNotNull(disposable);
        }

        [Test]
        public void CanSpecifyDefaultValueWhenTryingToResolve()
        {
            WindsorContainer container = new WindsorContainer();
            IoC.Initialize(container);
            IDisposable disposable = IoC.TryResolve<IDisposable>(new SqlConnection());
            Assert.IsNotNull(disposable);
        }
    }
}
    
