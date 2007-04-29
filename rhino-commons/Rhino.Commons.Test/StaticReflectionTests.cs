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
