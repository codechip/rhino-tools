using ICSharpCode.NRefactory.Ast;

namespace Rhino.Generators
{
	public static class AttributeUtil
	{

		public static bool HasAttribute(AttributedNode node, string attributeName)
		{
			return GetAttribute(node, attributeName)!=null;
		}

		public static Attribute GetAttribute(AttributedNode node, string attributeName)
		{
			foreach (AttributeSection attributeSection in node.Attributes)
			{
				foreach (Attribute attribute in attributeSection.Attributes)
				{
					if(attribute.Name == attributeName || attribute.Name == attributeName+"Attribute")
					{
						return attribute;
					}
				}
			}
			return null;
		}
	}
}