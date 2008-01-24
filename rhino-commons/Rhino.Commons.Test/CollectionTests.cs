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
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework;
using Rhino.Commons.Exceptions;

namespace Rhino.Commons.Test
{
	[TestFixture]
	public class CollectionTests
	{
		[Test]
		public void ForEachTest()
		{
			int count = 0;
			int[] arr = {1, 2, 3, 4};
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
			int[] arr = {1, 2, 3, 4};
			Assert.AreEqual(1, Collection.First(arr));
		}

		[Test]
		public void GenericFirstTest_NotList()
		{
			Dictionary<int, int> dic = new Dictionary<int, int>();
			dic.Add(1, 2);
			KeyValuePair<int, int> expected = new KeyValuePair<int, int>(1, 2);
			Assert.AreEqual(expected, Collection.First(dic));
		}

		[Test]
		[ExpectedException(typeof (ElementNotFoundException))]
		public void FirstTestOnEmptyCollection_NotList()
		{
			Dictionary<int, int> dic = new Dictionary<int, int>();
			Collection.First(dic);
		}

		[Test]
		[ExpectedException(typeof (ArgumentOutOfRangeException))]
		public void FirstTestOnEmptyCollection()
		{
			int[] dic = {};
			Collection.First(dic);
		}

		[Test]
		public void Map_WillTransfromAllInput_ToOutput()
		{
			IList<int> map = Collection.Map<string, int>(
				new string[] {"1", "2", "3"}, delegate(string s)
				{
					return int.Parse(s);
				});
			AssertCollectionEquals(new Int32[]{1,2,3}, map);
		}

		[Test]
		public void Reduce_WillAllowToWrite_Sum()
		{
			int total = Collection.Reduce(Collection.Containing(1,2,3,4,5,6,7,8,9),0,delegate(int i, int sum)
			{
				return i + sum;
			});
			Assert.AreEqual(45, total);
		}

		[Test]
		public void SelectAllOdds()
		{
			int[] arr = {1, 2, 3, 4, 5, 6, 7, 8};
			int[] expected = {1, 3, 5, 7};
			ICollection<int> odds = Collection.SelectAll(arr, delegate(int i)
			{
				return i%2 != 0;
			});
			AssertCollectionEquals(expected, odds);
		}

		[Test]
		public void SelectAllNotOdds()
		{
			int[] arr = {1, 2, 3, 4, 5, 6, 7, 8};
			int[] expected = {2, 4, 6, 8};
			ICollection<int> odds = Collection.SelectAllNot(arr, delegate(int i)
			{
				return i%2 != 0;
			});
			AssertCollectionEquals(expected, odds);
		}


		[Test]
		public void ToUniqueCollection_EmptyCollectionReturnsNewEmptyCollection()
		{
			ICollection<int> input = new List<int>();
			ICollection<int> actual = Collection.ToUniqueCollection(input);
			Assert.AreNotSame(actual, input);
			Assert.AreEqual(0, actual.Count);
		}


		[Test]
		public void ToUniqueCollection_NullCollectionReturnsNullCollection()
		{
			ICollection<int> actual = Collection.ToUniqueCollection<int>(null);
			Assert.IsNull(actual);
		}


		[Test]
		public void ToUniqueCollection_ReturnsGenericListImplementation()
		{
			ICollection<int> input = new List<int>();
			Assert.IsInstanceOfType(typeof (List<int>), Collection.ToUniqueCollection(input));
		}


		[Test]
		public void ToUniqueCollection_WillRemoveDuplicatePrimatives()
		{
			ICollection<int> input = ListContaining(1, 2, 2);
			AssertCollectionEquals(ListContaining(1, 2), Collection.ToUniqueCollection(input));
		}

		[Test]
		public void ToUniqueCollection_WillRemoveDuplicatePrimativesStoredAsObjects()
		{
			object o1 = 1;
			object o2 = 1;
			ICollection<object> input = ListContaining(o1, o2);
			AssertCollectionEquals(ListContaining(o1), Collection.ToUniqueCollection(input));
		}

		[Test]
		public void ToUniqueCollection_WillRemoveDuplicateStringsStoredAsObjects()
		{
			string a = new string(new char[] {'h', 'e', 'l', 'l', 'o'});
			string b = new string(new char[] {'h', 'e', 'l', 'l', 'o'});

			object c = a;
			object d = b;

			ICollection<object> input = ListContaining(c, d);

			AssertCollectionEquals(ListContaining(c), Collection.ToUniqueCollection(input));
		}


		[Test]
		public void ToUniqueCollection_WillRemoveDuplicateReferenceTypes()
		{
			List<int> obj = new List<int>();
			ICollection<List<int>> input = ListContaining(obj, obj);
			AssertCollectionEquals(ListContaining(obj), Collection.ToUniqueCollection(input));
		}


		[Test]
		public void Containing_WillRetunNullWhenPassedNull()
		{
			ICollection<object> actual = Collection.Containing<object>(null);
			Assert.IsNull(actual);
		}


		[Test]
		public void Containing_WillRetunEmptyCollectionWhenPassedNothing()
		{
			ICollection<int> actual = Collection.Containing<int>();
			Assert.IsEmpty((ICollection) actual);
		}


		[Test]
		public void Containing_WillRetunCollectionContainingArgumentsSupplied()
		{
			List<int> expected = new List<int>();
			expected.AddRange(new int[3] {1, 2, 3});
			AssertCollectionEquals(expected, Collection.Containing(1, 2, 3));
		}


		[Test]
		public void ListContaining_WillRetunNullWhenPassedNull()
		{
			ICollection<object> actual = Collection.Containing<object>(null);
			Assert.IsNull(actual);
		}


		[Test]
		public void ListContaining_WillRetunEmptyListWhenPassedNothing()
		{
			ICollection<int> actual = Collection.Containing<int>();
			Assert.IsEmpty((ICollection) actual);
		}


		[Test]
		public void ListContaining_WillRetunListContainingArgumentsSupplied()
		{
			List<int> expected = new List<int>();
			expected.AddRange(new int[3] {1, 2, 3});
			AssertCollectionEquals(expected, Collection.Containing(1, 2, 3));
		}


		private static ICollection<T> ListContaining<T>(params T[] items)
		{
			return new List<T>(items);
		}


		private static void AssertCollectionEquals<T>(ICollection<T> expected, ICollection<T> actual)
		{
			Assert.AreEqual(expected.Count, actual.Count);
			IEnumerator<T> expectedEnum = expected.GetEnumerator();
			IEnumerator<T> actualEnum = actual.GetEnumerator();
			while (expectedEnum.MoveNext())
			{
				actualEnum.MoveNext(); //same size, don't need to check it
				Assert.AreEqual(expectedEnum.Current, actualEnum.Current);
			}
		}
	}
}