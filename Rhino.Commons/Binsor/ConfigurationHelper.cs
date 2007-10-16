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
using Castle.Core.Configuration;

namespace Rhino.Commons.Binsor
{
	public static class ConfigurationHelper
	{
		public static IConfiguration CreateConfiguration(IConfiguration parent, string name,
														 IDictionary dictionary)
		{
			return CreateConfiguration(parent, name, dictionary, null);
		}

		public static IConfiguration CreateConfiguration(IConfiguration parent, string name,
		                                                 IDictionary dictionary, string valueKey)
		{
			string value = null;

			if (!string.IsNullOrEmpty(valueKey))
			{
				object valueItem = dictionary[valueKey];
				if (valueItem != null)
				{
					value = valueItem.ToString();
					dictionary.Remove(valueKey);
				}
			}

			IConfiguration config = CreateChild(parent, name, value);

			if (dictionary != null)
			{
				foreach(DictionaryEntry entry in dictionary)
				{
					IConfigurationBuilder builder = entry.Key as IConfigurationBuilder;
					if (builder != null)
					{
						builder.Build(config, entry.Value);	
					}
					else
					{
						bool useAttribute;
						string key = ExtractKey(entry.Key, out useAttribute);
						SetConfigurationValue(config, key, entry.Value, valueKey, useAttribute);
					}
				}
			}

			return config;
		}

		public static void SetConfigurationValue(IConfiguration config, string name,
		                                         object value, string valueKey, bool useAttribute)
		{
			bool isAttribute;
			name = ExtractKey(name, out isAttribute);
			useAttribute |= isAttribute;

			if (value is IDictionary)
			{
				CreateConfiguration(config, name, (IDictionary) value, valueKey);
			}
			else if (value is ICollection)
			{
				new list(name).Build(config, value);					
			}
			else if (value is IConfigurationFormatter)
			{
				((IConfigurationFormatter)value).Format(config, name, useAttribute);
			}
			else
			{
				string valueStr = string.Empty;

				if (value is bool)
				{
					valueStr = value.ToString().ToLower();
				}
				else if (value is Type)
				{
					valueStr = ((Type) value).AssemblyQualifiedName;
				}
				else if (value != null)
				{
					valueStr = value.ToString();
				}

				if (useAttribute)
				{
					config.Attributes[name] = valueStr;
				}
				else
				{
					CreateChild(config, name, valueStr);
				}
			}
		}

		public static void ConvertDependencyToConfiguration(IConfiguration config, string name, object value)
		{
			if (value is IDictionary)
			{
				config = CreateChild(config, name, null);
				new keymap("map").Build(config, value);
			}
			else if (value is ICollection)
			{
				config = CreateChild(config, name, null);
				new list("list").Build(config, value);
			}
			else
			{
				SetConfigurationValue(config, name, value, "value", false);
			}
		}

		public static bool RequiresConfiguration(object value)
		{
			if ((value is ComponentReference) || (value is Component))
			{
				return true;
			}

			if (value is IDictionary)
			{
				foreach (object item in ((IDictionary)value).Values)
				{
					if (RequiresConfiguration(item))
					{
						return true;
					}
				}
			}

			if (value is ICollection)
			{
				foreach (object item in (ICollection) value)
				{
					if (RequiresConfiguration(item))
					{
						return true;
					}
				}
			}

			return false;
		}

		internal static IConfiguration CreateChild(IConfiguration parent, string name, string value)
		{
			IConfiguration config = new MutableConfiguration(name, value);

			if (parent != null)
			{
				parent.Children.Add(config);
			}

			return config;
		}

		internal static string ExtractKey(object key, out bool isAttribute)
		{
			isAttribute = false;
			string keyName = key.ToString();

			if (keyName.StartsWith("@"))
			{
				isAttribute = true;
				keyName = keyName.Substring(1);
			}
			else if (key is ComponentReference)
			{
				isAttribute = true;
				keyName = ((ComponentReference)key).Name;
			}

			return keyName;
		}
	}

	public interface IConfigurationFormatter
	{
		void Format(IConfiguration parent, string name, bool useAttribute);
	}

	public interface IConfigurationBuilder
	{
		void Build(IConfiguration parent, object value);
	}

	#region child builder

	public class child : IConfigurationBuilder
	{
		private readonly string _name;
		private string _value = "value";

		public child(string name)
		{
			_name = name;
		}

		public string value
		{
			get { return _value; }
			set { _value = value; }
		}

		public void Build(IConfiguration parent, object value)
		{
			ConfigurationHelper.SetConfigurationValue(parent, _name, value, _value, false);
		}
	}

	#endregion

	#region list builder

	public class list : IConfigurationBuilder
	{
		private readonly string _name;
		private string _item = "item";
		private string _value = "value";

		public list()
		{	
		}

		public list(string name)
		{
			_name = name;
		}

		public string item
		{
			get { return _item; }
			set { _item = value; }
		}

		public string value
		{
			get { return _value; }
			set { _value = value; }
		}

		public void Build(IConfiguration parent, object value)
		{
			ICollection list = (ICollection) value;

			if (!string.IsNullOrEmpty(_name))
			{
				IConfiguration config = ConfigurationHelper.CreateChild(parent, _name, null);
				parent = config;
			}

			foreach (object child in list)
			{
				ConfigurationHelper.SetConfigurationValue(parent, _item, child, _value, false);
			}				
		}
	}

	#endregion

	#region keymap builder

	public class keymap : IConfigurationBuilder
	{
		private readonly string _name;
		private string _key = "key";
		private string _item = "item";

		public keymap()
		{	
		}

		public keymap(string name)
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
			IDictionary map = (IDictionary) value;

			if (!string.IsNullOrEmpty(_name))
			{
				IConfiguration config = ConfigurationHelper.CreateChild(parent, _name, null);
				parent = config;
			}

			foreach (DictionaryEntry entry in map)
			{
				Build(parent, entry);			
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

	#endregion

	#region keyvalues builder

	public class keyvalues : keymap
	{
		private string _value = "value";

		public keyvalues()
		{	
		}

		public keyvalues(string name)
			: base(name)
		{	
		}

		public string value
		{
			get { return _value; }
			set { _value = value; }
		}

		protected override void Build(IConfiguration parent, DictionaryEntry entry)
		{
			IConfiguration child = ConfigurationHelper.CreateChild(parent, item, null);
			child.Attributes[key] = entry.Key.ToString();
			ConfigurationHelper.SetConfigurationValue(child, _value, entry.Value, null, true);
		}
	}

	#endregion
}