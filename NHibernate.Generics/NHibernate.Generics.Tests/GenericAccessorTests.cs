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
using NHibernate.Property;
using MbUnit.Framework;
using System.Reflection;
using System.Collections;
using NHibernate.Generics;
using NHibernate.Generics.Tests.Properties;

namespace NHibernate.Generics.Tests
{
    [TestFixture]
    public class GenericAccessorTests
    {
        [Test]
        public void TestNonExistentFieldReturnsNull()
        {
            System.Type t = typeof(GenericAccessor);
            MethodInfo m = t.GetMethod("GetField", BindingFlags.Static | BindingFlags.NonPublic);
            Assert.IsNotNull(m);
            object[] parameters = new object[]{typeof(Blog), "madeUpFieldName"};
            object result = m.Invoke(null, parameters);

            Assert.IsNull(result);
        }

        [Test]
        [ExpectedException(typeof(PropertyNotFoundException), "Could not find field '_nothing' in class 'NHibernate.Generics.Tests.Blog'")]
        public void ExceptionWhenFieldDoesnotExists_Getter()
        {
            new GenericAccessor().GetGetter(typeof(Blog), "Nothing");
        }

        [Test]
        [ExpectedException(typeof(PropertyNotFoundException), "Could not find field '_nothing' in class 'NHibernate.Generics.Tests.Blog'")]
        public void ExceptionWhenFieldDoesnotExists_Setter()
        {
            new GenericAccessor().GetSetter(typeof(Blog), "Nothing");
        }

        [Test]
        [ExpectedException(typeof(PropertyAccessException), "could not set a field value by reflection setter of NHibernate.Generics.Tests.GenericAccessorTests+ClassWithNullEntitySet._blogs")]
        public void ExceptionWhenSettingFieldValueWhenEntityIsNull()
        {
            ISetter setter = new GenericAccessor().GetSetter(typeof(ClassWithNullEntitySet), "Blogs");
            setter.Set(new ClassWithNullEntitySet(), null);
        }
        
        public class ClassWithNullEntitySet
        {
            EntitySet<Blog> _blogs;

            public EntitySet<Blog> Blogs
            {
                get { return _blogs; }
            }
        }
    }
}
