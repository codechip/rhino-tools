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
