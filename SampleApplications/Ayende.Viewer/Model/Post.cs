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
using Castle.ActiveRecord;
using Iesi.Collections.Generic;

namespace Model
{
	[ActiveRecord]
	public class Post : ActiveRecordBase<Post>
	{
		private string content;
		private DateTime created, modified;
		private Guid postId;
		private string title;
		private ISet<Category> categories = new HashedSet<Category>();

		[Property(NotNull = true, ColumnType = "StringClob", SqlType = "NTEXT")]
		public string Content
		{
			get { return content; }
			set { content = value; }
		}

		[Property(NotNull = true)]
		public DateTime Created
		{
			get { return created; }
			set { created = value; }
		}

		[Property(NotNull = true)]
		public DateTime Modified
		{
			get { return modified; }
			set { modified = value; }
		}

		[PrimaryKey(PrimaryKeyType.Assigned)]
		public Guid PostId
		{
			get { return postId; }
			set { postId = value; }
		}

		[Property(Length = 512, NotNull = true)]
		public string Title
		{
			get { return title; }
			set { title = value; }
		}

		[HasAndBelongsToMany(Table="CategoriesPosts", ColumnKey = "PostId", ColumnRef = "CategoryId", Lazy = true)]
		public ISet<Category> Categories
		{
			get { return categories; }
			set { categories = value; }
		}

		public void AddToCategory(Category category)
		{
			category.Posts.Add(this);
			Categories.Add(category);
		}
	}
}
