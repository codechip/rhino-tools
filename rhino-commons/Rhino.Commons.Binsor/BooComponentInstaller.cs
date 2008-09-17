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

namespace Rhino.Commons.Binsor
{
	using System;
	using Castle.MicroKernel;
	using Castle.Windsor;
    using Castle.Core.Resource;
    using Castle.MicroKernel.SubSystems.Resource;
	public class BooComponentInstaller : IComponentsInstaller
	{
		private readonly string fileName;
		private readonly IEnvironmentInfo env;
	    private readonly CustomUri uri;
		public BooComponentInstaller(String fileName, IEnvironmentInfo env)
		{
			this.fileName = fileName;
			this.env = env;
		}
        public BooComponentInstaller(CustomUri uri, IEnvironmentInfo env)
        {
            this.uri = uri;
            this.env = env;
        }
		public void SetUp(IWindsorContainer container, IConfigurationStore store)
		{
			string name = null;
            name = env != null ? env.GetEnvironmentName() : "ResourceWindsor";

		    if (uri != null)
		    {
		        BooReader.Read(container, uri, name);
		    }
		    else
		    {
		        BooReader.Read(container, fileName, name);
		    }
		}
	}
}
