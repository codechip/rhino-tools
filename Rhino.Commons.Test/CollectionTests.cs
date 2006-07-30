using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace Rhino.Commons.Test
{
    [TestFixture]
    public class CollectionTests
    {
        [Test]
        public void ForEachTest()
        {
            int count = 0;
            int[] arr = { 1, 2, 3, 4 };
            Collection.ForEach(arr, delegate(int i)
                                    {
                                        count += 1;
                                        Assert.AreEqual(count, i);
                                    });
            Assert.AreEqual(4, count);
        }
        
        [Test]
        public void GenericFirstTest()
        {
            int[] arr = { 1, 2, 3, 4 };
            Assert.AreEqual(1, Collection.First(arr));
        }

        [Test]
        public void GenericFirstTest_NotList()
        {
            Dictionary<int, int> dic = new Dictionary<int, int>();
            dic.Add(1, 2);
            KeyValuePair<int, int> expected= new KeyValuePair<int, int>(1,2);
            Assert.AreEqual(expected, Collection.First(dic));
        }

        [Test]
        [ExpectedException(typeof(ElementNotfoundException))]
        public void FirstTestOnEmptyCollection_NotList()
        {
            Dictionary<int, int> dic = new Dictionary<int, int>();
            Collection.First(dic);
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void FirstTestOnEmptyCollection()
        {
            int []dic = {};
            Collection.First(dic);
        }

        
        [Test]
        public void SelectAllOdds()
        {
            int[] arr = { 1, 2, 3, 4, 5, 6, 7, 8 };
            int[] expected = { 1, 3, 5, 7 };
            ICollection<int> odds = Collection.SelectAll(arr, delegate(int i) { return i % 2 != 0; });
            AssertCollectionEquals(expected, odds);
        }

        [Test]
        public void SelectAllNotOdds()
        {
            int[] arr = { 1, 2, 3, 4, 5, 6, 7, 8 };
            int[] expected = { 2, 4, 6, 8 };
            ICollection<int> odds = Collection.SelectAllNot(arr, delegate(int i) { return i % 2 != 0; });
            AssertCollectionEquals(expected, odds);
        }

        private void AssertCollectionEquals(ICollection<int> expected, ICollection<int> actual)
        {
            Assert.AreEqual(expected.Count, actual.Count);
            IEnumerator<int> expectedEnum = expected.GetEnumerator();
            IEnumerator<int> actualEnum = actual.GetEnumerator();
            while(expectedEnum.MoveNext())
            {
                actualEnum.MoveNext();//same size, don't need to check it
                Assert.AreEqual(expectedEnum.Current, actualEnum.Current);
            }
        }
    }
}
