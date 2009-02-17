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
using Castle.Core;
using Castle.MicroKernel.Registration;

namespace Rhino.Commons.Binsor
{
	#region LifestyleExtension 

	public abstract class LifestyleExtension : IComponentExtension
	{
		private readonly LifestyleType lifestyle;

		public LifestyleExtension(LifestyleType lifestyle)
		{
			this.lifestyle = lifestyle;
		}

		public virtual void Apply(Component component, ComponentRegistration registration)
		{
			registration.LifeStyle.Is(lifestyle);
		}
	}

	#endregion

	#region Transient Lifestyle

	public class Transient : LifestyleExtension
	{
		public Transient() : base(LifestyleType.Transient)
		{
		}
	}

	#endregion

	#region Singelton Lifestyle

	public class Singleton : LifestyleExtension
	{
		public Singleton() : base(LifestyleType.Singleton)
		{
		}
	}

	#endregion

	#region Thread Lifestyle

	public class Thread : LifestyleExtension
	{
		public Thread() : base(LifestyleType.Thread)
		{
		}
	}

	#endregion

	#region Pooled Lifestyle

	public class Pooled : LifestyleExtension
	{
		private int? initialPoolSize;
		private int? maxPoolSize;

		public Pooled() : base(LifestyleType.Pooled)
		{
		}

		public int InitialPoolSize
		{
			set { initialPoolSize = value; }
		}

		public int MaxPoolSize
		{
			set { maxPoolSize = value; }
		}

		public override void Apply(Component component, ComponentRegistration registration)
		{
			registration.LifeStyle.PooledWithSize(initialPoolSize.GetValueOrDefault(), maxPoolSize.GetValueOrDefault());
		}
	}

	#endregion

	#region PerWebRequest Lifestyle

	public class PerWebRequest : LifestyleExtension
	{
		public PerWebRequest() : base(LifestyleType.PerWebRequest)
		{
		}
	}

	#endregion

	#region Custom Lifestyle

	public class Custom : LifestyleExtension
	{
		private readonly Type customLifestyleType;

		public Custom(Type customLifestyleType) : base(LifestyleType.Custom)
		{
			this.customLifestyleType = customLifestyleType;
		}

		public override void Apply(Component component, ComponentRegistration registration)
		{
			registration.LifeStyle.Custom(customLifestyleType);
		}
	}

	#endregion
}
