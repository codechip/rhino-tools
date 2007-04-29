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
using System.Text;

namespace Ayende.NHibernateQueryAnalyzer.Utilities
{
    [Serializable]
    public class MappingEntity
    {
        private SortedList<string,MappingEntityProperty> m_properties = new SortedList<string,MappingEntityProperty>();
        private string m_TableName;
        private string m_EntityName;

        public string TableName
        {
            get { return m_TableName; }
            set { m_TableName = value; }
        }

        public string EntityName
        {
            get { return m_EntityName; }
            set { m_EntityName = value; }
        }

        public SortedList<string,MappingEntityProperty> Properties
        {
            get { return m_properties; }
        }

        public void AddProperty(MappingEntityProperty property)
        {
            m_properties.Add(property.PropertyName, property);
        }

        public void AddProperty(string propertyName, string propertyType, bool isEntityType, string returnedClassName)
        {
             m_properties.Add(propertyName, new MappingEntityProperty(propertyName,propertyType,isEntityType,returnedClassName));
        }
    }

    [Serializable]
    public class MappingEntityProperty
    {
        private string m_PropertyName;
        private string m_PropertyType;
        private bool m_IsEntityType;
        private string m_ReturnClassName;

        public MappingEntityProperty(string propertyName, string propertyType,bool isEntityType, string returnClassName)
        {
            PropertyName = propertyName;
            PropertyType = propertyType;
            m_IsEntityType = isEntityType;
            ReturnClassName = returnClassName;
        }

        public string PropertyName
        {
            get { return m_PropertyName; }
            set { m_PropertyName = value; }
        }

        public string PropertyType
        {
            get { return m_PropertyType; }
            set { m_PropertyType = value; }
        }

        public bool IsEntityType
        {
            get { return m_IsEntityType; }
            set { m_IsEntityType = value; }
        }

        public string ReturnClassName
        {
            get { return m_ReturnClassName; }
            set { m_ReturnClassName = value; }
        }
    }
}
