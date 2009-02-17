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
	/// <summary>
	/// Base class for the future of a query, when you try to access the real
	/// result of the query than all the future queries in the current context
	/// (current thread / current request) are executed as a single batch and all
	/// their results are loaded in a single round trip.
	/// </summary>
	public class FutureBase
	{
		private const string cacheKey = "future.of.entity.batch.key";
		private bool wasLoaded;

		/// <summary>
		/// Gets a value indicating whether this instance was loaded.
		/// </summary>
		/// <value><c>true</c> if this query was loaded; otherwise, <c>false</c>.</value>
		protected bool WasLoaded
		{
			get { return wasLoaded; }
			set { wasLoaded = value; }
		}

		/// <summary>
		/// Gets the batcher.
		/// </summary>
		/// <value>The batcher.</value>
		protected static CriteriaBatch Batcher
		{
			get
			{
				CriteriaBatch current = (CriteriaBatch)Local.Data[cacheKey];
				if (current == null)
					Local.Data[cacheKey] = current = new CriteriaBatch();
				return current;
			}
		}

		/// <summary>
		/// Execute all the queries in the batch.
		/// </summary>
		protected void ExecuteBatchQuery()
		{
			Batcher.Execute(UnitOfWork.CurrentSession);
			wasLoaded = true;
			ClearBatcher();
		}

		/// <summary>
		/// Clears the batcher.
		/// </summary>
		private static void ClearBatcher()
		{
			Local.Data[cacheKey] = null;
		}
	}
}