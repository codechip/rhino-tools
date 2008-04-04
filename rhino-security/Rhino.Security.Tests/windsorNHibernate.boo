import System.Reflection
import Rhino.Commons from Rhino.Commons.NHibernate as nh
import Rhino.Security.Framework from Rhino.Security
import Rhino.Security.Configuration from Rhino.Security
import Rhino.Security.Configuration from Rhino.Security.NHibernate
import Rhino.Security.Tests


facility Rhino.Security.Configuration.NHSecurityFacility:
  userType = typeof(User)
  tableStructure = SecurityTableStructure.Prefix


component IEntityInformationExtractor of Account, AccountInfromationExtractor

component IRepository, NHRepository
component IUnitOfWorkFactory, NHibernateUnitOfWorkFactory