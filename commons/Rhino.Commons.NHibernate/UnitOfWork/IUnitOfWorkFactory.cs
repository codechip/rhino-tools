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
using System.Collections;
using System.Data;
using NHibernate;

namespace Rhino.Commons
{
    public interface IUnitOfWorkFactory : IDisposable 
	{
		/// <summary>
		/// Initialize the factory, note that this may be called more than once
		/// </summary>
		void Init();

		/// <summary>
		/// Create a new unit of work implementation.
		/// </summary>
		/// <param name="maybeUserProvidedConnection">Possible connection that the user supplied</param>
		/// <param name="previous">Previous unit of work, if existed</param>
		/// <returns>A usable unit of work</returns>
		IUnitOfWorkImplementor Create(IDbConnection maybeUserProvidedConnection, IUnitOfWorkImplementor previous);

        ISession CurrentSession { get; set; }
        
        ISession GetCurrentSessionFor(Type typeOfEntity);
        
        ISession GetCurrentSessionFor(string name);

        IDisposable SetCurrentSessionName(string name);

        void SetCurrentSession(Type typeOfEntity, ISession session);

        /// <summary>
        ///  When using LongConversation UnitOfWorkApplication uses this method to restore the
        ///  conversation between requests
        /// </summary>
        /// <param name="hashtable">the Hashtable to load the unit of work from</param>
        /// <param name="iUoW">the IUnitOfWork that had been restored</param>
        /// <param name="LongConversationId">the Long Conversation Id</param>
	    void LoadUnitOfWorkFromHashtable(Hashtable hashtable, out IUnitOfWork iUoW, out Guid? LongConversationId);

        /// <summary>
        ///  When using LongConversation UnitOfWorkApplication uses this method to store the
        ///  conversation between requests
        /// </summary>
		/// <param name="hashtable">the Hashtable to save the unit of work to</param>
	    void SaveUnitOfWorkToHashtable(Hashtable hashtable);
	}
}
