import Rhino.Commons.Test.Components from Rhino.Commons.Test
import Rhino.Commons from Rhino.Commons.NHibernate
import Rhino.Commons.Test.Binsor
import Castle.Facilities.Logging

def DefineComponent():
	component "email_sender", ISender, EmailSender:
		Host = 'example.dot.org', To = ( 'Kaitlyn', 'Matthew', 'Lauren' )

	component 'email_sender3', ISender, EmailSender