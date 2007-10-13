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
using Boo.Lang;
using Castle.Core.Configuration;
using Castle.MicroKernel;

namespace Rhino.Commons.Binsor
{
    public class Facility : IQuackFu
    {
        private readonly Type facility;
        private readonly string key;
		private readonly object[] arguments;
        private readonly IConfiguration configuration = new MutableConfiguration("facility");
        //important, we need to get this when we create the facility, because we need
        //to support nested components
        private readonly IKernel kernel = IoC.Container.Kernel;

        public Facility(string name, Type facility)
        {
            key = name;
            this.facility = facility;
        }

		public Facility(string name, Type facility, params object[] arguments)
			: this(name, facility)
		{
			this.arguments = arguments;
		}

        public string Key
        {
            get { return key; }
        }

        public Facility Register()
        {
        	IFacility instance;

			if (arguments == null || arguments.Length == 0)
			{
				instance = (IFacility) Activator.CreateInstance(facility);
			}
			else
			{
				instance = (IFacility) Activator.CreateInstance(facility, arguments);
			}

            kernel.ConfigurationStore.AddFacilityConfiguration(key, configuration);
            kernel.AddFacility(key, instance);

            return this;
        }

        public object QuackGet(string name, object[] property_parameters)
        {
            throw new NotSupportedException("You can't get a property on a facility");
        }

        public object QuackSet(string name, object[] property_parameters, object value)
        {
			ConfigurationHelper.SetConfigurationValue(configuration, name, value, true);
            return null;
        }

        public object QuackInvoke(string name, params object[] args)
        {
            throw new NotSupportedException("You can't invoke a method on a facility");
        }
    }
}