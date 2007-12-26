import Rhino.Commons.Test.Components from Rhino.Commons.Test
import Rhino.Commons from Rhino.Commons.NHibernate
import Rhino.Commons.Test.Binsor
import Castle.Facilities.Logging

def DefineComponent2():
	component "email_sender", ISender, EmailSender
	
	component "email_sender2", ISender, EmailSender:
		Host = 'example123.dot.org', To = ( 'Lauren' )

	component 'email_sender3', ISender, EmailSender