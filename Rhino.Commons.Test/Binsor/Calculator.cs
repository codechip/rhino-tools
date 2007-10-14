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


using System.Collections.Generic;

namespace Rhino.Commons.Test.Components
{
	public class Order
	{
		private readonly List<OrderItem> items = new List<OrderItem>();
		private string countryCode;


		public List<OrderItem> Items
		{
			get { return items; }
		}

		public string CountryCode
		{
			get { return countryCode; }
			set { countryCode = value; }
		}
	}

	public class OrderItem
	{
		private decimal costPerItem;
		private bool isFragile;
		private string name;
		private int quantity;

		public OrderItem(string name, int quantity, decimal costPerItem, bool isFragile)
		{
			this.name = name;
			this.quantity = quantity;
			this.costPerItem = costPerItem;
			this.isFragile = isFragile;
		}

		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		public bool IsFragile
		{
			get { return isFragile; }
			set { isFragile = value; }
		}

		public int Quantity
		{
			get { return quantity; }
			set { quantity = value; }
		}

		public decimal CostPerItem
		{
			get { return costPerItem; }
			set { costPerItem = value; }
		}
	}


	public abstract class AbstractCalculator
	{
		public abstract decimal Calculate(decimal currentTotal, Order order);
	}

	public class TotalCalculator : AbstractCalculator
	{
		private static decimal CalculateTotal(Order order)
		{
			decimal total = 0;
			foreach (OrderItem item in order.Items)
			{
				total += (item.Quantity * item.CostPerItem);
			}
			return total;
		}

		public override decimal Calculate(decimal currentTotal, Order order)
		{
			return currentTotal + CalculateTotal(order);
		}
	}

	public class GstCalculator : AbstractCalculator
	{
		private decimal _gstRate = 1.125m;

		public decimal GstRate
		{
			get { return _gstRate; }
			set { _gstRate = value; }
		}

		private static bool IsNewZealand(Order order)
		{
			return (order.CountryCode == "NZ");
		}

		public override decimal Calculate(decimal currentTotal, Order order)
		{
			if (IsNewZealand(order))
			{
				return (currentTotal * _gstRate);
			}
			return currentTotal;
		}
	}

	public class ShippingCalculator : AbstractCalculator
	{
		private decimal _fragileShippingPremium = 1.5m;
		private decimal _shippingCost = 5.0m;

		public decimal ShippingCost
		{
			get { return _shippingCost; }
			set { _shippingCost = value; }
		}

		public decimal FragileShippingPremium
		{
			get { return _fragileShippingPremium; }
			set { _fragileShippingPremium = value; }
		}

		private decimal GetShippingTotal(Order order)
		{
			decimal shippingTotal = 0;

			foreach (OrderItem item in order.Items)
			{
				decimal itemShippingCost = ShippingCost * item.Quantity;
				if (item.IsFragile)
				{
					itemShippingCost *= FragileShippingPremium;
				}
				shippingTotal += itemShippingCost;
			}
			return shippingTotal;
		}

		public override decimal Calculate(decimal currentTotal, Order order)
		{
			return currentTotal + GetShippingTotal(order);
		}
	}

	public interface ICostCalculator
	{
		decimal CalculateTotal(Order order);
	}

	public class DefaultCostCalculator : ICostCalculator
	{
		private readonly AbstractCalculator[] _calculators;

		public DefaultCostCalculator(AbstractCalculator[] calculators)
		{
			_calculators = calculators;
		}

		public decimal CalculateTotal(Order order)
		{
			decimal currentTotal = 0;

			foreach (AbstractCalculator calculator in _calculators)
			{
				currentTotal = calculator.Calculate(currentTotal, order);
			}
			return currentTotal;
		}
	}
}