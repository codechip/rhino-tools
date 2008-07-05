using System;
using System.Collections.Generic;
using MbUnit.Framework;
using System.Linq;

namespace BerkeleyDb.Tests
{
	[TestFixture]
	public class AssociationTests
	{
		[SetUp]
		public void Setup()
		{
			using (var environment = new BerkeleyDbEnvironment("test"))
			{
				environment.Delete("my-tree");
				environment.Delete("my-queue");
				environment.CreateTree("my-tree");
				environment.CreateQueue("my-queue", 128);
			}
		}

		[Test]
		public void Can_select_from_queue_and_get_values_from_tree()
		{
			using (var environment = new BerkeleyDbEnvironment("test"))
			using (var tx = environment.BeginTransaction())
			using (var queue = environment.OpenQueue("my-queue"))
			using (var tree = environment.OpenTree("my-tree"))
			{
				Guid id = Guid.NewGuid();
				tree.Put(id, "bye");
				queue.Append(id);
				tx.Commit();
			}

			using (var environment = new BerkeleyDbEnvironment("test"))
			using (var tx = environment.BeginTransaction())
			using (var queue = environment.OpenQueue("my-queue"))
			using (var tree = environment.OpenTree("my-tree"))
			{
				foreach (var str in queue.SelectAndConsumeFromAssociation<string>(tree, s => true))
				{
					Assert.AreEqual("bye", str);
				}
				tx.Commit();
			}
		}


		[Test]
		public void Can_select_from_queue_and_get_values_from_tree_with_helper_method()
		{
			using (var environment = new BerkeleyDbEnvironment("test"))
			using (var tx = environment.BeginTransaction())
			using (var queue = environment.OpenQueue("my-queue"))
			using (var tree = environment.OpenTree("my-tree"))
			{
				queue.AppendAssociation(tree, "a");
				queue.AppendAssociation(tree, "b");
				queue.AppendAssociation(tree, "c");
				tx.Commit();
			}

			using (var environment = new BerkeleyDbEnvironment("test"))
			using (var tx = environment.BeginTransaction())
			using (var queue = environment.OpenQueue("my-queue"))
			using (var tree = environment.OpenTree("my-tree"))
			{
				var array = queue.SelectAndConsumeFromAssociation<string>(tree, s => true).ToArray();
				Assert.AreEqual("a", array[0]);
				Assert.AreEqual("b", array[1]);
				Assert.AreEqual("c", array[2]);
				tx.Commit();
			}
		}

		[Test]
		public void Can_select_from_queue_and_get_values_from_tree_with_filter()
		{
			using (var environment = new BerkeleyDbEnvironment("test"))
			using (var tx = environment.BeginTransaction())
			using (var queue = environment.OpenQueue("my-queue"))
			using (var tree = environment.OpenTree("my-tree"))
			{
				queue.AppendAssociation(tree, "a");
				queue.AppendAssociation(tree, "b");
				queue.AppendAssociation(tree, "c");
				tx.Commit();
			}

			using (var environment = new BerkeleyDbEnvironment("test"))
			using (var tx = environment.BeginTransaction())
			using (var queue = environment.OpenQueue("my-queue"))
			using (var tree = environment.OpenTree("my-tree"))
			{
				var array = queue.SelectAndConsumeFromAssociation<string>(tree, s => s != "a").ToArray();
				Assert.AreEqual("b", array[0]);
				Assert.AreEqual("c", array[1]);
				tx.Commit();
			}
		}

		[Test]
		public void Select_with_filter_will_keep_values_in_queue_if_filtered_out()
		{
			using (var environment = new BerkeleyDbEnvironment("test"))
			using (var tx = environment.BeginTransaction())
			using (var queue = environment.OpenQueue("my-queue"))
			using (var tree = environment.OpenTree("my-tree"))
			{
				queue.AppendAssociation(tree, "a");
				queue.AppendAssociation(tree, "b");
				queue.AppendAssociation(tree, "c");
				queue.AppendAssociation(tree, "d");
				tx.Commit();
			}

			using (var environment = new BerkeleyDbEnvironment("test"))
			using (var tx = environment.BeginTransaction())
			using (var queue = environment.OpenQueue("my-queue"))
			using (var tree = environment.OpenTree("my-tree"))
			{
				var array = queue.SelectAndConsumeFromAssociation<string>(tree, s => s != "b" && s != "d").ToArray();
				Assert.AreEqual("a", array[0]);
				Assert.AreEqual("c", array[1]);

				array = queue.SelectAndConsumeFromAssociation<string>(tree, s => true).ToArray();
				Assert.AreEqual("b", array[0]);
				Assert.AreEqual("d", array[1]);

				tx.Commit();
			}
		}

		[Test]
		public void When_consuming_value_will_be_removed_from_tree()
		{
			Guid id;
			using (var environment = new BerkeleyDbEnvironment("test"))
			using (var tx = environment.BeginTransaction())
			using (var queue = environment.OpenQueue("my-queue"))
			using (var tree = environment.OpenTree("my-tree"))
			{
				id = queue.AppendAssociation(tree, "a");
				tx.Commit();
			}

			using (var environment = new BerkeleyDbEnvironment("test"))
			using (var tx = environment.BeginTransaction())
			using (var queue = environment.OpenQueue("my-queue"))
			using (var tree = environment.OpenTree("my-tree"))
			{
				var array = queue.SelectAndConsumeFromAssociation<string>(tree, s => s != "b" && s != "d").ToArray();
				Assert.AreEqual("a", array[0]);

				Assert.IsNull(tree.Get(id));

				tx.Commit();
			}
		}
	}
}