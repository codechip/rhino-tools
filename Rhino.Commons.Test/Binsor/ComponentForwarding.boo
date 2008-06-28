import Rhino.Commons.Test.Components
import Rhino.Commons.Test.Binsor from Rhino.Commons.Test

for type in AllTypes("Rhino.Commons.NHibernate") \
	.Where({ t as System.Type | t.Name.Contains("NHRepository") }):
	component "nh.repos", type.GetFirstInterface(), type

component 'FubarRepository', IFubarRepository, FubarRepository

Kernel.RegisterHandlerForwarding(IRepository of Fubar, 'FubarRepository')