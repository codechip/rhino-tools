namespace Rhino.Commons.Test.Binsor
{
	using Castle.Core.Configuration;
	using MbUnit.Framework;

	public class ConfigurationAsserts : BaseTest
	{
		public static void AssertAttribute(IConfiguration parent, string name, string value)
		{
			string attribute = parent.Attributes[name];
			Assert.AreEqual(value, attribute);
		}

		public static IConfiguration AssertChild(IConfiguration parent, string name, string value)
		{
			IConfiguration child = parent.Children[name];
			Assert.IsNotNull(child, "Expected child {0}", name);
			if (value != null)
			{
				Assert.AreEqual(value, child.Value);
			}
			return child;
		}

		public static IConfiguration AssertChild(IConfiguration parent, string name)
		{
			return AssertChild(parent, name, null);
		}

		public static IConfiguration AssertKeyValue(IConfiguration parent, string item,
		                                             string key, string value)
		{
			foreach (IConfiguration child in parent.Children)
			{
				if (item == child.Name && child.Attributes["key"] == key &&
				    child.Value == value)
				{
					return child;
				}
			}

			Assert.Fail("No child {0} found with key={1}, value={2}", item, key, value);
			return null;
		}

		public static IConfiguration AssertKeyValueAttrib(IConfiguration parent, string item,
														   string key, string value)
		{
			foreach (IConfiguration child in parent.Children)
			{
				if (item == child.Name && child.Attributes["key"] == key &&
					child.Attributes["value"] == value)
				{
					return child;
				}
			}

			Assert.Fail("No child {0} found with key={1}, value={2}", item, key, value);
			return null;
		}
		
	}
}