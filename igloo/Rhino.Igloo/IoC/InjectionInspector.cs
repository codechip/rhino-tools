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
		#endregion
	}
}
