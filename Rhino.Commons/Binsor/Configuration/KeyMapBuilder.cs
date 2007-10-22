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


using System.Collections;
using Castle.Core.Configuration;

namespace Rhino.Commons.Binsor.Configuration
{
	public class KeyMapBuilder : IConfigurationBuilder
	{
		private readonly string _name;
		private string _key = "key";
		private string _item = "item";

		public KeyMapBuilder()
		{
		}

		public KeyMapBuilder(string name)
		{
			_name = name;
		}

		public string key
		{
			get { return _key; }
			set { _key = value; }
		}

		public string item
		{
			get { return _item; }
			set { _item = value; }
		}

		public void Build(IConfiguration parent, object value)
		{
			IDictionary map = (IDictionary)value;

			if (!string.IsNullOrEmpty(_name))
			{
				IConfiguration config = ConfigurationHelper.CreateChild(parent, _name, null);
				parent = config;
			}

			foreach (DictionaryEntry entry in map)
			{
				IConfigurationBuilder builder = entry.Key as IConfigurationBuilder;
				if (builder != null)
				{
					builder.Build(parent, entry.Value);
				}
				else
				{
					Build(parent, entry);
				}
			}
		}

		protected virtual void Build(IConfiguration parent, DictionaryEntry entry)
		{
			string keyName = entry.Key.ToString();
			ConfigurationHelper.SetConfigurationValue(parent, _item, entry.Value, null, false);
			IConfiguration child = parent.Children[parent.Children.Count - 1];
			child.Attributes[_key] = keyName;
		}
	}

	public sealed class keymap : KeyMapBuilder
	{
		public keymap()
		{
		}

		public keymap(string name) : base(name)
		{
		}
	}
}
