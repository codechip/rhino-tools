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
	using System.Collections.Generic;
	using Boo.Lang;
	using Castle.Core.Configuration;
	using Configuration;

	public class Extend : IQuackFu, INeedSecondPassRegistration, IConfigurationFormatter
	{
		private readonly Component component;
		private readonly IEnumerable<Type> forwardedTypes;

		public Extend(string name, params IComponentExtension[] extensions)
		{
			component = BooReader.GetComponentByName(name);
			BooReader.NeedSecondPassRegistrations.Add(this);
			component.RegisterExtensions(extensions);
		}

		public Extend(ComponentReference reference, params IComponentExtension[] extensions)
			: this(reference.Name, extensions)
		{
		}

		public Extend(string name, Type forwaredType, params IComponentExtension[] extensions)
			: this(name, new Type[] { forwaredType }, extensions)
		{
		}

		public Extend(ComponentReference reference, Type forwaredType, params IComponentExtension[] extensions)
			: this(reference.Name, forwaredType, extensions)
		{
		}

		public Extend(string name, IEnumerable<Type> forwardedTypes, params IComponentExtension[] extensions)
			: this(name, extensions)
		{
			this.forwardedTypes = forwardedTypes;
		}

		public Extend(ComponentReference reference, IEnumerable<Type> forwardedTypes, params IComponentExtension[] extensions)
			: this(reference.Name, forwardedTypes, extensions)
		{
		}

		public void Format(IConfiguration parent, string name, bool useAttribute)
		{
			component.Format(parent, name, useAttribute);
		}

		public object QuackGet(string name, object[] parameters)
		{
			return component.QuackGet(name, parameters);
		}

		public object QuackSet(string name, object[] parameters, object value)
		{
			return component.QuackSet(name, parameters, value);
		}

		public object QuackInvoke(string name, params object[] args)
		{
			return component.QuackInvoke(name, args);
		}

		public void RegisterSecondPass()
		{
			if (forwardedTypes != null)
			{
				foreach (Type forwardedType in forwardedTypes)
				{
					component.kernel.RegisterHandlerForwarding(forwardedType, component.Name);
				}
			}
		}
	}
}