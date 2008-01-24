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
	using System.Reflection;

	/// <summary>
	/// Helper class to use delegate inference in order to get
	/// faster GetMethod approach
	/// </summary>
	public static class StaticReflection
	{
		#region Procedures

		/// <summary>
		/// Get the method of the delegate
		/// </summary>
		public static MethodInfo MethodInfo(Proc func0)
		{
			return func0.Method;
		}

		/// <summary>
		/// Get the method of the delegate
		/// </summary>
		public static MethodInfo MethodInfo<A0>(Proc<A0> func1)
		{
			return func1.Method;
		}

		/// <summary>
		/// Get the method of the delegate
		/// </summary>
		public static MethodInfo MethodInfo<A0, A1>(Proc<A0, A1> func2)
		{
			return func2.Method;
		}

		/// <summary>
		/// Get the method of the delegate
		/// </summary>
		public static MethodInfo MethodInfo<A0, A1, A2>(Proc<A0, A1, A2> func3)
		{
			return func3.Method;
		}

		/// <summary>
		/// Get the method of the delegate
		/// </summary>
		public static MethodInfo MethodInfo<A0, A1, A2, A3>(Proc<A0, A1, A2, A3> func4)
		{
			return func4.Method;
		}

		/// <summary>
		/// Get the method of the delegate
		/// </summary>
		public static MethodInfo MethodInfo<A0, A1, A2, A3, A4>(Proc<A0, A1, A2, A3, A4> func5)
		{
			return func5.Method;
		}

		/// <summary>
		/// Get the method of the delegate
		/// </summary>
		public static MethodInfo MethodInfo<A0, A1, A2, A3, A4, A5>(Proc<A0, A1, A2, A3, A4, A5> func6)
		{
			return func6.Method;
		}

		/// <summary>
		/// Get the method of the delegate
		/// </summary>
		public static MethodInfo MethodInfo<A0, A1, A2, A3, A4, A5, A6>(Proc<A0, A1, A2, A3, A4, A5, A6> func7)
		{
			return func7.Method;
		}

		/// <summary>
		/// Get the method of the delegate
		/// </summary>
		public static MethodInfo MethodInfo<A0, A1, A2, A3, A4, A5, A6, A7>(Proc<A0, A1, A2, A3, A4, A5, A6, A7> func8)
		{
			return func8.Method;
		}

		/// <summary>
		/// Get the method of the delegate
		/// </summary>
		public static MethodInfo MethodInfo<A0, A1, A2, A3, A4, A5, A6, A7, A8>(Proc<A0, A1, A2, A3, A4, A5, A6, A7, A8> func9)
		{
			return func9.Method;
		}

		#endregion

		#region Functions

		/// <summary>
		/// Get the method of the delegate
		/// </summary>
		public static MethodInfo MethodInfo<TRet>(Func<TRet> func0)
		{
			return func0.Method;
		}

		/// <summary>
		/// Get the method of the delegate
		/// </summary>
		public static MethodInfo MethodInfo<TRet, A0>(Func<TRet, A0> func1)
		{
			return func1.Method;
		}

		/// <summary>
		/// Get the method of the delegate
		/// </summary>
		public static MethodInfo MethodInfo<TRet, A0, A1>(Func<TRet, A0, A1> func2)
		{
			return func2.Method;
		}

		/// <summary>
		/// Get the method of the delegate
		/// </summary>
		public static MethodInfo MethodInfo<TRet, A0, A1, A2>(Func<TRet, A0, A1, A2> func3)
		{
			return func3.Method;
		}

		/// <summary>
		/// Get the method of the delegate
		/// </summary>
		public static MethodInfo MethodInfo<TRet, A0, A1, A2, A3>(Func<TRet, A0, A1, A2, A3> func4)
		{
			return func4.Method;
		}

		/// <summary>
		/// Get the method of the delegate
		/// </summary>
		public static MethodInfo MethodInfo<TRet, A0, A1, A2, A3, A4>(Func<TRet, A0, A1, A2, A3, A4> func5)
		{
			return func5.Method;
		}

		/// <summary>
		/// Get the method of the delegate
		/// </summary>
		public static MethodInfo MethodInfo<TRet, A0, A1, A2, A3, A4, A5>(Func<TRet, A0, A1, A2, A3, A4, A5> func6)
		{
			return func6.Method;
		}

		/// <summary>
		/// Get the method of the delegate
		/// </summary>
		public static MethodInfo MethodInfo<TRet, A0, A1, A2, A3, A4, A5, A6>(Func<TRet, A0, A1, A2, A3, A4, A5, A6> func7)
		{
			return func7.Method;
		}

		/// <summary>
		/// Get the method of the delegate
		/// </summary>
		public static MethodInfo MethodInfo<TRet, A0, A1, A2, A3, A4, A5, A6, A7>(Func<TRet, A0, A1, A2, A3, A4, A5, A6, A7> func8)
		{
			return func8.Method;
		}

		/// <summary>
		/// Get the method of the delegate
		/// </summary>
		public static MethodInfo MethodInfo<TRet, A0, A1, A2, A3, A4, A5, A6, A7, A8>(Func<TRet, A0, A1, A2, A3, A4, A5, A6, A7, A8> func9)
		{
			return func9.Method;
		}

		#endregion
	}
}