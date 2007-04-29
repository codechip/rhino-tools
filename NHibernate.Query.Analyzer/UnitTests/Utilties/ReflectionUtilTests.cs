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
using System.Collections;
using System.Reflection;
using System.Xml.Serialization;
using Ayende.NHibernateQueryAnalyzer.Utilities;
using MbUnit.Framework;

namespace Ayende.NHibernateQueryAnalyzer.Tests.Utilties
{
    [TestFixture]
    public class ReflectionUtilTests
    {
        [Test]
        public void GetPropertiesDictionary_NullProperty()
        {
            TestForReflection tfr = new TestForReflection();
            IDictionary props = ReflectionUtil.GetPropertiesDictionary(tfr);
            Assert.AreEqual("null", props["NullProperty"]);
        }

        [Test]
        public void GetPropertyValueWhenCannotRead()
        {
            TestForReflection t = new TestForReflection();
            object o = ReflectionUtil.GetPropertyValue(t, "SetValue");
            Assert.AreEqual("No value", (string)o, "Property with set only return a valid value.");
        }

        [Test]
        public void GetPropertyValueStr()
        {
            TestForReflection t = new TestForReflection();
            Assert.AreEqual(t.Value, ReflectionUtil.GetPropertyValue(t, "Value"), "Get property value return invalid value");
        }

        [Test]
        public void GetPropertyValueArray()
        {
            TestForReflection t = new TestForReflection();
            object o = ReflectionUtil.GetPropertyValue(t, "StringArray");
            Assert.AreEqual(typeof(string[]), o.GetType());
        }

        [Test]
        public void IsSimpleType()
        {
            Assert.IsTrue(ReflectionUtil.IsSimpleType(typeof(bool)), "ReflectionUtil didn't recognized a simple type");
            Assert.IsTrue(ReflectionUtil.IsSimpleType(typeof(string)), "ReflectionUtil didn't recognized a simple type");
            Assert.IsTrue(ReflectionUtil.IsSimpleType(typeof(int)), "ReflectionUtil didn't recognized a simple type");
            Assert.IsTrue(ReflectionUtil.IsSimpleType(typeof(int)), "ReflectionUtil didn't recognized a simple type");
            Assert.IsTrue(ReflectionUtil.IsSimpleType(typeof(AssemblyNameFlags)), "ReflectionUtil didn't recognized a simple type");
            Assert.IsFalse(ReflectionUtil.IsSimpleType(typeof(TestForReflection)), "ReflectionUtil didn't recognized a simple type");
        }

        [Test]
        public void IsSimpleObject()
        {
            Assert.IsTrue(ReflectionUtil.IsSimpleObject(null), "Null is not considered a simple type");
        }


        [Test]
        public void GetName()
        {
            Assert.AreEqual("{String}", ReflectionUtil.GetName("Just a string"), "Didn't get the type name if there is no name field or property");
        }

        [Test]
        public void GetNameOrEmpty()
        {
            Assert.AreEqual("FooBar", ReflectionUtil.GetNameOrEmpty(new TestForReflection()), "Didn't get the value of the Name property");
            Assert.AreEqual("names", ReflectionUtil.GetNameOrEmpty(new NamedObject()), "Didn't get the value of name field");
            Assert.AreEqual("", ReflectionUtil.GetNameOrEmpty("ssh! I'm a secret type with no name property or field"), "Returned a value other than empty string on a type with no name field or property");
        }


        [Test]
        public void GetFieldValue()
        {
            FieldInfo field = typeof(NamedObject).GetField("name");
            Assert.AreEqual("names", ReflectionUtil.GetFieldValue(new NamedObject(), field), "didn't get the field's value");
            Assert.AreEqual("No value", ReflectionUtil.GetFieldValue(null, field), "Returned value other than 'no value'");
            Assert.AreEqual("No value", ReflectionUtil.GetFieldValue(null, null), "Returned value other than 'no value'");
            Assert.AreEqual("No value", ReflectionUtil.GetFieldValue(new NamedObject(), null), "Returned value other than 'no value'");

        }


        [Test]
        public void GetValue()
        {
            Assert.AreEqual("test", ReflectionUtil.GetValue("test"), "Doesn't return the value of string");
            Assert.AreEqual("1", ReflectionUtil.GetValue(1), "Doesn't return the value of int");
            Assert.AreEqual("names", ReflectionUtil.GetValue(new NamedObject()), "Doesn't return the name of a complex object");
            Assert.AreEqual("{String[]}", ReflectionUtil.GetValue(new string[] { }), "Doesn't return the type of a complex unnamed object");
        }

        [Test]
        public void GetFieldsWithAttribute()
        {
            FieldInfoCollection fi = ReflectionUtil.GetFieldsWithAttribute(typeof(NamedObject), typeof(XmlIgnoreAttribute));
            Assert.AreEqual(1, fi.Count);
            Assert.AreEqual("marked", fi[0].Name);
        }

