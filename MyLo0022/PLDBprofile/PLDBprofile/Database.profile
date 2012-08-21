<?xml version="1.0" encoding="utf-8" ?>
<profile dslVersion="1.0.0.0"
       name="DBProfile" displayName="Database Profile"
       xmlns="http://schemas.microsoft.com/UML2.1.2/ProfileDefinition">
  <stereotypes>
    <stereotype name="Schema" displayName="Schema">
      <metaclasses>
        <metaclassMoniker name="/DBProfile/Microsoft.VisualStudio.Uml.Classes.IPackage"/>
      </metaclasses>
      <properties>
        <property name="Name" displayName="Schema Name" defaultValue="dbo">
          <propertyType>
            <externalTypeMoniker name="/DBProfile/System.String" />
          </propertyType>
        </property>
      </properties>
    </stereotype>
    <stereotype name="Table" displayName="Table">
      <metaclasses>
        <metaclassMoniker name="/DBProfile/Microsoft.VisualStudio.Uml.Classes.IClass"/>
      </metaclasses>
    </stereotype>
    <stereotype name="Resource" displayName="Resource">
      <metaclasses>
        <metaclassMoniker name="/DBProfile/Microsoft.VisualStudio.Uml.Classes.IClass"/>
      </metaclasses>
    </stereotype>
    <stereotype name="BlankNode" displayName="BlankNode">
      <metaclasses>
        <metaclassMoniker name="/DBProfile/Microsoft.VisualStudio.Uml.Classes.IClass"/>
      </metaclasses>
    </stereotype>
    <stereotype name="HierarchyTable" displayName="Hierarchy Table">
      <metaclasses>
        <metaclassMoniker name="/DBProfile/Microsoft.VisualStudio.Uml.Classes.IClass"/>
      </metaclasses>
    </stereotype>
    <stereotype name="TableHierarchy" displayName="Table Hierarchy">
      <metaclasses>
        <metaclassMoniker name="/DBProfile/Microsoft.VisualStudio.Uml.Classes.IClass"/>
      </metaclasses>
    </stereotype>
    <stereotype name="AssociationTable" displayName="Association Table">
      <metaclasses>
        <metaclassMoniker name="/DBProfile/Microsoft.VisualStudio.Uml.Classes.IClass"/>
      </metaclasses>
    </stereotype>
    <stereotype name="Column" displayName="Column">
      <metaclasses>
        <metaclassMoniker name="/DBProfile/Microsoft.VisualStudio.Uml.Classes.IProperty">
        </metaclassMoniker>
      </metaclasses>
      <properties>
        <property name="PrimaryKey" displayName="PrimaryKey" defaultValue="false">
          <propertyType>
            <externalTypeMoniker name="/DBProfile/System.Boolean" />
          </propertyType>
        </property>
        <property name="DataType" displayName="Data Type" defaultValue="varchar()">
          <propertyType>
            <enumerationTypeMoniker name="/DBProfile/DataType"/>
          </propertyType>
        </property>
        <property name="Length" displayName="Length" defaultValue="24">
          <propertyType>
            <externalTypeMoniker name="/DBProfile/System.String"/>
          </propertyType>
        </property>
        <property name="AllowNulls" displayName="Allow Nulls" defaultValue="true">
          <propertyType>
            <externalTypeMoniker name="/DBProfile/System.Boolean"/>
          </propertyType>
        </property>
      </properties>
    </stereotype>
    <stereotype name="ForeignKey" displayName="Foreign Key">
      <metaclasses>
        <metaclassMoniker name="/DBProfile/Microsoft.VisualStudio.Uml.Classes.IAssociation" />
      </metaclasses>
      <properties>
        <property name="DeleteRule" displayName="Delete Rule" defaultValue="No Action">
          <propertyType>
            <enumerationTypeMoniker name="/DBProfile/ForeignKeyRule"/>
          </propertyType>
        </property>
        <property name="UpdateRule" displayName="Update Rule" defaultValue="No Action">
          <propertyType>
            <enumerationTypeMoniker name="/DBProfile/ForeignKeyRule"/>
          </propertyType>
        </property>
      </properties>
    </stereotype>
    <stereotype name="AbstractReference" displayName="Abstract Reference">
      <metaclasses>
        <metaclassMoniker
          name="/DBProfile/Microsoft.VisualStudio.Uml.Classes.IAssociation">
        </metaclassMoniker>
      </metaclasses>
    </stereotype>
    <stereotype name="Reference" displayName="Reference">
      <metaclasses>
        <metaclassMoniker
          name="/DBProfile/Microsoft.VisualStudio.Uml.Classes.IAssociation">
        </metaclassMoniker>
      </metaclasses>
    </stereotype>
    <stereotype name="Predicate" displayName="Predicate">
      <metaclasses>
        <metaclassMoniker
          name="/DBProfile/Microsoft.VisualStudio.Uml.Classes.IAssociation">
        </metaclassMoniker>
      </metaclasses>
    </stereotype>
    <stereotype name="Parent" displayName="Parent">
      <metaclasses>
        <metaclassMoniker
          name="/DBProfile/Microsoft.VisualStudio.Uml.Classes.IAssociation">
        </metaclassMoniker>
      </metaclasses>
    </stereotype>
    <stereotype name="Child" displayName="Child">
      <metaclasses>
        <metaclassMoniker
          name="/DBProfile/Microsoft.VisualStudio.Uml.Classes.IAssociation">
        </metaclassMoniker>
      </metaclasses>
    </stereotype>
  </stereotypes>
  <metaclasses>
    <metaclass name="Microsoft.VisualStudio.Uml.Classes.IClass" />
    <metaclass name="Microsoft.VisualStudio.Uml.Classes.IPackage" />
    <metaclass name="Microsoft.VisualStudio.Uml.Classes.IProperty" />
    <metaclass name="Microsoft.VisualStudio.Uml.Classes.IAssociation" />
  </metaclasses>
  <propertyTypes>
    <externalType name="System.Boolean" />
    <externalType name="System.String" />
    <enumerationType name="DataType">
      <!-- SQL 2008 Data Types: http://msdn.microsoft.com/en-us/library/ms187752.aspx -->
      <enumerationLiterals>
        <enumerationLiteral name="bigint" displayName="bigint"/>
        <enumerationLiteral name="binary" displayName="binary"/>
        <enumerationLiteral name="blob" displayName="blob"/>
        <enumerationLiteral name="boolean" displayName="boolean"/>
        <enumerationLiteral name="char()" displayName="char()"/>
        <enumerationLiteral name="date" displayName="date"/>
        <enumerationLiteral name="datetime" displayName="datetime"/>
        <enumerationLiteral name="datetimeoffset" displayName="datetimeoffset"/>
        <enumerationLiteral name="datetimezone" displayName="datetimezone"/>
        <enumerationLiteral name="decimal()" displayName="decimal()"/>
        <enumerationLiteral name="double" displayName="double"/>
        <enumerationLiteral name="guid" displayName="guid"/>
        <enumerationLiteral name="float()" displayName="float()"/>
        <enumerationLiteral name="image" displayName="image"/>
        <enumerationLiteral name="int" displayName="int"/>
        <enumerationLiteral name="money" displayName="money"/>
        <enumerationLiteral name="ntext" displayName="ntext"/>
        <enumerationLiteral name="numeric" displayName="numeric"/>
        <enumerationLiteral name="text" displayName="text"/>
        <enumerationLiteral name="time" displayName="time"/>
        <enumerationLiteral name="timestamp" displayName="timestamp"/>
        <enumerationLiteral name="tinyint" displayName="tinyint"/>
        <enumerationLiteral name="varbinary()" displayName="varbinary()"/>
        <enumerationLiteral name="varchar()" displayName="varchar()"/>
        <enumerationLiteral name="xml" displayName="xml"/>
      </enumerationLiterals>
    </enumerationType>
    <enumerationType name="ForeignKeyRule">
      <enumerationLiterals>
        <enumerationLiteral name="No Action" displayName="No Action"/>
        <enumerationLiteral name="Cascade" displayName="Cascade"/>
        <enumerationLiteral name="SetNull" displayName="Set Null"/>
        <enumerationLiteral name="SetDefault" displayName="Set Default"/>
      </enumerationLiterals>
    </enumerationType>
  </propertyTypes>
</profile>
