using System.Reflection;
using MbUnit.Framework;

namespace Rhino.Commons.Test
{
    [TestFixture]
    public class StaticReflectionTests
    {
        public void Proc1() { }

        [Test]
        public void Proc1Test()
        {
            MethodInfo expected = typeof(StaticReflectionTests).GetMethod("Proc1");
            MethodInfo actual = StaticReflection.MethodInfo(Proc1);

            Assert.AreEqual(expected, actual);
        }

        public void Proc2(int i) { }

        [Test]
        public void Proc2Test()
        {
            MethodInfo expected = typeof(StaticReflectionTests).GetMethod("Proc2");
            MethodInfo actual = StaticReflection.MethodInfo<int>(Proc2);

            Assert.AreEqual(expected, actual);
        }

        public void Proc3(int i, int j) { }

        [Test]
        public void Proc3Test()
        {
            MethodInfo expected = typeof(StaticReflectionTests).GetMethod("Proc3");
            MethodInfo actual = StaticReflection.MethodInfo<int, int>(Proc3);

            Assert.AreEqual(expected, actual);
        }

        public int Func1() { return 0; }

        [Test]
        public void Func1Test()
        {
            MethodInfo expected = typeof(StaticReflectionTests).GetMethod("Func1");
            MethodInfo actual = StaticReflection.MethodInfo<int>(Func1);

            Assert.AreEqual(expected, actual);
        }

        public int Func2(int i) { return 0; }

        [Test]
        public void Func2Test()
        {
            MethodInfo expected = typeof(StaticReflectionTests).GetMethod("Func2");
            MethodInfo actual = StaticReflection.MethodInfo<int, int>(Func2);

            Assert.AreEqual(expected, actual);
        }

        public int Func3(int i, int j) { return 0; }

        [Test]
        public void Func3Test()
        {
            MethodInfo expected = typeof(StaticReflectionTests).GetMethod("Func3");
            MethodInfo actual = StaticReflection.MethodInfo<int, int, int>(Func3);

            Assert.AreEqual(expected, actual);
        }
    }
}
