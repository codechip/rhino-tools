using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Collections;
using MbUnit.Framework;

namespace BerkeleyDb.Tests
{
	[TestFixture]
	public class BerkeleyDbTreeTests
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
		public void Can_create_btree()
		{
			using (var environment = new BerkeleyDbEnvironment("test"))
			{
				using (var tree = environment.OpenTree("my-tree"))
				{
					Assert.IsNotNull(tree);
				}
			}
		}

		[Test]
		public void Can_put_items_using_string()
		{
			using (var environment = new BerkeleyDbEnvironment("test"))
			{
				environment.CreateTree("my-tree");
				using (var tree = environment.OpenTree("my-tree"))
					tree.Put("hello", "bye");
			}
		}

		[Test]
		public void Can_put_items_using_guid()
		{
			using (var environment = new BerkeleyDbEnvironment("test"))
			{
				using (var tree = environment.OpenTree("my-tree"))
					tree.Put(Guid.NewGuid(), "bye");
			}
		}

		[Test]
		public void Can_get_items_using_string()
		{
			using (var environment = new BerkeleyDbEnvironment("test"))
			{
				using (var tree = environment.OpenTree("my-tree"))
					tree.Put("hello", "bye");
			}

			using (var environment = new BerkeleyDbEnvironment("test"))
			{
				using (var tree = environment.OpenTree("my-tree"))
				{
					object value = tree.Get("hello");
					Assert.AreEqual("bye", value);
				}
			}
		}

		[Test]
		public void Can_get_items_using_guid()
		{
			var guid = Guid.NewGuid();
			using (var environment = new BerkeleyDbEnvironment("test"))
			{
				using (var tree = environment.OpenTree("my-tree"))
				{
					tree.Put(guid, "bye");
					Assert.IsNotNull(tree);
				}
			}

			using (var environment = new BerkeleyDbEnvironment("test"))
			{
				using (var tree = environment.OpenTree("my-tree"))
				{
					var value = tree.Get(guid);
					Assert.AreEqual("bye", value);
				}
			}
		}

		[Test]
		public void Can_get_items_with_transaction()
		{
			using (var environment = new BerkeleyDbEnvironment("test"))
			using (var tx = environment.BeginTransaction())
			using (var tree = environment.OpenTree("my-tree"))
			{
				tree.Put("hello", "bye");
				tx.Commit();
			}

			using (var environment = new BerkeleyDbEnvironment("test"))
			using (var tx = environment.BeginTransaction())
			using (var tree = environment.OpenTree("my-tree"))
			{
				object value = tree.Get("hello");
				Assert.AreEqual("bye", value);
				tx.Commit();
			}
		}

		[Test]
		public void Can_get_items_with_transaction_when_rolled_back_item_not_in_tree()
		{
			using (var environment = new BerkeleyDbEnvironment("test"))
			using (var tx = environment.BeginTransaction())
			using (var tree = environment.OpenTree("my-tree"))
			{
				tree.Put("hello", "bye");
				tx.Rollback();
			}

			using (var environment = new BerkeleyDbEnvironment("test"))
			using (var tx = environment.BeginTransaction())
			using (var tree = environment.OpenTree("my-tree"))
			{
				object value = tree.Get("hello");
				Assert.IsNull(value);
			}
		}

		[Test]
		public void Can_get_items_with_transaction_in_tree_and_queue()
		{
			using (var environment = new BerkeleyDbEnvironment("test"))
			using (var tx = environment.BeginTransaction())
			using (var queue = environment.OpenQueue("my-queue"))
			using (var tree = environment.OpenTree("my-tree"))
			{
				queue.Append(new DateTime(2000,1,1));
				tree.Put("hello", "bye");
				tx.Commit();
			}

			using (var environment = new BerkeleyDbEnvironment("test"))
			using (var tx = environment.BeginTransaction())
			using (var queue = environment.OpenQueue("my-queue"))
			using (var tree = environment.OpenTree("my-tree"))
			{
				object value = tree.Get("hello");
				Assert.AreEqual("bye", value);
				value = queue.Consume();
				Assert.AreEqual(new DateTime(2000, 1, 1), value);
				tx.Commit();
			}
		}

		[Test]
		public void Can_truncate_tree()
		{
			using (var environment = new BerkeleyDbEnvironment("test"))
			using (var tx = environment.BeginTransaction())
			using (var tree = environment.OpenTree("my-tree"))
			{
				tree.Put("hello", "bye");
				tx.Commit();
			}

			using (var environment = new BerkeleyDbEnvironment("test"))
			using (var tx = environment.BeginTransaction())
			using (var tree = environment.OpenTree("my-tree"))
			{
				tree.Truncate();
				tx.Commit();
			}

			using (var environment = new BerkeleyDbEnvironment("test"))
			using (var tx = environment.BeginTransaction())
			using (var tree = environment.OpenTree("my-tree"))
			{
				object value = tree.Get("hello");
				Assert.IsNull(value);
				tx.Commit();
			}
		}

