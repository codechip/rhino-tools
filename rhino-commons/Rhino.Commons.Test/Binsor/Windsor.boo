import Rhino.Commons
import Rhino.Commons.Test.Components from Rhino.Commons.Test
import Rhino.Commons.Test.Binsor

# generic type registration
Component("defualt_repository", IRepository, NHRepository)
# sepcialized generic type registration (a bit ugly, I'll admit
Component("customer_repository", 
	typeof(IRepository).MakeGenericType(Fubar), 
	typeof(FakeRepository).MakeGenericType(Fubar))
	

email = Component("email_sender", ISender, EmailSender)
email.Host = "example.dot.org"