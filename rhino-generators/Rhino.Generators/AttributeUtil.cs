using DDW;

namespace Rhino.Generators
{
	public static class AttributeUtil
	{
		public static bool HasAttribute(BaseNode node, string attName)
		{
			return GetAttribute(node, attName) != null;
		}

		public static AttributeNode GetAttribute(BaseNode node, string attName)
		{
			foreach (AttributeNode attributeNode in node.Attributes)
			{
				string nodeAttName = ASTHelper.GetName(attributeNode.Name);
				if (nodeAttName == attName ||
				    nodeAttName == attName + "Attribute")
					return attributeNode;
			}
			return null;
		}
	}
}