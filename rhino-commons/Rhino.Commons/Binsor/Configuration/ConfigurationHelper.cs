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

namespace Rhino.Commons.Binsor.Configuration
{
	public static class ConfigurationHelper
	{
		public static IConfiguration CreateConfiguration(IConfiguration parent, string name,
														 IDictionary dictionary)
		{
			return CreateConfiguration(parent, name, dictionary, "value");
		}

		public static IConfiguration CreateConfiguration(IConfiguration parent, string name,
		                                                 IDictionary dictionary, string valueKey)
		{
			string value = null;

			if (dictionary != null && !string.IsNullOrEmpty(valueKey))
			{
				object valueItem = dictionary[valueKey];
				if (valueItem != null)
				{
					value = valueItem.ToString();
					dictionary.Remove(valueKey);
				}
			}

			if (!string.IsNullOrEmpty(name))
			{
				parent = CreateChild(parent, name, value);
			}

			if (dictionary != null)
			{
				foreach(DictionaryEntry entry in dictionary)
				{
					IConfigurationBuilder builder = entry.Key as IConfigurationBuilder;
					if (builder != null)
					{
						builder.Build(parent, entry.Value);	
					}
					else
					{
						bool useAttribute;
						string key = ExtractKey(entry.Key, out useAttribute);
						SetConfigurationValue(parent, key, entry.Value, valueKey, useAttribute);
					}
				}
			}

			return parent;
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

		public static IConfiguration CreateChild(IConfiguration parent, string name, string value)
		{
			IConfiguration config = new MutableConfiguration(name, value);

			if (parent != null)
			{
				parent.Children.Add(config);
			}

			return config;
		}

		public static string ExtractKey(object key, out bool isAttribute)
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
}