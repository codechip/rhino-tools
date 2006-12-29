using System.Collections;
using System.Reflection;
using Castle.MonoRail.Framework.Helpers;

namespace Rhino.Components
{
	public class GridComponentWithAutoGenerateColumns : GridComponent
	{
		private PropertyInfo[] properties;

		protected override void ShowRows(IEnumerable source)
		{
			if (properties == null)//there are no rows, if this is the case
				return;
			bool isAlternate = false;
			foreach (object item in source)
			{
				if (isAlternate)
					RenderText("<tr id='alternateItem'>");
				else
					RenderText("<tr id='item'>");
				foreach (PropertyInfo info in properties)
				{
					RenderText("<td>");
					object val = info.GetValue(item, null) ?? "null";
					RenderText(val.ToString());
					RenderText("</td>");
				}
				isAlternate = !isAlternate;
				RenderText("</tr>");
			}
		}

		protected override void ShowHeader(IEnumerable source)
		{
			IEnumerator enumerator = source.GetEnumerator();
			bool hasItem = enumerator.MoveNext();
			if(hasItem==false)
			{
				return;
			}
			object first = enumerator.Current;
			properties = first.GetType().GetProperties();
			foreach (PropertyInfo property in this.properties)
			{
				RenderText("<th id='header'>");
				RenderText(property.Name);
				RenderText("</th>");
			}
		}
	}
}