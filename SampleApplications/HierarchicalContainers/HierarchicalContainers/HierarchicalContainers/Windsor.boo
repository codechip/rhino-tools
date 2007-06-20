import HierarchicalContainers
import System.IO

Component("nhibernate_unit_of_work", IUnitOfWorkFactory, NHibernateUnitOfWorkFactory,
	configurationFileName: """..\..\Configurations\hibernate.cfg.xml""")
	
Component("nhibernate_repository", IRepository, NHRepository)
Component("container_selector", ContainerSelector)

for configFile in Directory.GetFiles("""..\..\Configurations""", "*.cfg.xml"):
	continue if Path.GetFileName(configFile) == "hibernate.cfg.xml"
	print "Build child configuration for ${configFile}"
	child = RhinoContainer(IoC.Container)
	using IoC.UseLocalContainer(child):
		Component("nhibernate_unit_of_work", IUnitOfWorkFactory, NHibernateUnitOfWorkFactory,
			configurationFileName: configFile)
		Component("nhibernate_repository", IRepository, NHRepository)
	#need to remove both .cfg and .xml
	containerName = Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(configFile))
	IoC.Container.Resolve(ContainerSelector).Register(containerName, child)