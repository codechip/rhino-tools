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
using System.Reflection;
using System.Collections;
using Boo.Lang;
using Castle.Core.Configuration;
using Castle.MicroKernel;
using Castle.MicroKernel.Facilities;

namespace Rhino.Commons.Binsor
{
	using System.Collections.Generic;

	public class Facility : IQuackFu
	{
		private readonly string _key;
        private readonly Type _facility;
		private readonly IDictionary _parameters = new Hashtable();
		private readonly IConfiguration _configuration;

        //important, we need to get this when we create the facility, because we need
        //to support nested components
        private readonly IKernel kernel = IoC.Container.Kernel;

        public Facility(string name, Type facility)
        {
            _key = name;
            _facility = facility;
        }

		public Facility(string name, Type facility, IDictionary configuration)
			: this(name, facility)
		{
			_configuration = ConfigurationHelper.CreateConfiguration(null, "facility", configuration);
		}

        public string Key
        {
            get { return _key; }
        }

        public Facility Register()
        {
        	IFacility instance = CreateFacilityInstance();

			if (_configuration != null)
			{
				kernel.ConfigurationStore.AddFacilityConfiguration(_key, _configuration);
			}

        	kernel.AddFacility(_key, instance);

            return this;
        }

        public object QuackGet(string name, object[] property_parameters)
        {
            throw new NotSupportedException("You can't get a property on a facility");
        }

        public object QuackSet(string name, object[] property_parameters, object value)
        {
			return _parameters[name] = value;
		}

        public object QuackInvoke(string name, params object[] args)
        {
            throw new NotSupportedException("You can't invoke a method on a facility");
        }

		private IFacility CreateFacilityInstance()
		{
			List<object> parameters = new List<object>();
			ConstructorInfo constructor = SelectEligbleConstructor(parameters);
			return (IFacility) constructor.Invoke(parameters.ToArray());
		}

		private ConstructorInfo SelectEligbleConstructor(ICollection<object> arguments)
		{
			ConstructorInfo[] constructors = _facility.GetConstructors();

			foreach (ConstructorInfo constuctorInfo in constructors)
			{
				ParameterInfo[] parameters = constuctorInfo.GetParameters();
				if (parameters.Length != _parameters.Count) continue;

				arguments.Clear();

				foreach (ParameterInfo parameterInfo in parameters)
				{
					if (_parameters.Contains(parameterInfo.Name))
					{
						object parameter = _parameters[parameterInfo.Name];

						if (parameter != null && parameterInfo.ParameterType.IsInstanceOfType(parameter))
						{
							arguments.Add(parameter);
						}
						else 
						{
							break;
						}
					}
				}

				if (parameters.Length == arguments.Count)
				{
					return constuctorInfo;
				}
			}

			throw new FacilityException(
				"Unable to find a matching constructor for the Facility " +
				_facility.Name);
		}
    }
}