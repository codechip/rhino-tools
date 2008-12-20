using System.Collections.Generic;
using System.Linq;
using Rhino.ServiceBus.DataStructures;
using Xunit;

namespace Rhino.ServiceBus.Tests.DataStructures
{
    public class HashtableTest
    {
        private readonly Hashtable<string, int> dic = new Hashtable<string, int>();

        [Fact]
        public void Can_add_item_to_dictionary()
        {
            dic.Write(
                delegate(AddAction<string, int> add, RemoveAction<string> remove, TryGetAction<string, int> tryGet)
                {
                    add("a", 5);
                    add("b", 7);
                    add("c", 6);
                });
            dic.Read(get =>
            {
                int val;
                get("a", out val);
                Assert.Equal(5, val);
                get("b", out val);
                Assert.Equal(7, val);
                get("c", out val);
                Assert.Equal(6, val);
            });
        }

        [Fact]
        public void Can_remove_item_to_dictionary()
        {
            dic.Write((add, remove, tryGet) =>
            {
                add("a", 5);
                add("c", 6);
            });
            dic.Read(get =>
            {
                int val;
                bool result = get("b", out val);
                Assert.False(result);
            });
        }

        [Fact]
        public void Can_iterate_over_items_in_dictionary()
        {
            dic.Write((add, remove, tryGet) =>
            {
                add("a", 5);
                add("c", 6);
            });

            KeyValuePair<string, int>[] array = dic.OrderBy(x => x.Key)
                .ToArray();

            Assert.Equal("a", array[0].Key);
            Assert.Equal(5, array[0].Value);

            Assert.Equal("c", array[1].Key);
            Assert.Equal(6, array[1].Value);
        }
    }
}