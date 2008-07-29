using System.Reflection;
using Rhino.Commons.Facilities;
using MbUnit.Framework;
using System;

namespace Rhino.Commons.Test.Facilities
{
    [TestFixture]
    public class When_instantiating_a_NHibernateUnitOfWorkFacilityConfig
    {
        [Test]
        public void should_have_zero_assemblies_with_empty_ctor()
        {
            Assert.IsEmpty(new NHibernateUnitOfWorkFacilityConfig().Assemblies);
        }

        [Test]
        public void should_include_one_assembly_when_loaded_by_name()
        {
            Assembly assembly = GetType().Assembly;
            Assembly[] assemblies = new NHibernateUnitOfWorkFacilityConfig(assembly.FullName).Assemblies;

            Assert.AreEqual(1, assemblies.Length);
            Assert.AreEqual(assembly, assemblies[0]);
        }

        [Test]
        public void should_include_one_assembly()
        {
            Assembly assembly = GetType().Assembly;
            Assembly[] assemblies = new NHibernateUnitOfWorkFacilityConfig(assembly).Assemblies;

            Assert.AreEqual(1, assemblies.Length);
            Assert.AreEqual(assembly, assemblies[0]);
        }

        [Test]
        public void should_have_zero_entities()
        {
            Assert.IsEmpty(new NHibernateUnitOfWorkFacilityConfig().Entities);
        }

        [Test]
        public void should_have_a_default_predicate_to_register_no_entities()
        {
            Assert.IsFalse(new NHibernateUnitOfWorkFacilityConfig().IsCandidateForRepository.Invoke(typeof(object)));
        }

        [Test]
        public void should_use_the_default_nhibernate_config_file()
        {
            Assert.AreEqual("hibernate.cfg.xml", new NHibernateUnitOfWorkFacilityConfig().NHibernateConfigurationFile);
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void will_throw_an_excpetion_for_a_null_string()
        {
            string nullstring = null;
            new NHibernateUnitOfWorkFacilityConfig(nullstring);
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void will_throw_an_excpetion_for_an_empty_string()
        {
            new NHibernateUnitOfWorkFacilityConfig(string.Empty);
        }
    }

    [TestFixture]
    public class When_adding_assemblies_to_NHibernateUnitOfWorkFacilityConfig
    {
        Assembly thisAssembly, thatAssembly;

        [SetUp]
        public void Before_each_test()
        {
            thisAssembly = GetType().Assembly;
            thatAssembly = typeof(NHibernateUnitOfWorkFacilityConfig).GetType().Assembly;
        }

        [Test]
        public void should_add_unique_assemblies_by_name()
        {
            Assembly[] assemblies = CreateSUT()
                .AddAssembly(thisAssembly.FullName)
                .AddAssembly(thisAssembly.FullName)
                .AddAssembly(thatAssembly.FullName)
                .Assemblies;

            Assert.AreEqual(2, assemblies.Length);
            Assert.In(thisAssembly, assemblies);
            Assert.In(thatAssembly, assemblies);
        }

        [Test]
        public void should_add_unique_assemblies()
        {
            Assembly[] assemblies = CreateSUT()
                .AddAssembly(thisAssembly)
                .AddAssembly(thisAssembly)
                .AddAssembly(thatAssembly)
                .Assemblies;

            Assert.AreEqual(2, assemblies.Length);
            Assert.In(thisAssembly, assemblies);
            Assert.In(thatAssembly, assemblies);
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void will_throw_an_excpetion_for_null()
        {
            string nullstring = null;
            CreateSUT().AddAssembly(nullstring);
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void will_throw_an_excpetion_for_an_empty_string()
        {
            CreateSUT().AddAssembly(string.Empty);
        }

        private NHibernateUnitOfWorkFacilityConfig CreateSUT()
        {
            return new NHibernateUnitOfWorkFacilityConfig();
        }
    }

    [TestFixture]
    public class When_adding_entities_to_NHibernateUnitOfWorkFacilityConfig
    {
        [Test]
        public void should_add_unique_types()
        {
            Type _int = typeof(int);
            Type _string = typeof(string);
            Type[] entities = CreateSUT()
                .AddEntity(_int)
                .AddEntity(_int)
                .AddEntity(_string)
                .Entities;

            Assert.AreEqual(2, entities.Length);
            Assert.In(_int, entities);
            Assert.In(_string, entities);
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void will_throw_an_excpetion_for_null()
        {
            CreateSUT().AddEntity(null);
        }

        private NHibernateUnitOfWorkFacilityConfig CreateSUT()
        {
            return new NHibernateUnitOfWorkFacilityConfig();
        }
    }

    [TestFixture]
    public class When_changing_the_NH_config_for_NHibernateUnitOfWorkFacilityConfig
    {
        [Test]
        public void should_override_default_value()
        {
            string file = "foo";
            Assert.AreEqual(file, CreateSUT().NHibernateConfiguration(file).NHibernateConfigurationFile);
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void will_throw_an_excpetion_for_null_string()
        {
            CreateSUT().NHibernateConfiguration(null);
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void will_throw_an_excpetion_for_an_empty_string()
        {
            CreateSUT().NHibernateConfiguration(string.Empty);
        }

        private NHibernateUnitOfWorkFacilityConfig CreateSUT()
        {
            return new NHibernateUnitOfWorkFacilityConfig();
        }
    }

    [TestFixture]
    public class When_changing_the_entity_registry_to_NHibernateUnitOfWorkFacilityConfig
    {
        [Test]
        public void should_override_the_default_predicate()
        {
            Predicate<Type> typeIsAlwaysTrue = delegate(Type t) { return true; };
            NHibernateUnitOfWorkFacilityConfig config = CreateSUT().RegisterEntitiesWhere(typeIsAlwaysTrue);
            Assert.AreEqual(typeIsAlwaysTrue, config.IsCandidateForRepository);
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void will_throw_an_excpetion_for_null()
        {
            CreateSUT().RegisterEntitiesWhere(null);
        }

        private NHibernateUnitOfWorkFacilityConfig CreateSUT()
        {
            return new NHibernateUnitOfWorkFacilityConfig();
        }
    }
}
