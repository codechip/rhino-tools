#region license
// Copyright (c) 2005 - 2007 Ayende Rahien (ayende@ayende.com)
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//     * Neither the name of Ayende Rahien nor the names of its
//     contributors may be used to endorse or promote products derived from this
//     software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion

import System.Reflection
import Rhino.Commons.Test.Components from Rhino.Commons.Test
import Rhino.Commons from Rhino.Commons.NHibernate
import Rhino.Commons.Test.Binsor
import Castle.Facilities.Logging
import Rhino.Commons.Facilities from Rhino.Commons.ActiveRecord
import Castle.Facilities.ActiveRecordIntegration from Castle.Facilities.ActiveRecordIntegration
import Castle.Facilities.FactorySupport from Castle.MicroKernel
import Castle.Facilities.Startable from Castle.MicroKernel
import Castle.Facilities.EventWiring from Castle.MicroKernel

import file from "disposable.boo"

# Facility constructors

facility StartableFacility
facility FactorySupportFacility
facility EventWiringFacility

component EmailListener
component EmailSenderFactory

facility LoggingFacility: 
	loggingApi = LoggerImplementation.Log4net
	configFile = 'log4net.config'

# Facility configuration
	
facility ActiveRecordFacility:
	configuration:
		@isWeb = true, isDebug = true
		assemblies = [ Assembly.Load("Rhino.Commons.NHibernate") ]
		config(keyvalues, item: add):
			show_sql = true
			command_timeout = 5000
			cache.foo.use_query_cache = false
			dialect = 'NHibernate.Dialect.MsSql2005Dialect'
			connection.provider = 'NHibernate.Connection.DriverConnectionProvider'
			connection.driver_class = 'NHibernate.Driver.SqlClientDriver'
			connection.connection_string = 'connectionString1'
		config(keyvalues, item: add):
			@type = Fubar
			dialect = 'NHibernate.Dialect.SQLiteDialect'
			connection.provider = 'NHibernate.Connection.DriverConnectionProvider'
			connection.driver_class = 'NHibernate.Driver.SQLite20Driver'
			connection.connection_string = 'connectionString2'
			connection.release_mode = 'on_close'
	
facility ActiveRecordUnitOfWorkFacility("Rhino.Commons.Binsor")
	
# generic type registration

component 'default_repository', IRepository, NHRepository:
	lifestyle Transient

component 'disposable', System.IDisposable, MyDisposable.Impl

component 'customer_repository', IRepository of Fubar, FakeRepository of Fubar:
	inner = @NHRepository

component 'fubar_repository', IRepository of Fubar, FakeRepository of Fubar:
	inner = @default_repository

component 'email_sender', ISender, EmailSender:
	Host = 'example.dot.org', To = ( 'Kaitlyn', 'Matthew', 'Lauren' )

component 'email_sender2', ISender, EmailSender:
	@startable = true
	parameters:
		to = ( "craig", "ayende" )
		backups = ( @email_sender, )

# making sure that loops work

for i in range(4):
	component "foo_${i}", Fubar:
		foo = i

component 'email_sender3', ISender, EmailSender:
	@startable = true
	@tag1 = 'important', tag2 = 'priority'
	lifestyle Pooled, InitialPoolSize = 10, MaxPoolSize = 100
	configuration:
		hosts(list, item: host) = ['rhino', 'orca']
		protocol
		'configuration'
		vision left = 20, right = 30
		location latitude = 100, longitude = 80
		father = 'Murray'
		name = 'David Beckham', age = 32
		parameters:
			name = 'David Beckham', age = 32
			friends:
				friend = @friend1
				friend = @friend2
			address(keymap, key: key):
				line1 = '124 Fletcher'
				city = 'Valley Stream'
				state = 'NY'
				zipCode = '11512'
			address2(keymap) = { 
				line1: '3747 Dartbrook',
				city: 'Rockwall',
				state: 'TX',
				zipCode: '75032'
				}


component ISender, EmailSender:
	@startable = true
	createUsing @EmailSenderFactory.Create
	wireEvent Sent:
		to @EmailListener.OnSent


for itemName in ["a", "b"]:
	component "someListener.${itemName}", EmailListener
	component "somePublisher.${itemName}", ISender, EmailSender:
		@startable = true
		createUsing @EmailSenderFactory.Create
		wireEvent Sent:
			to "someListener.${itemName}".OnSent
     
     
if Environment == "Binsor2":
	component 'foo_bar', Fubar:
		foo = Environment
		
component "foo_instance", Fubar:
	createUsing Instance
	
component MyComponent:
	parameters:
		someMapping:
			map(keymap):
				key1 = 'value1'
				key2 = 'value2'	