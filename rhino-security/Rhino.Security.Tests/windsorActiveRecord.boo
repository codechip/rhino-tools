import System.Reflection
import Rhino.Security.Framework from Rhino.Security
import Rhino.Security.Configuration from Rhino.Security
import Rhino.Security.Configuration from Rhino.Security.ActiveRecord
import Rhino.Security.Tests


facility Rhino.Security.Configuration.ARSecurityFacility:
  userType = typeof(User)
  tableStructure = SecurityTableStructure.Prefix

component INHibernateInitializationAware, EnableTestCaching

component IEntityInformationExtractor of Account, AccountInfromationExtractor

component IRepository, ARRepository
component IUnitOfWorkFactory, ActiveRecordUnitOfWorkFactory