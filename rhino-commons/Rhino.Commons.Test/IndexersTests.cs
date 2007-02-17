using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework;

namespace Rhino.Commons.Test
{
    [TestFixture]
    public class IndexersTests
    {
        [Test]
        public void Getter()
        {
            UsingIndexers indexers = new UsingIndexers();

            Assert.AreEqual(8, indexers.Indexer[1]);
            Assert.AreEqual(1, indexers.Key);
        }

        [Test]
        public void Setter()
        {
            UsingIndexers indexers = new UsingIndexers();

            indexers.Indexer[3] = 9;

            Assert.AreEqual(3, indexers.Key);
            Assert.AreEqual(9, indexers.Value);
        }

        [Test]
        public void GetterOnly()
        {
            UsingIndexers indexers = new UsingIndexers();

            Assert.AreEqual(8, indexers.Getter[1]);
            Assert.AreEqual(1, indexers.Key);
        }

        [Test]
        public void SetterOnly()
        {
            UsingIndexers indexers = new UsingIndexers();

            indexers.Setter[3] = 9;

            Assert.AreEqual(3, indexers.Key);
            Assert.AreEqual(9, indexers.Value);
        }
        
    }

    public class UsingIndexers
    {
        public int Key = 5;
        public int Value = 8;

        public UsingIndexers()
        {
            Indexer = new PropertyIndexer<int, int>(Get, Set);
            Getter = new PropertyIndexerGetter<int, int>(Get);
            Setter = new PropertyIndexerSetter<int, int>(Set);
        }

        public PropertyIndexer<int, int> Indexer;
        public PropertyIndexerGetter<int, int> Getter;
        public PropertyIndexerSetter<int, int> Setter;

        public int Get(int key)
        {
            Key = key;
            return Value;
        }

        public void Set(int key, int value)
        {
            Key = key;
            Value = value;
        }
    }
}
