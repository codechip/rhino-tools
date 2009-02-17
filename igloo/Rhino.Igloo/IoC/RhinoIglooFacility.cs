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

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web;
using Castle.Core;
using Castle.Core.Configuration;
using Castle.MicroKernel.Facilities;
using log4net;
using Castle.MicroKernel.Registration;

namespace Rhino.Igloo
{
    /// <summary>
    /// Setup the container so we can get Bijection support for controllers and views
    /// </summary>
    public class RhinoIglooFacility : AbstractFacility
    {
        private IInjectedEntityStrategy[] strategies;

        //order is important. if input is before inputs then arrays will be missed. 
        //if ThrowingInjectedMemberStrategy is not last then an exception will be thrown.
        private static readonly IInjectedMemberStrategy[] injectedMemberStrategies = new IInjectedMemberStrategy[]
            {
                new InputsInjectedMemberStrategy(),
                new InputInjectedMemberStrategy(),
                new SessionInjectedMemberStrategy(),
                new FlashInjectedMemberStrategy(),
                new ThrowingInjectedMemberStrategy()
            };

        private static readonly ILog logger = LogManager.GetLogger(typeof(RhinoIglooFacility));

        private Assembly[] assemblies = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="RhinoIglooFacility"/> class.
        /// </summary>
        /// <param myName="assemblies">The assemblies.</param>
        public RhinoIglooFacility(params Assembly[] assemblies)
        {
            this.assemblies = assemblies;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RhinoIglooFacility"/> class.
        /// </summary>
        public RhinoIglooFacility()
        {
        }

        /// <summary>
        /// The custom initialization for the Facility.
        /// </summary>
        /// <remarks>It must be overriden.</remarks>
        protected override void Init()
        {
            strategies = new IInjectedEntityStrategy[]
            {
                new InjectedEntityUsingEagerLoadStrategy(Kernel),
                new InjectedEntityUsingFetchingStrategyStrategy(Kernel),
                new InjectedEntityUsingGetMethodStrategy(Kernel)
            };

            Kernel.Resolver.AddSubResolver(new ContextResolver(Kernel));
            Kernel.Resolver.AddSubResolver(new LoggingResolver());

            if (assemblies == null)
            {
                if (FacilityConfig == null)
                {
                    logger.Fatal("Configuration for Bijection Facility not found.");
                    throw new FacilityException("Sorry, but the Bijection Facility depends on a proper configuration node.");
                }

                IConfiguration assemblyConfig = FacilityConfig.Children["assemblies"];

                if (assemblyConfig == null || assemblyConfig.Children.Count == 0)
                {
                    logger.Fatal("No assembly specified on Bijection Facility config.");

                    throw new FacilityException("You need to specify at least one assembly that contains " +
                        "the Inject decorated classes. For example, <assemblies><item>MyAssembly</item></assemblies>");
                }

                ConfigurationCollection assembliyConfigNodes = assemblyConfig.Children;

                this.assemblies = new Assembly[assembliyConfigNodes.Count];

                for (int i = 0; i < assembliyConfigNodes.Count; i++)
                {
                    IConfiguration assemblyNode = assembliyConfigNodes[i];
                    assemblies[i] = ObtainAssembly(assemblyNode.Value);
                }
            }

            // Added a ComponentCache Repository to track it
            Kernel.AddComponent("component.repository", typeof(ComponentRepository));
            Kernel.ComponentModelBuilder.AddContributor(new InjectionInspector());
            Kernel.ComponentCreated += Kernel_ComponentCreated;
            RegisterViewComponent();
            RegisterControllers();
        }

        private Assembly ObtainAssembly(String assemblyName)
        {
            logger.DebugFormat("Loading assembly '{0}' for RhinoIglooFacility", assemblyName);
            return Assembly.Load(assemblyName);
        }

        private void RegisterControllers()
        {
            Kernel.AddComponent("BaseController", typeof(BaseController));
            foreach (Assembly assembly in assemblies)
            {
                foreach (Type type in assembly.GetExportedTypes())
                {
                    if (!typeof(BaseController).IsAssignableFrom(type))continue; 
                    if (AttributeUtil.ShouldSkipAutomaticRegistration(type)) continue;
                    Kernel.AddComponent(type.Name, type, LifestyleType.Transient);
                }
            }
        }

        /// <summary>
        /// Registers the view component.
        /// </summary>
        private void RegisterViewComponent()
        {
            ComponentRepository repository = (ComponentRepository)Kernel[typeof(ComponentRepository)];

            foreach (Assembly assembly in assemblies)
            {
                foreach (Type type in assembly.GetExportedTypes())
                {
                    // MasterPage / Web page / UserControl
                    if (!AttributeUtil.IsView(type)) continue;
                    repository.AddComponent(type);
                }
            }
        }

        private void Kernel_ComponentCreated(ComponentModel model, object instance)
        {
            IDictionary<InjectAttribute, PropertyInfo> members = model.ExtendedProperties[InjectionInspector.InMembers] as IDictionary<InjectAttribute, PropertyInfo>;
            IDictionary<InjectEntityAttribute, PropertyInfo> entitiesToInject = model.ExtendedProperties[InjectionInspector.InEntityMembers] as IDictionary<InjectEntityAttribute, PropertyInfo>;

            if (members != null) InjectMembers(instance, members);
            if (entitiesToInject != null) InjectEntities(instance, entitiesToInject);
        }

        /// <summary>
        /// Injects the members.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="membersToInject">The members to inject.</param>
        private static void InjectMembers(object instance, IDictionary<InjectAttribute, PropertyInfo> membersToInject)
        {
            if (membersToInject == null) return;
            if (membersToInject.Count == 0) return;
            foreach (InjectAttribute attribute in membersToInject.Keys)
            {
                foreach (IInjectedMemberStrategy strategy in injectedMemberStrategies)
                {
                    PropertyInfo property = membersToInject[attribute];
                    try
                    {
                        if (!strategy.IsSatisfiedBy(attribute.Scope)) continue;
                        strategy.SetValueFor(instance, attribute.Name, property);
                        break;
                    }
                    catch (Exception e)
                    {
                        string message = string.Format("Failed to convert {0} to type {1}", attribute.Name, property.PropertyType);
                        logger.Debug(message, e);
                    }
                }
            }
        }

        private void InjectEntities(object instance, IDictionary<InjectEntityAttribute, PropertyInfo> entitiesToInject)
        {
            foreach (InjectEntityAttribute attribute in entitiesToInject.Keys)
            {
                PropertyInfo property = entitiesToInject[attribute];
                object key = ConvertKey(instance, attribute);
                if (key == null) continue;

                foreach (IInjectedEntityStrategy strategy in strategies)
                {
                    if (!strategy.IsSatisfiedBy(attribute, property)) continue;
                    logger.DebugFormat("Using {0} to fetch {1} with an id of {2} ({3}).", strategy, property.PropertyType, key, key.GetType());

                    object entity = strategy.GetValueFor(key, attribute, property);
                    property.SetValue(instance, entity, null);
                    break;
                }
            }
        }

        private static object ConvertKey(object instance, InjectEntityAttribute attribute)
        {
            if (Scope.Input[attribute.Name] == null) return null;
            try
            {
                return ConversionUtil.ConvertTo(attribute.IdType, Scope.Input[attribute.Name]);
            }
            catch (Exception e)
            {
                logger.Debug(string.Format("Failed to convert ID ({0}) for {1} to integer.", attribute.Name, instance), e);
                return null;
            }
        }
    }
}
