import Rhino.Commons
import Rhino.Commons.Test.Components from Rhino.Commons.Test
import Rhino.Commons.Test.Binsor

# generic type registration
Component(defualt_repository, IRepository, NHRepository)
# sepcialized generic type registration (a bit ugly, I'll admit)
customer_repository = Component("customer_repository", 
	IRepository of Fubar,  FakeRepository of Fubar,
	inner: @defualt_repository)


email = Component("email_sender", ISender, EmailSender,
	Host: "example.dot.org")

# making sure that loops work

for i in range(4):
	o = Component("foo_${i}", Fubar)
	o.foo = i