using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Property;
using NUnit.Framework;
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
            EntitySet<Blog> _blogs = new EntitySet<Blog>();

            public EntitySet<Blog> Blogs
            {
                get { return _blogs; }
            }
        }
    }
}
