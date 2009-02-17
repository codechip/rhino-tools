import Rhino.Commons.Test.Components from Rhino.Commons.Test
import Rhino.Commons from Rhino.Commons.NHibernate
import Rhino.Commons.Test.Binsor
import Castle.Facilities.Logging

def DefineComponent():
	component "email_sender4", ISender, EmailSender
	
	component "email_sender5", ISender, EmailSender:
		Host = 'example123.dot.org', To = ( 'Lauren' )

	component 'email_sender6', ISender, EmailSender