namespace Rhino.Security
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;
    using Castle.ActiveRecord;
    using Castle.ActiveRecord.Framework;
    using Castle.ActiveRecord.Framework.Internal;
    using Castle.Core.Logging;
    using Commons;

    /// <summary>
    /// This class allows to configure the security system
    /// </summary>
    public class Security
    {

        /// <summary>
        /// Prepares to change all internal reference in the security system
        /// from IUser to the user implementation of the project
        /// </summary>
        public static void PrepareForActiveRecordInitialization<TUser>()
            where TUser : IUser
        {
                PrepareForActiveRecordInitialization<TUser>(SecurityTableStructure.Schema);
        }

        /// <summary>
        /// Prepares to change all internal reference in the security system
        /// from IUser to the user implementation of the project
        /// </summary>
        /// <typeparam name="TUser">The type of the user.</typeparam>
        /// <param name="tableStructure">The table structure.</param>
        public static void PrepareForActiveRecordInitialization<TUser>(SecurityTableStructure tableStructure)
            where TUser : IUser
        {
            ActiveRecordStarter.ModelsValidated += delegate(ActiveRecordModelCollection models, IConfigurationSource source)
                {
                    foreach (ActiveRecordModel model in models)
                    {
                        if (model.Type.Assembly != typeof(IUser).Assembly)
                            continue;
                        model.Accept(new AddCachingVisitor());
                        model.Accept(new ReplaceUserVisitor(typeof(TUser)));
                        model.Accept(new ChangeSchemaVisitor(tableStructure));
                    }
                };
        }

        /// <summary>
        /// Prepares to change all internal reference in the security system
        /// from IUser to the user implementation of the project when the user
        /// is not an entity in the model, but an external cocnept
        /// </summary>
        /// <typeparam name="TExternalUser">The type of the external user.</typeparam>
        /// <param name="tableStructure">The table structure.</param>
        public static void PrepareForActiveRecordInitializationWithExternalUser<TExternalUser>(SecurityTableStructure tableStructure)
            where TExternalUser:IExternalUser, new()
        {
            ActiveRecordStarter.ModelsValidated += delegate(ActiveRecordModelCollection models, IConfigurationSource source)
                  {
                      foreach (ActiveRecordModel model in models)
                      {
                          if (model.Type.Assembly != typeof(IUser).Assembly)
                              continue;
                          model.Accept(new AddCachingVisitor());
                          model.Accept(new ReplaceUserWithExternalVisitor<TExternalUser>());
                          model.Accept(new ChangeSchemaVisitor(tableStructure));
                      }
                  };
        }

        /// <summary>
        /// Gets the logger for the security system.
        /// </summary>
        /// <value>The logger.</value>
        public static ILogger Logger
        {
            get
            {
                return IoC.TryResolve<ILogger>(new NullLogger());

            }
        }

        /// <summary>
        /// Extracts the key from the specified entity.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        public static Guid ExtractKey<TEntity>(TEntity entity)
            where TEntity : class
        {
            Guard.Against<ArgumentNullException>(entity == null, "Entity cannot be null");
            IEntityInformationExtractor<TEntity> extractor = IoC.Resolve<IEntityInformationExtractor<TEntity>>();
            return extractor.GetSecurityKeyFor(entity);
        }

		/// <summary>
		/// Gets a human readable description for the specified entity
		/// </summary>
		/// <typeparam name="TEntity">The type of the entity.</typeparam>
		/// <param name="entity">The entity.</param>
		/// <returns></returns>
		 public static string GetDescription<TEntity>(TEntity entity) where TEntity : class
		{
            IEntityInformationExtractor<TEntity> extractor = IoC.Resolve<IEntityInformationExtractor<TEntity>>();
            return extractor.GetDescription(ExtractKey(entity));
		}

        /// <summary>
        /// Gets the security key property for the specified entity type
        /// </summary>
        /// <param name="entityType">Type of the entity.</param>
        /// <returns></returns>
        public static string GetSecurityKeyProperty(Type entityType)
        {
            lock(GetSecurityKeyPropertyCache)
            {
                Func<string> func;
                if (GetSecurityKeyPropertyCache.TryGetValue(entityType, out func))
                    return func();
                func = (Func<string>)
                    Delegate.CreateDelegate(typeof (Func<string>), 
                        getSecurityKeyMethod.MakeGenericMethod(entityType));
                GetSecurityKeyPropertyCache[entityType] = func;
                return func();
            }
        }

        internal static string GetSecurityKeyPropertyInternal<TEntity>()
        {
            return IoC.Resolve<IEntityInformationExtractor<TEntity>>().SecurityKeyPropertyName;
        }

        private readonly static Dictionary<Type, Func<string>> GetSecurityKeyPropertyCache = new Dictionary<Type, Func<string>>();
        private readonly static MethodInfo getSecurityKeyMethod = typeof(Security).GetMethod("GetSecurityKeyPropertyInternal", BindingFlags.NonPublic | BindingFlags.Static);
    }
}
