using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using Castle.Core;
using Castle.MicroKernel;
using Castle.MicroKernel.ModelBuilder;


namespace Extranet.Infrastructure.Web
{
    /// <summary>
    /// Retrieves injected/outjected properties
    /// </summary>
    public class BijectionInspector : IContributeComponentModelConstruction
    {
        /// <summary>
        /// In members token
        /// </summary>
        public const string InMembers = "_IN_MEMBERS_";

        /// <summary>
        /// Out members token
        /// </summary>
        public const string OutMembers = "_OUT_MEMBERS_";

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
            RetrieveOutMembers(model);
        }

        #endregion

        private void RetrieveInMembers(ComponentModel model)
        {
            PropertyInfo[] properties = model.Implementation.GetProperties(BINDING_FLAGS_SET);

            IDictionary<InjectAttribute, PropertyInfo> inMembers = new Dictionary<InjectAttribute, PropertyInfo>();

            for (int i = 0; i < properties.Length; i++)
            {
                InjectAttribute injectAttribute = AttributeUtil.GetInjectAttribute(properties[i]);
                if (injectAttribute != null)
                {
                    if(properties[i].GetSetMethod().IsVirtual==false)
                    {
                        throw new InvalidOperationException("Properties with [Inject] must be virtual");
                    }
                    if (injectAttribute.Name.Length == 0)
                    {
                        injectAttribute.Name = properties[i].Name;
                    }
                    MethodInfo methodInfo = properties[i].GetSetMethod();
                    if (methodInfo == null)
                    {
                        throw new ConfigurationErrorsException("Outject attribute requires a set method on properties " + properties[i].Name + " in model " + model.Name);
                    }

                    inMembers.Add(injectAttribute, properties[i]);
                }
            }
            model.ExtendedProperties[InMembers] = inMembers;
        }

        private void RetrieveOutMembers(ComponentModel model)
        {
            PropertyInfo[] properties = model.Implementation.GetProperties(BINDING_FLAGS_SET);

            IDictionary<OutjectAttribute, PropertyInfo> outMembers = new Dictionary<OutjectAttribute, PropertyInfo>();

            for (int i = 0; i < properties.Length; i++)
            {
                OutjectAttribute outjectAttribute = AttributeUtil.GetOutjectAttribute(properties[i]);
                if (outjectAttribute != null)
                {
                    if (properties[i].GetSetMethod().IsVirtual == false)
                    {
                        throw new InvalidOperationException("Properties with [Outject] must be virtual");
                    } 
                    if (outjectAttribute.Name.Length == 0)
                    {
                        outjectAttribute.Name = properties[i].Name;
                    }
                    MethodInfo methodInfo = properties[i].GetGetMethod();
                    if (methodInfo==null)
                    {
                        throw new ConfigurationErrorsException("Outject attribute requires a get method on properties "+properties[i].Name+" in model "+model.Name);
                    }
                    outMembers.Add(outjectAttribute, properties[i]);
                }
            }
            model.ExtendedProperties[OutMembers] = outMembers;
        }

        private static bool NeedBijection(ComponentModel model)
        {
            bool needBijection = false;
            IDictionary<InjectAttribute, PropertyInfo> inMembers = (IDictionary<InjectAttribute, PropertyInfo>)model.ExtendedProperties[InMembers];
            if (inMembers != null)
            {
                needBijection = inMembers.Count > 0;
            }
            IDictionary<OutjectAttribute, PropertyInfo> outMembers = (IDictionary<OutjectAttribute, PropertyInfo>)model.ExtendedProperties[OutMembers];
            if (outMembers != null && !needBijection)
            {
                needBijection = outMembers.Count > 0;
            }
            return needBijection;
        }
    }
}
