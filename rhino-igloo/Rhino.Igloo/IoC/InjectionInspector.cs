using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using Castle.Core;
using Castle.MicroKernel;
using Castle.MicroKernel.ModelBuilder;


namespace Rhino.Igloo
{
    /// <summary>
    /// Retrieves injected/outjected properties
    /// </summary>
    public class InjectionInspector : IContributeComponentModelConstruction
    {
        /// <summary>
        /// In members token
        /// </summary>
        public const string InMembers = "_IN_MEMBERS_";

        /// <summary>
        /// In entity members token
        /// </summary>
        public const string InEntityMembers = "_IN_ENTITY_MEMBERS_";

        /// <summary>
        /// Binding token
        /// </summary>
        private BindingFlags BINDING_FLAGS_SET
            = BindingFlags.Public
            | BindingFlags.SetProperty
            | BindingFlags.Instance
            | BindingFlags.SetField
            ;
        
        #region IContributeComponentModelConstruction Members

        /// <summary>
        /// Usually the implementation will look in the configuration property
        /// of the model or the service interface, or the implementation looking for
        /// something.
        /// </summary>
        /// <param name="kernel">The kernel instance</param>
        /// <param name="model">The component model</param>
        public void ProcessModel(IKernel kernel, ComponentModel model)
        {
            RetrieveInMembers(model);
        }

        #endregion

        private void RetrieveInMembers(ComponentModel model)
        {
            PropertyInfo[] properties = model.Implementation.GetProperties(BINDING_FLAGS_SET);

            IDictionary<InjectAttribute, PropertyInfo> inMembers = new Dictionary<InjectAttribute, PropertyInfo>();
            IDictionary<InjectEntityAttribute, PropertyInfo> inEntityMembers = new Dictionary<InjectEntityAttribute, PropertyInfo>();

            for (int i = 0; i < properties.Length; i++)
            {
                InjectAttribute injectAttribute = AttributeUtil.GetInjectAttribute(properties[i]);
                if (injectAttribute != null)
                {
                    inMembers.Add(injectAttribute, properties[i]);
                }

                InjectEntityAttribute injectEntityAttribute = AttributeUtil.GetInjectEntityAttribute(properties[i]);
                if (injectEntityAttribute != null)
                {
                   inEntityMembers.Add(injectEntityAttribute, properties[i]);
                }
            }
            model.ExtendedProperties[InMembers] = inMembers;
            model.ExtendedProperties[InEntityMembers] = inEntityMembers;
        }
    }
}
