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
using System.IO;
using Castle.Windsor;
using MbUnit.Framework;
using Rhino.Commons.Binsor;

namespace Rhino.Commons.Test.Binsor
{
	using Components;

	[TestFixture]
	public class CalculatorTestCase : BaseTest
	{
		private IWindsorContainer _container;

		[SetUp]
		public override void TestInitialize()
		{
            base.TestInitialize();

			_container = new RhinoContainer();
			string path = Path.GetFullPath(@"Binsor\Calculator.boo");
			Console.WriteLine(path);
			BooReader.Read(_container, path);
		}

		[Test]
		public void CanInjectArraysOfComponentsInConstructor()
		{
			Order order1 = new Order();
			order1.CountryCode = "NZ";
			order1.Items.Add(new OrderItem("water", 10, 1.0m, false));
			order1.Items.Add(new OrderItem("glass", 5, 20.0m, true));

			Order order2 = new Order();
			order2.CountryCode = "US";
			order2.Items.Add(new OrderItem("sand", 50, 0.2m, false));

			ICostCalculator costCalculator = _container.Resolve<ICostCalculator>();
			Assert.AreEqual(110m, costCalculator.CalculateTotal(order1));
			Assert.AreEqual(10m, costCalculator.CalculateTotal(order2));
		}

		[Test]
		public void CanInjectArraysOfComponentsInConstructorUsingParameters()
		{
			Order order1 = new Order();
			order1.CountryCode = "NZ";
			order1.Items.Add(new OrderItem("water", 10, 1.0m, false));
			order1.Items.Add(new OrderItem("glass", 5, 20.0m, true));

			Order order2 = new Order();
			order2.CountryCode = "US";
			order2.Items.Add(new OrderItem("sand", 50, 0.2m, false));

			ICostCalculator costCalculator = _container.Resolve<ICostCalculator>("costCalculator.default2");
			Assert.AreEqual(110m, costCalculator.CalculateTotal(order1));
			Assert.AreEqual(10m, costCalculator.CalculateTotal(order2));
		}

		[Test]
		public void CanInjectArraysOfComponentsInConstructor2()
		{
			Order order1 = new Order();
			order1.CountryCode = "NZ";
			order1.Items.Add(new OrderItem("water", 10, 1.0m, false));
			order1.Items.Add(new OrderItem("glass", 5, 20.0m, true));

			Order order2 = new Order();
			order2.CountryCode = "US";
			order2.Items.Add(new OrderItem("sand", 50, 0.2m, false));

			ICostCalculator costCalculator = _container.Resolve<ICostCalculator>("cost_calculator_default");
			Assert.AreEqual(110m, costCalculator.CalculateTotal(order1));
			Assert.AreEqual(10m, costCalculator.CalculateTotal(order2));
		}

		[Test]
		public void CanInjectArraysOfComponentsInConstructorUsingParameters2()
		{
			Order order1 = new Order();
			order1.CountryCode = "NZ";
			order1.Items.Add(new OrderItem("water", 10, 1.0m, false));
			order1.Items.Add(new OrderItem("glass", 5, 20.0m, true));

			Order order2 = new Order();
			order2.CountryCode = "US";
			order2.Items.Add(new OrderItem("sand", 50, 0.2m, false));

			ICostCalculator costCalculator = _container.Resolve<ICostCalculator>("cost_calculator_default2");
			Assert.AreEqual(110m, costCalculator.CalculateTotal(order1));
			Assert.AreEqual(10m, costCalculator.CalculateTotal(order2));
		}
	}
}