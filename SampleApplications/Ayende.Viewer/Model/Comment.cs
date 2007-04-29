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
using Castle.ActiveRecord;

namespace Model
{
	[ActiveRecord]
	public class Comment : ActiveRecordBase<Comment>
	{
		private Guid commentId;
		private Post post;
		private DateTime created;
		private DateTime modified;
		private string author;
		private string email;
		private string homepage;
		private string content;

		[PrimaryKey(PrimaryKeyType.Assigned)]
		public Guid CommentId
		{
			get { return commentId; }
			set { commentId = value; }
		}

		[BelongsTo(NotNull = true)]
		public Post Post
		{
			get { return post; }
			set { post = value; }
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

		[Property(NotNull = true)]
		public string Author
		{
			get { return author; }
			set { author = value; }
		}

		[Property(NotNull = true)]
		public string Email
		{
			get { return email; }
			set { email = value; }
		}

		[Property(NotNull = true)]
		public string HomePage
		{
			get { return homepage; }
			set { homepage = value; }
		}

		[Property(NotNull = true, Length = 8000)]
		public string Content
		{
			get { return content; }
			set { content = value; }
		}
	}
}