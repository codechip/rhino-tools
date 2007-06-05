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


using Castle.ActiveRecord;

[assembly: HqlNamedQuery("AllCats", "from Cat")]

namespace NHibernate.Query.Generator.Tests.ActiveRecord
{
	

	[ActiveRecord(DiscriminatorValue = "C", DiscriminatorColumn = "subclass", DiscriminatorType = "String")]
	public   class Cat : ActiveRecordBase<Cat>
	{
		[Field(Column = "subclass", Update = false, Insert = false, Access = PropertyAccess.Field)]
		protected string subclass = "C";

		int id;

		[PrimaryKey]
		public virtual  int Id
		{
			get { return id; }
			set { id = value; }
		}
	}

	[ActiveRecord(DiscriminatorValue = "D")]
	public   class DomesticCat : Cat
	{
		public   DomesticCat()
		{
			subclass = "D";
		}

		[Property]
		public virtual  string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		private string _name;
	}
}
