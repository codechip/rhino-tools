using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace Rhino.Commons.Test
{
    [TestFixture]
    public class LocalDataTests
    {
        [Test]
        public void CanSaveAndLoadData()
        {
            object key = new object();
            string val = "value";
            Local.Data[key] = val;

            Assert.AreEqual(val, Local.Data[key]);
        }
    }
}
