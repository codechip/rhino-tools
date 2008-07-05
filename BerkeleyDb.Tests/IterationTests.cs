using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MbUnit.Framework;

namespace BerkeleyDb.Tests
{
	[TestFixture]
	public class IterationTests
	{
		[SetUp]
		public void Setup()
		{
			if (Directory.Exists("test"))
				Directory.Delete("test", true);
			Directory.CreateDirectory("test");
			using (var environment = new BerkeleyDbEnvironment("test"))
			{
				environment.Delete("my-tree");
				environment.Delete("my-queue");
				environment.CreateTree("my-tree");
				environment.CreateQueue("my-queue", 128);
			}
		}

		[Test]
		public void Can_iterate_items_in_queue()
		{
			using (var environment = new BerkeleyDbEnvironment("test"))
			using (var tx = environment.BeginTransaction())
			using (var queue = environment.OpenQueue("my-queue"))
			{
				queue.Append(new DateTime(2000, 1, 1));
				queue.Append(true);
				queue.Append("there");
				tx.Commit();
			}

			using (var environment = new BerkeleyDbEnvironment("test"))
			using (var tx = environment.BeginTransaction())
			using (var queue = environment.OpenQueue("my-queue"))
			{
				var enumerator = queue.Select().GetEnumerator();
				enumerator.MoveNext();
				Assert.AreEqual(new DateTime(2000, 1, 1), enumerator.Current);
				enumerator.MoveNext();
				Assert.AreEqual(true, enumerator.Current);
				enumerator.MoveNext();
				Assert.AreEqual("there", enumerator.Current);
				var result = enumerator.MoveNext();
				Assert.IsFalse(result);
				tx.Commit();
			}
		}

		[Test]
		public void Can_iterate_items_in_queue_with_condition()
		{
			using (var environment = new BerkeleyDbEnvironment("test"))
			using (var tx = environment.BeginTransaction())
			using (var queue = environment.OpenQueue("my-queue"))
			{
				queue.Append(new DateTime(2000, 1, 1));
				queue.Append(true);
				queue.Append("there");
				tx.Commit();
			}

			using (var environment = new BerkeleyDbEnvironment("test"))
			using (var tx = environment.BeginTransaction())
			using (var queue = environment.OpenQueue("my-queue"))
			{
				var enumerator = queue.Select(o => o is bool).GetEnumerator();
				enumerator.MoveNext();
				Assert.AreEqual(true, enumerator.Current);
				var result = enumerator.MoveNext();
				Assert.IsFalse(result);
				tx.Commit();
			}
		}

		[Test]
		public void Can_iterate_and_consume_items_in_queue()
		{
			using (var environment = new BerkeleyDbEnvironment("test"))
			using (var tx = environment.BeginTransaction())
			using (var queue = environment.OpenQueue("my-queue"))
			{
				queue.Append(new DateTime(2000, 1, 1));
				queue.Append(true);
				queue.Append("there");
				tx.Commit();
			}

			using (var environment = new BerkeleyDbEnvironment("test"))
			using (var tx = environment.BeginTransaction())
			using (var queue = environment.OpenQueue("my-queue"))
			{
				var enumerator = queue.SelectAndConsume().GetEnumerator();
				enumerator.MoveNext();
				Assert.AreEqual(new DateTime(2000, 1, 1), enumerator.Current);
				enumerator.MoveNext();
				Assert.AreEqual(true, enumerator.Current);
				enumerator.MoveNext();
				Assert.AreEqual("there", enumerator.Current);
				var result = enumerator.MoveNext();
				Assert.IsFalse(result);
				tx.Commit();
			}

			using (var environment = new BerkeleyDbEnvironment("test"))
			using (var tx = environment.BeginTransaction())
			using (var queue = environment.OpenQueue("my-queue"))
			{
				var enumerator = queue.Select().GetEnumerator();

				Assert.IsFalse(enumerator.MoveNext());
				tx.Commit();
			}
		}

		[Test]
		public void Can_iterate_and_consume_items_in_queue_with_condition()
		{
			using (var environment = new BerkeleyDbEnvironment("test"))
			using (var tx = environment.BeginTransaction())
			using (var queue = environment.OpenQueue("my-queue"))
			{
				queue.Append(new DateTime(2000, 1, 1));
				queue.Append(true);
				queue.Append("there");
				tx.Commit();
			}

			using (var environment = new BerkeleyDbEnvironment("test"))
			using (var tx = environment.BeginTransaction())
			using (var queue = environment.OpenQueue("my-queue"))
			{
				var enumerator = queue.SelectAndConsume(o => o is bool).GetEnumerator();
				enumerator.MoveNext();
				Assert.AreEqual(true, enumerator.Current);
				var result = enumerator.MoveNext();
				Assert.IsFalse(result);
				tx.Commit();
			}

			using (var environment = new BerkeleyDbEnvironment("test"))
			using (var tx = environment.BeginTransaction())
			using (var queue = environment.OpenQueue("my-queue"))
			{
				var enumerator = queue.SelectAndConsume().GetEnumerator();
				enumerator.MoveNext();
				Assert.AreEqual(new DateTime(2000, 1, 1), enumerator.Current);
				enumerator.MoveNext();
				Assert.AreEqual("there", enumerator.Current);
				var result = enumerator.MoveNext();
				Assert.IsFalse(result);
				tx.Commit();
			}
		}