        [Test]
        public void ConvertTo()
        {
            byte b = 244;
            int i = 2334;
            DateTime d = DateTime.Now;
            string s = "string testing";
            UriPartial uri = UriPartial.Authority;
            Assert.AreEqual(b, ReflectionUtil.ConvertTo(b.ToString(), typeof(byte)), "Can't convert bytes");
            Assert.AreEqual(i, ReflectionUtil.ConvertTo(i.ToString(), typeof(int)), "Can't convert int");
            Assert.AreEqual(d.Date, ((DateTime)ReflectionUtil.ConvertTo(d.ToString(), typeof(DateTime))).Date, "Can't convert datetime");
            Assert.AreEqual(s, ReflectionUtil.ConvertTo(s.ToString(), typeof(string)), "Can't convert string");
            Assert.AreEqual(uri, ReflectionUtil.ConvertTo(uri.ToString(), typeof(UriPartial)), "Can't convert enum");
        }

        [Test]
        [ExpectedException(typeof(FormatException))]
        public void ConvertToInvalidValue()
        {
            ReflectionUtil.ConvertTo("blah blah", typeof(int));
        }

        [Test]
        [ExpectedException(typeof(NotSupportedException))]
        public void ConvertToInvalidType()
        {
            ReflectionUtil.ConvertTo("blah blah", typeof(FieldInfo));
        }

        [Test]
        public void GetFieldWithOutAttributes()
        {
            NamedObject n = new NamedObject();
            FieldInfoCollection fi = ReflectionUtil.GetFieldsWithOutAttributes(n.GetType(), typeof(XmlElementAttribute), typeof(XmlIgnoreAttribute));
            Assert.AreEqual(0, fi.Count, "Get fields without attributes found attributes even though they are marked!");
            fi = ReflectionUtil.GetFieldsWithOutAttributes(n.GetType(), typeof(AssemblyFlagsAttribute), typeof(AssemblyKeyNameAttribute));
            Assert.AreEqual(2, fi.Count, "Getfields without attributes didn't find all the fields, even though they are marked with other attribute");
            fi = ReflectionUtil.GetFieldsWithOutAttributes(n.GetType(), typeof(XmlIgnoreAttribute));
            Assert.AreEqual(1, fi.Count, "GetFields without attributes didn't filter correctly");
        }

        [Test]
        public void SetName()
        {
            NamedObject n = new NamedObject();
            ReflectionUtil.SetName(n, "New Name");
            Assert.AreEqual("New Name", n.name, "Can't set the name when it's a field");
            TestForReflection t = new TestForReflection();
            ReflectionUtil.SetName(t, "New Name (2)");
            Assert.AreEqual("New Name (2)", t.Name, "Can't set the name when it's a property");

            //This to check that we don't have an exception on fields without names
            ReflectionUtil.SetName(new object(), "object's name");
        }

        [Test]
        public void RemoveFromArray()
        {
            TestForReflection t = new TestForReflection();
            FieldInfo fi = t.GetType().GetField("stringArray");
            ReflectionUtil.RemoveFromArray(fi, t, 0);
            Assert.AreEqual(1, t.stringArray.Length, "Didn't remove from array!");
            Assert.AreEqual("rahien", t.stringArray[0], "Removed the wrong element");
        }

        [Test]
        public void GetPropertiesDictionary()
        {
            TestForReflection t = new TestForReflection();
            IDictionary dic = ReflectionUtil.GetPropertiesDictionary(t);
            Assert.AreEqual(5, dic.Count);
            Assert.AreEqual("232", dic["Integer"], "Couldn't get the integer type property!");
            Assert.AreEqual("FooBar", dic["Name"], "Couldn't get the string property");
            Assert.AreEqual(typeof(String[]), dic["StringArray"].GetType());
        }

        [Test]
        public void GetFieldsDictionary()
        {
            TestForReflection t = new TestForReflection();
            IDictionary dic = ReflectionUtil.GetFieldsDictionary(t);
            Assert.AreEqual(5, dic.Count);
            Assert.AreEqual(232, dic["integer"], "Couldn't get the integer type field!");
            Assert.AreEqual("FooBar", dic["name"], "Couldn't get the string field");
            Assert.AreEqual("String[]", dic["stringArray"], "Couldn't get the string array field");
        }

        [Test]
        public void HasValue()
        {
            Assert.IsFalse(ReflectionUtil.HasValue(null), "HasValue returned true for null value");
            Assert.IsFalse(ReflectionUtil.HasValue(new object[0]), "HasValue returned true for empty array");
            Assert.IsFalse(ReflectionUtil.HasValue(new ArrayList()), "HasValue returned true for empty list");
            Assert.IsFalse(ReflectionUtil.HasValue(""), "Empty string returned true");
            Assert.IsTrue(ReflectionUtil.HasValue(new object[1]), "HasValue returned false for array with element");
            Assert.IsTrue(ReflectionUtil.HasValue(new object()), "HasValue returned false for non null reference");
            Assert.IsTrue(ReflectionUtil.HasValue("full"), "Non-Empty string returned false");
        }

        internal class NamedObject
        {
            [XmlElement()]
            public string name = "names";

            [XmlIgnore()]
            public string marked = "null";

        }

        internal class TestForReflection
        {
            public const string InitName = "FooBar";
            public int integer = 232;
            public string val = "value";
            public string[] stringArray = { "ayende", "rahien" };
            public string name = InitName;

            public int Integer
            {
                get { return integer; }
            }

            public string[] StringArray
            {
                get { return stringArray; }
            }


            public string Name
            {
                get { return name; }
                set { name = value; }
            }


            public string Value
            {
                get { return val; }
            }

            public string NullProperty
            {
                get { return null; }
            }

        }
    }
}