		[Test]
		public void Can_delete_items_using_strings()
		{
			using (var environment = new BerkeleyDbEnvironment("test"))
			using (var tx = environment.BeginTransaction())
			using (var tree = environment.OpenTree("my-tree"))
			{
				tree.Put("hello", "bye");
				tx.Commit();
			}

			using (var environment = new BerkeleyDbEnvironment("test"))
			using (var tx = environment.BeginTransaction())
			using (var tree = environment.OpenTree("my-tree"))
			{
				object value = tree.Get("hello");
				Assert.IsNotNull(value);
				tx.Commit();
			}

			using (var environment = new BerkeleyDbEnvironment("test"))
			using (var tx = environment.BeginTransaction())
			using (var tree = environment.OpenTree("my-tree"))
			{
				tree.Delete("hello");
				tx.Commit();
			}

			using (var environment = new BerkeleyDbEnvironment("test"))
			using (var tx = environment.BeginTransaction())
			using (var tree = environment.OpenTree("my-tree"))
			{
				object value = tree.Get("hello");
				Assert.IsNull(value);
				tx.Commit();
			}
		}

		[Test]
		public void Can_delete_items_using_guids()
		{
			var id = Guid.NewGuid();
			using (var environment = new BerkeleyDbEnvironment("test"))
			using (var tx = environment.BeginTransaction())
			using (var tree = environment.OpenTree("my-tree"))
			{
				tree.Put(id, "bye");
				tx.Commit();
			}

			using (var environment = new BerkeleyDbEnvironment("test"))
			using (var tx = environment.BeginTransaction())
			using (var tree = environment.OpenTree("my-tree"))
			{
				object value = tree.Get(id);
				Assert.IsNotNull(value);
				tx.Commit();
			}

			using (var environment = new BerkeleyDbEnvironment("test"))
			using (var tx = environment.BeginTransaction())
			using (var tree = environment.OpenTree("my-tree"))
			{
				tree.Delete(id);
				tx.Commit();
			}

			using (var environment = new BerkeleyDbEnvironment("test"))
			using (var tx = environment.BeginTransaction())
			using (var tree = environment.OpenTree("my-tree"))
			{
				object value = tree.Get(id);
				Assert.IsNull(value);
				tx.Commit();
			}
		}

		[Test]
		public void Can_get_items_using_object()
		{
			var id = new DateTime(2000, 1, 1);
			using (var environment = new BerkeleyDbEnvironment("test"))
			using (var tx = environment.BeginTransaction())
			using (var tree = environment.OpenTree("my-tree"))
			{
				tree.Put(id, "bye");
				tx.Commit();
			}

			using (var environment = new BerkeleyDbEnvironment("test"))
			using (var tx = environment.BeginTransaction())
			using (var tree = environment.OpenTree("my-tree"))
			{
				object value = tree.Get(id);
				Assert.IsNotNull(value);
				tx.Commit();
			}
		}

		[Test]
		public void Can_delete_items_using_object()
		{
			var id = new DateTime(2000,1,1);
			using (var environment = new BerkeleyDbEnvironment("test"))
			using (var tx = environment.BeginTransaction())
			using (var tree = environment.OpenTree("my-tree"))
			{
				tree.Put(id, "bye");
				tx.Commit();
			}

			using (var environment = new BerkeleyDbEnvironment("test"))
			using (var tx = environment.BeginTransaction())
			using (var tree = environment.OpenTree("my-tree"))
			{
				object value = tree.Get(id);
				Assert.IsNotNull(value);
				tx.Commit();
			}

			using (var environment = new BerkeleyDbEnvironment("test"))
			using (var tx = environment.BeginTransaction())
			using (var tree = environment.OpenTree("my-tree"))
			{
				tree.Delete(id);
				tx.Commit();
			}

			using (var environment = new BerkeleyDbEnvironment("test"))
			using (var tx = environment.BeginTransaction())
			using (var tree = environment.OpenTree("my-tree"))
			{
				object value = tree.Get(id);
				Assert.IsNull(value);
				tx.Commit();
			}
		}


		[Test]
		public void Can_roll_back_delete()
		{
			var id = Guid.NewGuid();
			using (var environment = new BerkeleyDbEnvironment("test"))
			using (var tx = environment.BeginTransaction())
			using (var tree = environment.OpenTree("my-tree"))
			{
				tree.Put(id, "bye");
				tx.Commit();
			}

			using (var environment = new BerkeleyDbEnvironment("test"))
			using (var tx = environment.BeginTransaction())
			using (var tree = environment.OpenTree("my-tree"))
			{
				object value = tree.Get(id);
				Assert.IsNotNull(value);
				tx.Commit();
			}

			using (var environment = new BerkeleyDbEnvironment("test"))
			using (var tx = environment.BeginTransaction())
			using (var tree = environment.OpenTree("my-tree"))
			{
				tree.Delete(id);
				tx.Rollback();
			}

			using (var environment = new BerkeleyDbEnvironment("test"))
			using (var tx = environment.BeginTransaction())
			using (var tree = environment.OpenTree("my-tree"))
			{
				object value = tree.Get(id);
				Assert.IsNotNull(value);
				tx.Commit();
			}
		}
	}
}