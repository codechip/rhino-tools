﻿#region license
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


using MbUnit.Framework;
using Rhino.Mocks.Interfaces;

namespace Rhino.Mocks.Tests.FieldsProblem
{
	using System;
	using System.Diagnostics;
	using System.IO;

	[TestFixture]
	public class PropertyWithTypeParameterTest
	{
		[Test]
		public void CreatedClosedGenericType()
		{
			MockRepository mocks = new MockRepository();
			mocks.StrictMock<ClosedGenericType>();
		}


		[Test]
		public void UsingdoOnMethodWithGenericReturnValue()
		{
			MockRepository mocks = new MockRepository();
			IGenericType<object> mock = mocks.StrictMock<IGenericType<object>>();
			IMethodOptions<object> methodOptions = Expect.Call(mock.MyMethod());
			methodOptions.Do((MyDelegate)delegate { return new object(); });
		}

		/// <summary>
		/// If this fails with Execution Engine Exception, you need to install the hotfix 
		/// for KB 957542.
		/// There is a bug in .Net 3.5 SP1 that this test exposes.
		/// </summary>
		[Test]
		public void DoubleGeneric()
		{
			string clrInstallationDir = Path.GetDirectoryName(typeof(object).Assembly.Location);
			string mscorwksFilename = Path.Combine(clrInstallationDir, "mscorwks.dll");
			FileVersionInfo clrVersion = FileVersionInfo.GetVersionInfo(mscorwksFilename);
			if(clrVersion.ProductMajorPart == 2 && 
				clrVersion.ProductMinorPart == 0 &&
				clrVersion.ProductBuildPart == 50727)
			{
				// CLR 2.0, now need to check if we have the .NET 3.5 with SP1 installed,
				// without the hotfix
				if (clrVersion.ProductPrivatePart >= 3053 &&
					clrVersion.ProductPrivatePart < 3068)
				{
					Assert.Ignore("You are running on .NET 3.5 SP1, without the KB 957542 hotfix. This version of the CLR has a bug that cause this test to fail");
				}
			}
			MockRepository mocks = new MockRepository();
			IDoubleGeneric<int> mock = mocks.StrictMock<IDoubleGeneric<int>>();
			Expect.Call(mock.Method<string>(1, ""));
		}
	}

	public interface IDoubleGeneric<One>
	{
		object Method<T>(One one, T two);
	}

	public interface IGenericType<T>
	{
		T MyMethod();
	}

	public delegate object MyDelegate();

	public class ClosedGenericType : OpenGenericType<TypeParameterType>
	{
		public override TypeParameterType GenericProperty
		{
			get { return null; }
		}
	}

	public abstract class OpenGenericType<T>
	{
		public abstract T GenericProperty { get; }
	}

	public class TypeParameterType
	{
	}
}