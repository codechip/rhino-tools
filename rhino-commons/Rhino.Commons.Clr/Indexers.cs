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
using System.Collections.Generic;
using System.Text;

namespace Rhino.Commons
{
	/// <summary>
	/// Allow to add named indexers to language like C#, that doesn't
	/// support them. 
	/// </summary>
	/// <typeparam name="RetType">The type of the return type.</typeparam>
	/// <typeparam name="IndexType">The type of the index type.</typeparam>
    public class PropertyIndexer<RetType, IndexType>
    {
        private readonly Func<RetType, IndexType> getter;
        private readonly Proc<IndexType, RetType> setter;

		/// <summary>
		/// Initializes a new instance of the <see cref="PropertyIndexer&lt;RetType, IndexType&gt;"/> class.
		/// </summary>
		/// <param name="getter">The getter.</param>
		/// <param name="setter">The setter.</param>
        public PropertyIndexer(Func<RetType, IndexType> getter, Proc<IndexType, RetType> setter)
        {
            this.getter = getter;
            this.setter = setter;
        }

		/// <summary>
		/// Gets or sets the RetType at the specified index.
		/// </summary>
		/// <value></value>
        public RetType this[IndexType index]
        {
            get { return getter(index); }
            set { setter(index, value); }
        }
    }

	/// <summary>
	/// Allow to add named getter indexers to language like C#, that doesn't
	/// support them. 
	/// </summary>
	/// <typeparam name="RetType">The type of the ret type.</typeparam>
	/// <typeparam name="IndexType">The type of the index type.</typeparam>
    public class PropertyIndexerGetter<RetType, IndexType>
    {
        private readonly Func<RetType, IndexType> getter;

		/// <summary>
		/// Initializes a new instance of the <see cref="PropertyIndexerGetter&lt;RetType, IndexType&gt;"/> class.
		/// </summary>
		/// <param name="getter">The getter.</param>
        public PropertyIndexerGetter(Func<RetType, IndexType> getter)
        {
            this.getter = getter;
        }

		/// <summary>
		/// Gets the RetType at the specified index.
		/// </summary>
		/// <value></value>
        public RetType this[IndexType index]
        {
            get { return getter(index); }
        }
    }

    
	/// <summary>
	/// Allow to add named setter indexers to language like C#, that doesn't
	/// support them. 
	/// </summary>
	/// <typeparam name="RetType">The type of the ret type.</typeparam>
	/// <typeparam name="IndexType">The type of the index type.</typeparam>
	public class PropertyIndexerSetter<RetType, IndexType>
    {
        private readonly Proc<IndexType, RetType> setter;

		/// <summary>
		/// Initializes a new instance of the <see cref="PropertyIndexerSetter&lt;RetType, IndexType&gt;"/> class.
		/// </summary>
		/// <param name="setter">The setter.</param>
        public PropertyIndexerSetter(Proc<IndexType, RetType> setter)
        {
            this.setter = setter;
        }

		/// <summary>
		/// Sets the RetType with the specified ERROR.
		/// </summary>
		/// <value></value>
        public RetType this[IndexType index]
        {
            set { setter(index, value); }
        }
    }
}
