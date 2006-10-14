using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Rhino.Commons.NHibernate
{
	/// <summary>
	/// This class was originally boo code in MonoRail Generator, and was originally-originally inspired
	/// by RoR's inflector.
	/// </summary>
	public sealed class Inflector
	{
		public static Inflector Instnace = new Inflector();
		public Inflector()
		{
			_plurals = new List<Inflection>();
			_singulars = new List<Inflection>();
			_uncountables = new List<string>();

			AddPluralInflections(new Inflection[]
			                     	{
			                     		new Inflection(new Regex("$"), "s"),
			                     		new Inflection(new Regex("(?i)s$"), "s"),
			                     		new Inflection(new Regex("(?i)(ax|test)is$"), "$1es"),
			                     		new Inflection(new Regex("(?i)(octop|vir)us$"), "$1i"),
			                     		new Inflection(new Regex("(?i)(alias|status)$"), "$1es"),
			                     		new Inflection(new Regex("(?i)(bu)s$"), "$1ses"),
			                     		new Inflection(new Regex("(?i)(buffal|tomat)o$"), "$1oes"),
			                     		new Inflection(new Regex("(?i)([ti])um$"), "$1a"),
			                     		new Inflection(new Regex("(?i)sis$"), "ses"),
			                     		new Inflection(new Regex("(?i)(?:([^f])fe|([lr])f)$"), "$1$2ves"),
			                     		new Inflection(new Regex("(?i)(hive)$"), "$1s"),
			                     		new Inflection(new Regex("(?i)([^aeiouy]|qu)y$"), "$1ies"),
			                     		new Inflection(new Regex("(?i)([^aeiouy]|qu)ies$"), "$1y"),
			                     		new Inflection(new Regex("(?i)(x|ch|ss|sh)$"), "$1es"),
			                     		new Inflection(new Regex("(?i)(matr|vert|ind)ix|ex$"), "$1ices"),
			                     		new Inflection(new Regex("(?i)([m|l])ouse$"), "$1ice"),
			                     		new Inflection(new Regex("(?i)^(ox)$"), "$1en"),
			                     		new Inflection(new Regex("(?i)(quiz)$"), "$1zes")
			                     	});
			AddSingularInflections(new Inflection[]
			                       	{
			                       		new Inflection(new Regex("(?i)s$"), ""),
			                       		new Inflection(new Regex("(?i)(n)ews$"), "$1ews"),
			                       		new Inflection(new Regex("(?i)([ti])a$"), "$1um"),
			                       		new Inflection(new Regex("(?i)((a)naly|(b)a|(d)iagno|(p)arenthe|(p)rogno|(s)ynop|(t)he)ses$"), "$1$2sis"),
			                       		new Inflection(new Regex("(?i)(^analy)ses$"), "$1sis"),
			                       		new Inflection(new Regex("(?i)([^f])ves$"), "$1fe"),
			                       		new Inflection(new Regex("(?i)(hive)s$"), "$1"),
			                       		new Inflection(new Regex("(?i)(tive)s$"), "$1"),
			                       		new Inflection(new Regex("(?i)([lr])ves$"), "$1f"),
			                       		new Inflection(new Regex("(?i)([^aeiouy]|qu)ies$"), "$1y"),
			                       		new Inflection(new Regex("(?i)(s)eries$"), "$1eries"),
			                       		new Inflection(new Regex("(?i)(m)ovies$"), "$1ovie"),
			                       		new Inflection(new Regex("(?i)(x|ch|ss|sh)es$"), "$1"),
			                       		new Inflection(new Regex("(?i)([m|l])ice$"), "$1ouse"),
			                       		new Inflection(new Regex("(?i)(bus)es$"), "$1"),
			                       		new Inflection(new Regex("(?i)(o)es$"), "$1"),
			                       		new Inflection(new Regex("(?i)(shoe)s$"), "$1"),
			                       		new Inflection(new Regex("(?i)(cris|ax|test)es$"), "$1is"),
			                       		new Inflection(new Regex("(?i)([octop|vir])i$"), "$1us"),
			                       		new Inflection(new Regex("(?i)(alias|status)es$"), "$1"),
			                       		new Inflection(new Regex("(?i)^(ox)en"), "$1"),
			                       		new Inflection(new Regex("(?i)(vert|ind)ices$"), "$1ex"),
			                       		new Inflection(new Regex("(?i)(matr)ices$"), "$1ix"),
			                       		new Inflection(new Regex("(?i)(quiz)zes$"), "$1")
			                       	});
			AddIrregularInflections(new Inflection[]
			                        	{
			                        		new Inflection("person", "people"),
			                        		new Inflection("man", "men"),
			                        		new Inflection("child", "children"),
			                        		new Inflection("sex", "sexes"),
			                        		new Inflection("move", "moves")
			                        	});
			AddUncountables(new string[]
			                	{
			                		"equipment", 
									"information", 
									"rice", 
									"money", 
									"species", 
									"series", 
									"fish", 
									"sheep"
								});
		}

		public void AddIrregularInflections(ICollection<Inflection> inflections)
		{
			_plurals.AddRange(inflections);
			foreach (Inflection inflection in inflections)
			{
				_singulars.Add(new Inflection(inflection.Replace, inflection.Pattern));
			}
		}

		public void AddPluralInflections(IEnumerable<Inflection> inflections)
		{
			_plurals.AddRange(inflections);
		}

		public void AddSingularInflections(IEnumerable<Inflection> inflections)
		{
			_singulars.AddRange(inflections);
		}

		public void AddUncountables(string[] words)
		{
			_uncountables.AddRange(words);
		}

		public bool IsUncountable(string word)
		{
			return _uncountables.Contains(word.ToLower());
		}

		public string ToPlural(string word)
		{
			if (!IsUncountable(word))
			{
				foreach (Inflection inflection1 in _plurals)
				{
					if (inflection1.IsMatch(word))
					{
						return inflection1.Apply(word);
					}
				}
			}
			return word;
		}

		public string ToSingular(string word)
		{
			if (!IsUncountable(word))
			{
				foreach (Inflection inflection1 in _singulars)
				{
					if (inflection1.IsMatch(word))
					{
						return inflection1.Apply(word);
					}
				}
			}
			return word;
		}

		private List<Inflection> _plurals;
		private List<Inflection> _singulars;
		private List<string> _uncountables;
	}


	[Serializable]
	public class Inflection
	{
		public Inflection(object pattern, object replace)
		{
			_pattern = pattern;
			_replace = (string)replace;
		}

		public string Apply(string str)
		{
			if (_pattern is string)
			{
				return _replace;
			}
			return (_pattern as Regex).Replace(str, _replace);
		}

		public bool IsMatch(string str)
		{
			if (_pattern is string)
			{
				return (string.Compare((string)_pattern, str, true) == 0);
			}
			return (_pattern as Regex).IsMatch(str);
		}


		public object Pattern
		{
			get { return _pattern; }
			set { _pattern = value; }
		}

		public string Replace
		{
			get { return _replace; }
			set { _replace = value; }
		}


		private object _pattern;
		private string _replace;
	}
}