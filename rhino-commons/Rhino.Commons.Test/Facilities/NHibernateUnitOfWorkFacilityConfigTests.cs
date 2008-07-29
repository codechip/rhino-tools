using System.Reflection;
using Rhino.Commons.Facilities;
using MbUnit.Framework;
using System;

namespace Rhino.Commons.Test.Facilities
{
    [TestFixture]
    public class NHibernateUnitOfWorkFacilityConfigTests
    {
        [Test]
        public void A_new_instance_should_have_zero_assemblies()
        {
            Assert.IsEmpty(new NHibernateUnitOfWorkFacilityConfig().Assemblies);
        }

        [Test]
        public void A_new_instance_can_include_one_assembly()
        {
            Assembly assembly = GetType().Assembly;
            Assembly[] assemblies = new NHibernateUnitOfWorkFacilityConfig(assembly.FullName).Assemblies;

            Assert.AreEqual(1, assemblies.Length);
            Assert.AreEqual(assembly, assemblies[0]);
        }

        [Test]
        public void A_new_instance_should_have_zero_entities()
        {
            Assert.IsEmpty(new NHibernateUnitOfWorkFacilityConfig().Entities);
        }

        [Test]
        public void A_new_instance_should_not_register_entities_to_repositories()
        {
            Assert.IsFalse(new NHibernateUnitOfWorkFacilityConfig().ShouldRegisterEntitiesToRepository);
        }

        [Test]
        public void A_new_instance_should_use_the_default_nhibernate_config_file()
        {
            Assert.AreEqual("hibernate.cfg.xml", new NHibernateUnitOfWorkFacilityConfig().NHibernateConfigurationFile);
        }

        [Test]
        public void Should_be_able_to_add_additional_assembly()
        {
            Assembly assembly = GetType().Assembly;
            Assembly[] assemblies = new NHibernateUnitOfWorkFacilityConfig(assembly.FullName)
                .AddAssembly(typeof(NHibernateUnitOfWorkFacilityConfig).GetType().Assembly.FullName)
                .Assemblies;

            Assert.AreEqual(2, assemblies.Length);
        }

        [Test]
        public void Should_include_exactly_one_instance_of_an_assembly()
        {
            Assembly assembly = GetType().Assembly;
            Assembly[] assemblies = new NHibernateUnitOfWorkFacilityConfig(assembly.FullName)
                .AddAssembly(assembly.FullName)
                .Assemblies;

            Assert.AreEqual(1, assemblies.Length);
            Assert.AreEqual(assembly, assemblies[0]);
        }


        [Test]
        public void Should_be_able_to_add_an_entity()
        {
            Type _int = typeof(int);
            Type _string = typeof(string);
            Type[] entities = new NHibernateUnitOfWorkFacilityConfig()
                .AddEntity(_int)
                .AddEntity(_string)
                .Entities;

            Assert.AreEqual(2, entities.Length);
            CollectionAssert.Contains(entities, _int);
            CollectionAssert.Contains(entities, _string);
        }

        [Test]
        public void Should_include_exactly_one_instance_of_an_entity()
        {
            Type _int = typeof(int);
            Type[] entities = new NHibernateUnitOfWorkFacilityConfig()
                .AddEntity(_int)
                .AddEntity(_int)
                .Entities;

            Assert.AreEqual(1, entities.Length);
            Assert.AreEqual(_int, entities[0]);
        }

        [Test]
        public void Should_be_able_to_set_flag_to_register_entities()
        {
            NHibernateUnitOfWorkFacilityConfig config = new NHibernateUnitOfWorkFacilityConfig().RegisterEntities(true);
            Assert.IsTrue(config.ShouldRegisterEntitiesToRepository);
        }

        [Test]
        public void Should_be_able_to_change_nhibernate_configuration_file()
        {
            string file = "foo";
            NHibernateUnitOfWorkFacilityConfig config = new NHibernateUnitOfWorkFacilityConfig().NHibernateConfiguration(file);
            Assert.AreEqual(file, config.NHibernateConfigurationFile);
        }
    }
}
