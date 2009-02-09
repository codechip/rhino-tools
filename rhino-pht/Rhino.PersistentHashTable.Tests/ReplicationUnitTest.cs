using Xunit;

namespace Rhino.DHT.Tests
{
    public class ReplicationUnitTest
    {
        [Fact]
        public void Will_distribute_work_starting_with_next_node()
        {
            var nodes = new[] { 1, 2, 3 };
            Assert.Equal(new[] { 3, 1 }, nodes.GetOtherElementsFromElement(2));
            Assert.Equal(new[] { 1, 2 }, nodes.GetOtherElementsFromElement(3));
            Assert.Equal(new[] { 2, 3 }, nodes.GetOtherElementsFromElement(1));
            Assert.Equal(new[] { 1, 2, 3 }, nodes.GetOtherElementsFromElement(4));
        }
    }
}