		[Test]
		public void Can_iterate_and_consume_items_in_queue_with_condition_strongly_typed()
		{
			using (var environment = new BerkeleyDbEnvironment("test"))
			using (var tx = environment.BeginTransaction())
			using (var queue = environment.OpenQueue("my-queue"))
			{
				queue.Append(new DateTime(2000, 1, 1));
				queue.Append(new DateTime(2001, 1, 1));
				queue.Append(new DateTime(2002, 1, 1));
				tx.Commit();
			}

			using (var environment = new BerkeleyDbEnvironment("test"))
			using (var tx = environment.BeginTransaction())
			using (var queue = environment.OpenQueue("my-queue"))
			{
				Assert.AreEqual(
					2,
					queue.SelectAndConsume<DateTime>(o => o.Year > 2000).Count());
				tx.Commit();
			}

			using (var environment = new BerkeleyDbEnvironment("test"))
			using (var tx = environment.BeginTransaction())
			using (var queue = environment.OpenQueue("my-queue"))
			{
				var enumerator = queue.SelectAndConsume<DateTime>(x => true).GetEnumerator();
				enumerator.MoveNext();
				Assert.AreEqual(2000, enumerator.Current.Year);
				var result = enumerator.MoveNext();
				Assert.IsFalse(result);
				tx.Commit();
			}
		}


		[Test]
		public void Can_select_and_consume_from_tree()
		{
			using (var environment = new BerkeleyDbEnvironment("test"))
			using (var tx = environment.BeginTransaction())
			using (var tree = environment.OpenTree("my-tree"))
			{
				tree.Put(new DateTime(2000, 1, 1), "1");
				tree.Put("s", "2");
				tree.Put(new string('u', 500), new string('3', 1024));
				tx.Commit();
			}

			using (var environment = new BerkeleyDbEnvironment("test"))
			using (var tx = environment.BeginTransaction())
			using (var tree = environment.OpenTree("my-tree"))
			{
				var array = tree.SelectAndConsume().OrderBy(entry => entry.Value).ToArray();

				Assert.AreEqual(new DateTime(2000, 1, 1), array[0].Key);
				Assert.AreEqual("1", array[0].Value);
				Assert.AreEqual("s", array[1].Key);
				Assert.AreEqual("2", array[1].Value);
				Assert.AreEqual(new string('u', 500), array[2].Key);
				Assert.AreEqual(new string('3', 1024), array[2].Value);
				tx.Commit();
			}
		}

		[Test]
		public void After_select_and_consume_on_tree_called_there_are_no_items_in_tree()
		{
			using (var environment = new BerkeleyDbEnvironment("test"))
			using (var tx = environment.BeginTransaction())
			using (var tree = environment.OpenTree("my-tree"))
			{
				tree.Put(new DateTime(2000, 1, 1), "1");
				tree.Put("s", "2");
				tree.Put(new string('u', 500), new string('3', 1024));
				tx.Commit();
			}

			using (var environment = new BerkeleyDbEnvironment("test"))
			using (var tx = environment.BeginTransaction())
			using (var tree = environment.OpenTree("my-tree"))
			{
				DictionaryEntry[] array1 = tree.SelectAndConsume().OrderBy(entry => entry.Value).ToArray();
				Assert.AreEqual(3, array1.Length);
				DictionaryEntry[] array2 = tree.SelectAndConsume().OrderBy(entry => entry.Value).ToArray();
				Assert.AreEqual(0, array2.Length);
				tx.Commit();
			}
		}

		[Test]
		public void When_select_and_consume_on_tree_encounters_a_deleted_item_will_skip_it()
		{
			using (var environment = new BerkeleyDbEnvironment("test"))
			using (var tx = environment.BeginTransaction())
			using (var tree = environment.OpenTree("my-tree"))
			{
				tree.Put(new DateTime(2000, 1, 1), "1");
				tree.Put("s", "2");
				tree.Put(new string('u', 500), new string('3', 1024));
				tx.Commit();
			}

			using (var environment = new BerkeleyDbEnvironment("test"))
			using (var tx = environment.BeginTransaction())
			using (var tree = environment.OpenTree("my-tree"))
			{
				var entries = new List<DictionaryEntry>();
				foreach (var entry in tree.SelectAndConsume())
				{
					// no order guarantees when traversing it
					if (Equals(entry.Key, "s") == false)
						tree.Delete("s");
					else
						tree.Delete(new string('u', 500));

					entries.Add(entry);
				}
				Assert.AreEqual(2, entries.Count);
				tx.Commit();
			}
		}

		[Test]
		public void When_select_and_consume_on_queue_encounters_a_deleted_item_will_skip_it()
		{
			using (var environment = new BerkeleyDbEnvironment("test"))
			using (var tx = environment.BeginTransaction())
			using (var queue = environment.OpenQueue("my-queue"))
			{
				queue.Append(new DateTime(2000, 1, 1));
				queue.Append(true);
				queue.Append("there");
				tx.Commit();
			}

			using (var environment = new BerkeleyDbEnvironment("test"))
			using (var tx = environment.BeginTransaction())
			using (var queue = environment.OpenQueue("my-queue"))
			{
				var enumerator = queue.Select().GetEnumerator();
				enumerator.MoveNext();
				Assert.AreEqual(new DateTime(2000, 1, 1), enumerator.Current);

				using (var environment2 = new BerkeleyDbEnvironment("test"))
				using (var tx2 = environment2.BeginTransaction())
				using (var queue2 = environment2.OpenQueue("my-queue"))
				{
					queue2.Consume(); // consume & discard an item, cursor should skip it
					tx2.Commit();
				}
				enumerator.MoveNext();
				Assert.AreEqual("there", enumerator.Current);
				var result = enumerator.MoveNext();
				Assert.IsFalse(result);
				tx.Commit();
			}
		}
	}
}