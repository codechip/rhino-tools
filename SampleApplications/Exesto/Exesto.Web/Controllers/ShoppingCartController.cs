using System.Collections;
using Castle.MonoRail.Framework;

namespace Exesto.Web.Controllers
{
	[Layout("default")]
	public class ShoppingCartController : SmartDispatcherController
	{
		private string[] products = { "Milk", "Honey", "Pie" };

		public void Index()
		{
			PropertyBag["cart"] = Cart;
			PropertyBag["products"] = products;
		}

		public void AddItem(int id)
		{
			PropertyBag["alreadyHadfreeShipping"] = Cart.Count > 4;
			Cart.Add(products[id]);
			PropertyBag["produdct"] = products[id];
			PropertyBag["freeShipping"] = Cart.Count > 4;
			RenderView("AddItem.brailjs");
		}

		public void ClearAllItems()
		{
			PropertyBag["alreadyHadfreeShipping"] = Cart.Count > 4;
			InitCart();
			RenderView("ClearAllItems.brailjs");
		}

		private void InitCart()
		{
			Session["cart"] = new ArrayList();
		}

		private IList Cart
		{
			get
			{
				IList items = (IList)Session["cart"];
				if (items == null)
				{
					InitCart();
					items = (IList)Session["cart"];
				}
				return items;
			}
		}
	}
}