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
using System.IO;
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
                "tablestart", "tableend",
				"link",
            };

		public override bool SupportsSection(string name)
		{
			foreach (string section in sections)
			{
				if(section.Equals(name,StringComparison.InvariantCultureIgnoreCase))
					return true;
			}
			return false;
		}

		public override void Render()
		{
			IEnumerable source = ComponentParams["source"] as IEnumerable;
			if (source == null)
			{
				throw new ViewComponentException(
					"The grid requires a view component parameter named 'source' which should contain 'IEnumerable' instance");
			}

			ShowStartTable();
			ShowHeader(source);

			ShowRows(source);

			ShowFooter();
			ShowEndTable();

			IPaginatedPage page = source as IPaginatedPage;
			if (page != null)
			{
				ShowPagination(page);
			}
		}

		protected virtual void ShowRows(IEnumerable source)
		{
			bool hasAlternate = Context.HasSection("alternateItem");
			bool isAlternate = false;
			bool hasItems = false;
			foreach (object item in source)
			{
				hasItems = true;
				PropertyBag["item"] = item;

				if (hasAlternate && isAlternate)
					Context.RenderSection("alternateItem");
				else
					Context.RenderSection("item");

				isAlternate = !isAlternate;
			}
			if (!hasItems)
				ShowEmpty();
		}

		private void ShowEmpty()
		{
			if (Context.HasSection("empty"))
			{
				Context.RenderSection("empty");
			}
			else
			{
				RenderText("Grid has no data");
			}
		}

		private void ShowPagination(IPaginatedPage page)
		{
			if (Context.HasSection("pagination"))
			{
				Context.RenderSection("pagination");
				return;
			}
			PaginationHelper paginationHelper = (PaginationHelper)Context.ContextVars["PaginationHelper"];
			StringWriter output = new StringWriter();
			output.Write(@"<div class='pagination'><span class='paginationLeft'>
Showing {0} - {1} of {2} 
</span><span class='paginationRight'>",
				page.FirstItem, page.LastItem, page.TotalItems);
			if (page.HasFirst)
				CreateLink(output, paginationHelper, 1, "first");
			else
				output.Write("first");

			output.Write(" | ");
			if (page.HasPrevious)
				CreateLink(output, paginationHelper, page.PreviousIndex, "prev");
			else
				output.Write("prev");

			output.Write(" | ");
			if (page.HasNext)
				CreateLink(output, paginationHelper, page.NextIndex, "next");
			else
				output.Write("next");

			output.Write(" | ");
			if (page.HasLast)
				CreateLink(output, paginationHelper, page.LastIndex, "last");
			else
				output.Write("last");

			output.Write(@"</span></div>");

			RenderText(output.ToString());
		}

		private void CreateLink(TextWriter output, PaginationHelper paginationHelper, int pageIndex, string title)
		{
			if(Context.HasSection("link"))
			{
				PropertyBag["pageIndex"] = pageIndex;
				PropertyBag["title"] = title;
				Context.RenderSection("link", output);
			}
			else
			{
				output.Write(paginationHelper.CreatePageLink(pageIndex, title, null, QueryStringAsDictionary()));
			}
		}

		private IDictionary QueryStringAsDictionary()
		{
			Hashtable table = new Hashtable();
			foreach (string key in Request.QueryString)
			{
				if (key == null)
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
				RenderText("<table class='grid' cellspacing='0' cellpadding='0'>");
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

		protected virtual void ShowHeader(IEnumerable source)
		{
			if (Context.HasSection("header") == false)
			{
				throw new ViewComponentException("A GridComponent must has a header");
			}
			Context.RenderSection("header");
		}
	}
}
