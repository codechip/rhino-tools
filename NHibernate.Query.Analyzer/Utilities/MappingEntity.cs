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
