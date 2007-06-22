import Rhino.Commons

# generic type registration
Component(defualt_repository, IRepository, NHRepository)
Component(nhibernate_factory, IUnitOfWorkFactory, NHibernateUnitOfWorkFactory)