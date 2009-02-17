// Copyright (c) 2005 - 2008 Ayende Rahien (ayende@ayende.com)
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
#if DOTNET35
namespace Rhino.Mocks.Tests.FieldsProblem
{
	using Exceptions;
	using MbUnit.Framework;

	[TestFixture]
	public class FieldProblem_Mike
	{
		[Test]
		public void Can_do_nested_virtual_calls()
		{
			var mocks = new MockRepository();
			var subject = mocks.PartialMock<SUT>();
			mocks.ReplayAll();

			subject.VirtualMethod();

			subject.AssertWasCalled(it => it.NestedVirtualMethod());
		}

		[Test]
		[ExpectedException(typeof(ExpectationViolationException),
			"SUT.NestedVirtualMethod(); Expected #1, Actual #0.")]
		public void Can_do_nested_virtual_calls_when_not_called()
		{
			var mocks = new MockRepository();
			var subject = mocks.PartialMock<SUT>();
			mocks.ReplayAll();


			subject.AssertWasCalled(it => it.NestedVirtualMethod());
		}


		#region Nested type: SUT

		public class SUT
		{
			public virtual void VirtualMethod()
			{
				NestedVirtualMethod();
			}

			public virtual void NestedVirtualMethod()
			{
			}
		}

		#endregion
	}
}
#endif