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


using System.Collections;
using Castle.Core.Configuration;
using Castle.MicroKernel;

namespace Rhino.Commons.Binsor
{
	public class EventWireExtension : AbstractComponentExtension
	{
		private readonly string _eventName;
		private readonly IDictionary _subscribers;

		public EventWireExtension(string eventName, IDictionary subscribers)
		{
			_eventName = eventName;
			_subscribers = subscribers;
		}

		protected override void ApplyToConfiguration(IKernel kernel, IConfiguration compConfig)
		{
			if (_subscribers.Count > 0)
			{
				IConfiguration subscribers = ObtainSubscribers(compConfig);

				foreach (DictionaryEntry entry in _subscribers)
				{
					IConfiguration subscriber = new MutableConfiguration("subscriber");
					ComponentReference componentId = (ComponentReference)entry.Key;
					subscriber.Attributes["event"] = _eventName;
					subscriber.Attributes["id"] = componentId.Name;
					subscriber.Attributes["handler"] = entry.Value.ToString();
					subscribers.Children.Add(subscriber);
				}
			}
		}

		private static IConfiguration ObtainSubscribers(IConfiguration config)
		{
			IConfiguration subscribers = config.Children["subscribers"];

			if (subscribers == null)
			{
				subscribers = new MutableConfiguration("subscribers");
				config.Children.Add(subscribers);
			}

			return subscribers;
		}
	}
}
