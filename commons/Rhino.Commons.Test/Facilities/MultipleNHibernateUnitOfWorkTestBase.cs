using System;
using System.Collections.Generic;
using System.IO;
using Castle.Windsor;
using MbUnit.Framework;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using Rhino.Commons.Facilities;
using Rhino.Commons.Test.Facilities.MutlipleUnitOfWorkArtifacts;

namespace Rhino.Commons.Test.Facilities
{
	public abstract class MultipleNHibernateUnitOfWorkTestBase
	{
		private IUnitOfWork uow;
		private List<SchemaExport> schemas;

		[SetUp]
		public void Setup()
		{
			InitializeIOC();
			uow = UnitOfWork.Start();
			//create databases
			ISession[] sessions = new ISession[] 
			{ 
				UnitOfWork.GetCurrentSessionFor(typeof(DomainObjectFromDatabase1)), 
				UnitOfWork.GetCurrentSessionFor(typeof(DomainObjectFromDatabase2)) 
			};
			schemas[0].Execute(false, true, false, sessions[0].Connection, null);
			schemas[1].Execute(false, true, false, sessions[1].Connection, null);

			//insert test data and evict from session
			With.Transaction(delegate
			{
				sessions[0].Evict(Repository<DomainObjectFromDatabase1>.Save(new DomainObjectFromDatabase1("foo")));
				sessions[1].Evict(Repository<DomainObjectFromDatabase2>.Save(new DomainObjectFromDatabase2("bar")));
			});
		}

		[TearDown]
		public void Teardown()
		{
			uow.Dispose();
			//remove databases
			schemas.ForEach(delegate(SchemaExport schema) { schema.Drop(false, true); });
			IoC.Reset();
		}

		private void InitializeIOC()
		{
			NHibernateUnitOfWorkFacilityConfig[] configs = CreateRules();
			IoC.Initialize(
				new WindsorContainer()
                    .AddFacility("Multiple.Units.Of.Work", new MultipleNHibernateUnitOfWorkFacility(configs)));

			//load schemas to create databases
			schemas = new List<SchemaExport>();
            foreach (NHibernateUnitOfWorkFacilityConfig config in configs)
			{
                Configuration cfg = new Configuration().Configure(config.NHibernateConfigurationFile);
				foreach (Type entity in config.Entities)
				{
                    cfg.AddClass(entity);
				}
				schemas.Add(new SchemaExport(cfg));
			}
		}

        private static NHibernateUnitOfWorkFacilityConfig[] CreateRules()
		{
            string directory = Path.Combine(System.Environment.CurrentDirectory, @"Facilities\MutlipleUnitOfWorkArtifacts");

            return new NHibernateUnitOfWorkFacilityConfig[] {
                new NHibernateUnitOfWorkFacilityConfig()
                    .AddEntity(typeof(DomainObjectFromDatabase1))
                    .NHibernateConfiguration(Path.Combine(directory, "Database1.cfg.xml"))
                    .RegisterEntitiesWhere(delegate {return true; }),
                new NHibernateUnitOfWorkFacilityConfig()
                    .AddEntity(typeof(DomainObjectFromDatabase2))
                    .NHibernateConfiguration(Path.Combine(directory, "Database2.cfg.xml"))
                    .RegisterEntitiesWhere(delegate {return true; })
            };
		}
	}
}