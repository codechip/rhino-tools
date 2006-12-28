// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections;
using System.Collections.Specialized;
using System.Reflection;
using System.Text;
using Castle.MonoRail.Framework;
using Castle.MonoRail.Framework.Helpers;

namespace Rhino.Components
{
	public class GridComponent : ViewComponent
	{
		static readonly string[] sections = new string[]
            {
                "header", "footer",
                "pagination", "empty",
                "item", "alternateItem",
                "tablestart", "tableend"
            };

		public override bool SupportsSection(string name)
		{
			return Array.IndexOf(sections, name) != -1;
		}

		public override void Render()
		{
			IPaginatedPage source = (IPaginatedPage)ComponentParams["source"];

			ShowStartTable();
			ShowHeader(source);

			if (source != null && source.TotalItems > 0)
			{
				ShowRows(source);
			}
			else
			{
				ShowEmpty();
			}

			ShowFooter();
			ShowEndTable();
			ShowPagination(source);
		}

		protected virtual void ShowRows(IPaginatedPage source)
		{
			bool hasAlternate = Context.HasSection("alternateItem");
			bool isAlternate = false;
			foreach (object item in source)
			{
				PropertyBag["item"] = item;

				if (hasAlternate && isAlternate)
					Context.RenderSection("alternateItem");
				else
					Context.RenderSection("item");

				isAlternate = !isAlternate;
			}
		}

		private void ShowEmpty()
		{
			if (Context.HasSection("empty"))
			{
				Context.RenderSection("empty");
			}
			else
			{
				RenderText("Grid has not data");
			}
		}

		private void ShowPagination(IPaginatedPage source)
		{
			if (Context.HasSection("pagination"))
			{
				Context.RenderSection("pagination");
				return;
			}
			PaginationHelper paginationHelper = (PaginationHelper)Context.ContextVars["PaginationHelper"];
			StringBuilder output = new StringBuilder();
			output.AppendFormat(@"<div class='pagination'><table><tr><td>
Showing {0} - {1} of {2}
</td><td align='right'>",
				source.FirstItem, source.LastItem, source.TotalItems);
			if (source.HasFirst)
				output.Append(paginationHelper.CreatePageLink(1, "first", null, QueryStringAsDictionary()));
			else
				output.Append("first");

			output.Append(" | ");
			if (source.HasPrevious)
				output.Append(paginationHelper.CreatePageLink(source.PreviousIndex, "prev", null, QueryStringAsDictionary()));
			else
				output.Append("prev");

			output.Append(" | ");
			if (source.HasNext)
				output.Append(paginationHelper.CreatePageLink(source.NextIndex, "next", null, QueryStringAsDictionary()));
			else
				output.Append("next");

			output.Append(" | ");
			if (source.HasLast)
				output.Append(paginationHelper.CreatePageLink(source.LastIndex, "last", null, QueryStringAsDictionary()));
			else
				output.Append("last");

			output.Append(@"</td></tr></table></div>");

			RenderText(output.ToString());
		}

		private IDictionary QueryStringAsDictionary()
		{
			Hashtable table = new Hashtable();
			foreach (string key in Request.QueryString)
			{
				if(key==null)
					continue;
				table[key] = Request.QueryString[key];
			}
			return table;
		}

		private void ShowStartTable()
		{
			if (Context.HasSection("tablestart"))
			{
				Context.RenderSection("tablestart");
			}
			else
			{
				RenderText("<table id='grid'>");
			}
		}

		private void ShowEndTable()
		{
			if (Context.HasSection("tableend"))
			{
				Context.RenderSection("tableend");
			}
			else
			{
				RenderText("</table>");
			}
		}

		private void ShowFooter()
		{
			if (Context.HasSection("footer"))
			{
				Context.RenderSection("footer");
			}
		}

		protected virtual void ShowHeader(IPaginatedPage source)
		{
			if (Context.HasSection("header") == false)
			{
				throw new ViewComponentException("A GridComponent must has a header");
			}
			Context.RenderSection("header");

		}
	}
}