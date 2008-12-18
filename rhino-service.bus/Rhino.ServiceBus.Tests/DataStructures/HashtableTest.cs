using Rhino.ServiceBus.DataStructures;
using Xunit;
using System.Linq;

namespace Rhino.ServiceBus.Tests.DataStructures
{
    public class HashtableTest
    {
        private Hashtable<string, int> dic = new Hashtable<string, int>();

        [Fact]
        public void Can_add_item_to_dictionary()
        {
            dic.Write((add,remove) =>
            {
                add("a", 5);
                add("b", 7);
                add("c", 6);
            });
            int val;
            dic.TryGet("a", out val);
            Assert.Equal(5, val);
            dic.TryGet("b", out val);
            Assert.Equal(7, val);
            dic.TryGet("c", out val);
            Assert.Equal(6, val);
        }

        [Fact]
        public void Can_remove_item_to_dictionary()
        {
            dic.Write((add, remove) =>
            {
                add("a", 5);
                add("c", 6);
            });
            int val;
            var result = dic.TryGet("b", out val);
            Assert.False(result);
        }

        [Fact]
        public void Can_iterate_over_items_in_dictionary()
        {
            dic.Write((add, remove) =>
            {
                add("a", 5);
                add("c", 6);
            });

            var array = dic.OrderBy(x => x.Key)
                .ToArray();

            Assert.Equal("a",array[0].Key);
            Assert.Equal(5, array[0].Value);

            Assert.Equal("c", array[1].Key);
            Assert.Equal(6, array[1].Value);
        }
    }
}