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
using Rhino.Mocks.Interfaces;

namespace Rhino.Mocks.Impl
{
	/// <summary>
	/// Behave like a stub, all properties and events acts normally, methods calls
	/// return default values by default (but can use expectations to set them up), etc.
	/// </summary>
	public class StubRecordMockState : RecordMockState
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="StubRecordMockState"/> class.
		/// </summary>
		/// <param name="mockedObject">The proxy that generates the method calls</param>
		/// <param name="repository">Repository.</param>
		public StubRecordMockState(IMockedObject mockedObject, MockRepository repository)
			: base(mockedObject, repository)
		{
			Type[] types = mockedObject.ImplementedTypes;
			SetPropertyBehavior(mockedObject, types);
		}

		private void SetPropertyBehavior(IMockedObject mockedObject, params Type[] types)
		{
			foreach (Type implementedType in types)
			{
				if (implementedType.BaseType != null && implementedType.BaseType != typeof(object))
				{
					SetPropertyBehavior(mockedObject, implementedType.BaseType);
				}

				SetPropertyBehavior(mockedObject, implementedType.GetInterfaces());

				foreach (PropertyInfo property in implementedType.GetProperties())
				{
					if (property.CanRead && property.CanWrite)
					{
						bool alreadyHasValue = mockedObject.RegisterPropertyBehaviorFor(property);
						if (property.PropertyType.IsValueType && alreadyHasValue == false)
						{
							//make sure that it creates a default value for value types
							mockedObject.HandleProperty(property.GetSetMethod(true),
														new object[] { Activator.CreateInstance(property.PropertyType) });
						}
					}
				}

			}
		}

		/// <summary>
		/// We don't care much about expectations here, so we will remove the expectation if
		/// it is not closed.
		/// </summary>
		protected override void AssertPreviousMethodIsClose()
		{
			if (LastExpectation == null)
				return;
			if (LastExpectation.ActionsSatisfied)
				return;
			Repository.Recorder.RemoveExpectation(LastExpectation);
			LastExpectation = null;
		}

		/// <summary>
		/// Verify that we can move to replay state and move
		/// to the reply state.
		/// </summary>
		/// <returns></returns>
		public override IMockState Replay()
		{
			AssertPreviousMethodIsClose();
			return new StubReplayMockState(this);
		}

        /// <summary>
        /// Get the default call count range expectation
        /// </summary>
        /// <returns></returns>
        protected override Range GetDefaultCallCountRangeExpectation()
        {
            return new Range(1, null);
        }
	}
}
