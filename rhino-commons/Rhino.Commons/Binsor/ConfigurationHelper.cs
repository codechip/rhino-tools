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
			string value = null;

			if (dictionary != null && dictionary.Contains("_"))
			{
				value = dictionary["_"].ToString();
				dictionary.Remove("_");
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
						string configName = entry.Key.ToString();
						SetConfigurationValue(config, configName, entry.Value, true);
					}
				}
			}

			return config;
		}

		public static IConfiguration SetConfigurationValue(IConfiguration config, string name,
		                                                   object value, bool useAttribute)
		{
			if (value is IDictionary)
			{
				return CreateConfiguration(config, name, (IDictionary) value);
			}
			else if (value is ICollection)
			{
				IConfiguration list = CreateChild(config, name, null);

				foreach(object item in (ICollection) value)
				{
					SetConfigurationValue(list, "item", item, false);
				}

				return list;
			}
			else
			{
				string valueStr = string.Empty;

				if (value is Component)
				{
					value = new ComponentReference((Component) value);
				}

				if (value is bool)
				{
					valueStr = value.ToString().ToLower();
				}
				else if (useAttribute && value is Component)
				{
					valueStr = ((ComponentReference)value).Name;
				}
				else if (useAttribute && value is ComponentReference)
				{
					valueStr = ((ComponentReference) value).Name;
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
					return CreateChild(config, name, valueStr);
				}
			}

			return config;
		}

		public static void ConvertDependencyToConfiguration(IConfiguration config, string name, object value)
		{
			if (value is IDictionary)
			{
				new keymap(name).Build(config, value);
			}
			else if (value is ICollection)
			{
				new list(name).Build(config, value);
			}
			else
			{
				SetConfigurationValue(config, name, value, false);
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
	}

	public interface IConfigurationBuilder
	{
		void Build(IConfiguration config, object value);
	}

	#region child Builder

	public class child : IConfigurationBuilder
	{
		private readonly string _name;

		public child(string name)
		{
			_name = name;
		}

		public void Build(IConfiguration config, object value)
		{
			ConfigurationHelper.SetConfigurationValue(config, _name, value, false);
		}
	}

	#endregion

	#region list Builder

	public class list : IConfigurationBuilder
	{
		private readonly string _name;
		private string _root;
		private string _item = "item";

		public list(string name)
		{
			_name = name;
		}

		public string root
		{
			set { _root = value; }
		}

		public string item
		{
			set { _item = value; }
		}

		public void Build(IConfiguration config, object value)
		{
			ICollection list = value as ICollection;
			if (list == null)
			{
				throw new ArgumentException("An ICollection is expected");
			}

			string rootName = ObtainRootName(list);
			IConfiguration child = ConfigurationHelper.CreateChild(config, _name, null);
			IConfiguration container = ConfigurationHelper.CreateChild(child, rootName, null);

			foreach (object listItem in (ICollection)value)
			{
				ConfigurationHelper.SetConfigurationValue(container, _item, listItem, false);
			}
		}

		private string ObtainRootName(ICollection collection)
		{
			if (!string.IsNullOrEmpty(_root))
			{
				return _root;
			}

			if (collection is Array)
			{
				return "array";
			}

			return "list";
		}
	}

	#endregion

	#region keymap Builder

	public class keymap : IConfigurationBuilder
	{
		private readonly string _name;
		private string _root = "map";
		private string _item = "add";
		private string _key = "key";

		public keymap(string name)
		{
			_name = name;	
		}

		public string root
		{
			get { return _root; }
			set { _root = value; }
		}

		public string item
		{
			get { return _item; }
			set { _item = value; }
		}

		public string key
		{
			get { return _key; }
			set { _key = value; }
		}

		public void Build(IConfiguration config, object value)
		{
			IDictionary dictionary = value as IDictionary;
			if (dictionary == null)
			{
				throw new ArgumentException("An IDictionary is expected");
			}

			IConfiguration map = ConfigurationHelper.CreateChild(config, _name, null);

			if (!string.IsNullOrEmpty(_root))
			{
				map = ConfigurationHelper.CreateChild(map, _root, null);
			}

			foreach (DictionaryEntry entry in dictionary)
			{
				Build(map, entry);
			}
		}

		protected virtual void Build(IConfiguration config, DictionaryEntry entry)
		{
			string keyName = entry.Key.ToString();
			IConfiguration child = ConfigurationHelper.SetConfigurationValue(
				config, _item, entry.Value, false);
			child.Attributes[_key] = keyName;		
		}
	}

	#endregion

	#region keyvalues Builder

	public class keyvalues : keymap
	{
		private string _value = "value";

		public keyvalues(string name)
			: base(name)
		{
		}

		public string value
		{
			get { return _value; }
			set { _value = value; }
		}

		protected override void Build(IConfiguration config, DictionaryEntry entry)
		{
			IConfiguration child = ConfigurationHelper.CreateChild(config, item, null);
			child.Attributes[key] = entry.Key.ToString();
			ConfigurationHelper.SetConfigurationValue(child, value, entry.Value, true);
		}
	}

	#endregion
}