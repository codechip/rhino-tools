using System.Xml;

namespace Rhino.Security.Configuration.NH
{
  /// <summary>Implementation of <see cref="INHibernateMappingBuilder"/> that
  /// builds a mapping document for the <see cref="Rhino.Security.Framework"/>.</summary>
  public class SecurityModelMappingBuilder : INHibernateMappingBuilder
  {
    private readonly INHibernateSecurityConfiguration configuration;

    /// <summary>Creates a <see cref="SecurityModelMappingBuilder"/></summary>
    /// <param name="configuration"></param>
    public SecurityModelMappingBuilder(INHibernateSecurityConfiguration configuration)
    {
      this.configuration = configuration;
    }

    private object[] BuildHbmStringFormatValues()
    {
      return new object[]
             {
               configuration.UserType.AssemblyQualifiedName,
               SchemaAndTableName("UsersGroup"),
               SchemaAndTableName("UsersToUsersGroup"),
               SchemaAndTableName("UsersGroupHierarchy"),
               SchemaAndTableName("UsersGroupHierarchy"),
               SchemaAndTableName("Permission"),
               SchemaAndTableName("Operation"),
               SchemaAndTableName("EntityType"),
               SchemaAndTableName("EntityReference"),
               SchemaAndTableName("EntitiesGroup"),
               SchemaAndTableName("EntityReferenceToEntitiesGroup")
             };
    }

    private string SchemaAndTableName(string tableName)
    {
      if (configuration.TableStructure == SecurityTableStructure.Prefix)
      {
        return "table=\"security_" + tableName + "\"";
      }
      return "schema=\"security\" table=\"" + tableName + "\"";
    }

    /// HACK: Fix this
    /// <summary>Builds the NHibernate Mapping Document</summary>
    /// <returns>An <see cref="XmlDocument"/> that represents the mapping.</returns>
    public virtual XmlDocument Build()
    {
      // 0  = IUser Type Full Name
      // 1  = UsersGroup Schema and Table Name
      // 2  = UsersGroup To Users Schema and Table Name
      // 3  = UsersGroup To Children Schema and Table Name
      // 4  = 
      // 5  =
      // 6  =
      // 7  =
      // 8  =
      // 9  =
      // 10 =
      object[] hbmValues = BuildHbmStringFormatValues();
      string xmlString = string.Format(
        @"
<hibernate-mapping xmlns=""urn:nhibernate-mapping-2.2""
                   namespace=""Rhino.Security.NH""
                   assembly=""Rhino.Security.NHibernate""
                   default-access=""property""
                   default-lazy=""true"" 
                   default-cascade=""none"" >

  <class name=""UsersGroup"" {1}>
    <id name=""Id"" type=""guid"">
      <generator class=""guid.comb"" />
    </id>
    
    <property name=""Name"" type=""string"" length=""255"" not-null=""true"" unique=""true"" update=""false"" />
    <many-to-one name=""Parent"" column=""ParentId"" class=""UsersGroup""  />
    <set name=""DirectChildren"" inverse=""true"" generic=""true"">
      <key column=""ParentId"" />
      <one-to-many class=""UsersGroup"" />
    </set>
    
    <set name=""Users"" generic=""true"" inverse=""false"" lazy=""true"" {2} >
      <key>
        <column name=""GroupId"" not-null=""true"" />
      </key>
      <many-to-many class=""{0}"" column=""UserId"" />
    </set>

    <set name=""AllChildren"" generic=""true"" inverse=""true"" {3} >
      <key>
        <column name=""ParentGroupId"" not-null=""true"" />
      </key>
      <many-to-many class=""UsersGroup"">
        <column name=""ChildGroupId"" not-null=""true"" />
      </many-to-many>
    </set>

    <set name=""AllParents"" generic=""true"" inverse=""false"" {4} >
      <key>
        <column name=""ChildGroupId"" not-null=""true"" />
      </key>
      <many-to-many class=""UsersGroup"">
        <column name=""ParentGroupId"" not-null=""true"" />
      </many-to-many>
    </set>
  </class>

  <class name=""Permission"" {5} >
    <id name=""Id"" type=""guid"">
      <generator class=""guid.comb"" />
    </id>
    <many-to-one name=""Operation"" class=""Operation"" column=""OperationId"" not-null=""true"" />
    <property name=""Allow"" type=""boolean"" not-null=""true"" />
    <property name=""Level"" type=""int"" not-null=""true"" />
    <property name=""EntitySecurityKey"" type=""guid"" not-null=""false"" />
    <many-to-one name=""User"" class=""{0}"" column=""UserId"" not-null=""false"" />
    <many-to-one name=""UsersGroup"" class=""UsersGroup"" column=""UsersGroupId"" not-null=""false"" />
    <many-to-one name=""EntitiesGroup"" class=""EntitiesGroup"" column=""EntitiesGroupId"" not-null=""false"" />
  </class>

  <class name=""Operation"" {6} >
    <id name=""Id"" type=""guid"">
      <generator class=""guid.comb"" />
    </id>
    <property name=""Name"" type=""string"" length=""255"" not-null=""true"" unique=""true"" update=""false"" />    
    <many-to-one name=""Parent"" class=""Operation"" column=""ParentId"" />
    <property name=""Comment"" type=""string"" length=""255"" />

    <set name=""Children"" inverse=""true"" generic=""true"" {6} >
      <key column=""ParentId"" />
      <one-to-many class=""Operation"" />
    </set>
  </class>

  <class name=""EntityType"" {7} >
    <id name=""Id"" type=""guid"">
      <generator class=""guid.comb"" />
    </id>
    <property name=""Name"" type=""string"" length=""255"" not-null=""true"" unique=""true"" update=""false"" />
  </class>

  <class name=""EntityReference"" {8}>
    <id name=""Id"" type=""guid"">
      <generator class=""guid.comb"" />
    </id>
    <property name=""EntitySecurityKey"" type=""guid"" not-null=""true"" unique=""true"" />
    <many-to-one name=""Type"" class=""EntityType"" column=""EntityTypeId"" not-null=""true"" />
  </class>

  <class name=""EntitiesGroup"" {9} >
    <id name=""Id"" type=""guid"">
      <generator class=""guid.comb"" />
    </id>
    <property name=""Name"" type=""string"" length=""255"" not-null=""true"" unique=""true"" update=""false"" />
    <set name=""Entities"" generic=""true"" {10} >
      <key>
        <column name=""EntitiesGroupId"" not-null=""true"" />
      </key>
      <many-to-many class=""EntityReference"" column=""EntityReferenceId"" />
    </set>
  </class>
</hibernate-mapping>
",
        hbmValues);

      XmlDocument xmlDocument = new XmlDocument();
      xmlDocument.InnerXml = xmlString;
      return xmlDocument;
    }


  }
}