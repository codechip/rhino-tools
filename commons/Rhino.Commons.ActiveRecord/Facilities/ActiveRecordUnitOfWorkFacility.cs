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

using System.Reflection;
using Castle.MicroKernel.Facilities;
using Castle.MicroKernel.Registration;
using Rhino.Commons.ForTesting;

namespace Rhino.Commons.Facilities
{
    public class ActiveRecordUnitOfWorkFacility : AbstractFacility
    {
        private readonly Assembly[] assemblies;
    	private string repositoryKey;

    	public ActiveRecordUnitOfWorkFacility(string assembly)
            : this(new string[] { assembly, })
        {
        }
        public ActiveRecordUnitOfWorkFacility(string[] assemblies)
			:this(null,assemblies)
        {
        }
		public ActiveRecordUnitOfWorkFacility(string repositoryKey, string assembly)
			: this(repositoryKey, new string[] { assembly, })
		{
		}
		public ActiveRecordUnitOfWorkFacility(string repositoryKey,string[] assemblies)
		{
			this.repositoryKey = repositoryKey;
			this.assemblies = new Assembly[assemblies.Length];
			for (int i = 0; i < assemblies.Length; i++)
			{
				this.assemblies[i] = Assembly.Load(assemblies[i]);
			}
		}



    	protected override void Init()
        {
    		ComponentRegistration<object> component =
				Component.For(typeof (IRepository<>)).ImplementedBy(typeof (ARRepository<>));
			if(!string.IsNullOrEmpty(repositoryKey))
			{
				component.Named(repositoryKey);
			}
    		Kernel.Register(component);
        	ComponentRegistration<IUnitOfWorkFactory> registerFactory = 
				Component.For<IUnitOfWorkFactory>()
        		.ImplementedBy<ActiveRecordUnitOfWorkFactory>();

			// if we are running in test mode, we don't want to register
			// the assemblies directly, we let the DatabaseTestFixtureBase do it
			// this allow us to share the configuration between the test & prod projects
			if(DatabaseTestFixtureBase.IsRunningInTestMode == false)
			{
				registerFactory.DependsOn(Property.ForKey("assemblies").Eq(assemblies));
			}
        	Kernel.Register(registerFactory);
        }

        public Assembly[] Assemblies
        {
            get { return assemblies; }
        }
    }
}
