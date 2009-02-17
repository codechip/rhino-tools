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

import Rhino.Commons.Test.Components from Rhino.Commons.Test
import Rhino.Commons from Rhino.Commons.NHibernate
import Rhino.Commons.Test.Binsor
import Castle.Facilities.Logging

# Facility constructors

Facility("loggerFacility", LoggingFacility, 
	loggingApi: LoggerImplementation.Log4net, 
	configFile: "log4net.config"
	) 
	
# generic type registration
Component("default_repository", IRepository, NHRepository, LifestyleType.Transient)

customer_repository = Component("customer_repository", 
	IRepository of Fubar,  FakeRepository of Fubar,
	inner: @default_repository)


email = Component("email_sender", ISender, EmailSender,
	Host: "example.dot.org")

Component("email_sender2", ISender, EmailSender,
	{ @startable: true,
	  parameters: {
		to: ( "craig", "ayende" ),
		backups: ( @email_sender, email ) 
		} 
	  } )

# making sure that loops work

for i in range(4):
	o = Component("foo_${i}", Fubar)
	o.foo = i
	
Component("fubar1", Fubar,
	{ @factoryId: 'fubar_factory', 
	  @factoryCreate: 'Create',
	  parameters: {
		fields: {
			keymap('fields'): {
				name: 'David Beckham',
				age: 32 
				}
			} 
		} }
	) 
		
Component("fubar2", Fubar,
	{ @factoryId: 'fubar_factory', 
	  @factoryCreate: 'Create',
	  parameters: {
		keyvalues('fields'): {
			name: 'David Beckham',
			age: 32 
			}
		} } 
	) 
