namespace Rhino.Etl.Tests.Aggregation
{
    using System.Collections.Generic;
    using Core;
    using MbUnit.Framework;

    public class BaseAggregationFixture
    {
        protected List<Row> rows;

        [SetUp]
        public void SetUp()
        {
            rows = new List<Row>();
            AddRow("milk", 15);
            AddRow("milk", 15);
            AddRow("sugar", 10);
            AddRow("sugar", 15);
            AddRow("coffee", 6);
            AddRow("sugar", 3);
        }

        private void AddRow(string name, int price)
        {
            Row row = new Row();
            row["name"] = name;
            row["price"] = price;
            rows.Add(row);
        }
    }
}