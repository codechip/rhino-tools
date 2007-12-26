import file from MultiFileConfig_Main.boo
import file from MultiFileConfig_Main2.boo

DefineComponent()
DefineComponent2()

extend "email_sender":
	Host = 'example.dot.org', To = ( 'Kaitlyn', 'Matthew', 'Lauren' )

extend "email_sender2":
	Host = 'example.dot.org', To = ( 'Kaitlyn', 'Matthew', 'Lauren' )

extend 'email_sender3':
	@startable = true
	@tag1 = 'important', tag2 = 'priority'
	lifestyle Pooled, InitialPoolSize = 10, MaxPoolSize = 100
	configuration:
		hosts(list, item: host) = ['rhino', 'orca']