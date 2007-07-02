import NHibernate.Cfg from NHibernate
import System
import System.IO
import System.Reflection
import PayRollSystem
import PayRollSystem.Common

Component("nhibernate_unit_of_work", IUnitOfWorkFactory, NHibernateUnitOfWorkFactory)

Component("nhibernate_repository", IRepository, NHRepository)
Component("container_selector", ContainerSelector)

modelAssembly = Assembly.Load("PayRollSystem")
resourcesNames = modelAssembly.GetManifestResourceNames()

for configResource in resourcesNames:
	configResourceName = configResource.Replace("PayRollSystem.", "").Replace(".hibernate.cfg.xml","")
	continue if not configResource.EndsWith(".cfg.xml")
	print "Build child configuration for ${configResourceName}"
	child = RhinoContainer(IoC.Container)
	using IoC.UseLocalContainer(child):
		Component("nhibernate_unit_of_work", IUnitOfWorkFactory, NHibernateUnitOfWorkFactory)
		Component("nhibernate_repository", IRepository, NHRepository)
		cfg = NHibernate.Cfg.Configuration()
		sessionFactory = cfg.Configure(modelAssembly, configResource).BuildSessionFactory()
		IoC.Container.Kernel.AddComponentInstance("nhibernate_cfg", cfg);
		IoC.Resolve(IUnitOfWorkFactory).RegisterSessionFactory( sessionFactory )
		
	#need to remove both .cfg and .xml
	containerName = Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(configResourceName))
	IoC.Container.Resolve(ContainerSelector).Register(containerName, child)
	
	
	
	
	
	
	
	
	
	
	
	# for later
	#overtimeCalc = "PayRollSystem.${configResourceName}.${configResourceName}OverTimeCalculator, PayRollSystem"
	#Component(overtimeCalc, IOverTimeCalculator, Type.GetType(overtimeCalc) )