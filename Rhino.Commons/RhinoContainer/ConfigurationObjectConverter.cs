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


namespace Rhino.Commons
{
	using System;
	using System.Text;
	using Castle.Core.Configuration;
	using System.Reflection;

    using Castle.MicroKernel.SubSystems.Conversion;

	/// <summary>
	/// Perform the conversion by mapping the configuration values
	/// to the object properties.
	/// </summary>
	[Serializable]
	public class ConfigurationObjectConverter : AbstractTypeConverter
	{
		public override bool CanHandleType(Type type)
		{
			return type.IsDefined(typeof(ConfigurationObjectAttribute), true);
		}

		public override object PerformConversion(string value, Type targetType)
		{
			throw new NotImplementedException();
		}

		public override object PerformConversion(IConfiguration configuration, Type targetType)
		{
			object instance = Activator.CreateInstance(targetType);

			BindingFlags bindingFlags = BindingFlags.IgnoreCase | BindingFlags.Instance | 
				BindingFlags.Public | BindingFlags.NonPublic;
			foreach (IConfiguration itemConfig in configuration.Children)
			{
				PropertyInfo propInfo = targetType.GetProperty(itemConfig.Name, bindingFlags);
				
				if (propInfo == null)//in configuration and not in the object? this is an error.
					throw new InvalidOperationException(
						string.Format("Could not find property {0} on type {1}", itemConfig.Name, targetType));
				
				if(propInfo.CanWrite==false)
					throw new InvalidOperationException(
						string.Format("Could not set property {0} on type {1}. It has no setter", 
						itemConfig.Name, targetType));

				object value = Context.Composition.PerformConversion(itemConfig.Value, propInfo.PropertyType);

				propInfo.SetValue(instance, value, null);
			}

			return instance;
		}
	}
}
