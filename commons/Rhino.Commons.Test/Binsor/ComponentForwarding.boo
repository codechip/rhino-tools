import Rhino.Commons.Test.Components
import Rhino.Commons.Test.Binsor from Rhino.Commons.Test

for repos in AllTypes("Rhino.Commons.NHibernate") \
	.Where({ t as System.Type | t.Name.Contains("NHRepository") }):
	component "nh.repos" is repos < repos.GetFirstInterface()

component "fubar" is FubarRepository < IFubarRepository, IRepository of Fubar

component FakeRepository of Fubar < IRepository of Fubar

extend "fubar" < FakeRepository of Fubar