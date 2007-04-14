using System;
using Castle.ActiveRecord;
using Castle.ActiveRecord.Framework;
using Castle.ActiveRecord.Framework.Internal;
using Rhino.Commons;

namespace Rhino.Igloo
{
    /// <summary>
    /// Add caching support to all entities and collections
    /// </summary>
    public class ActiveRecordCachingPreprocessor
    {
        /// <summary>
        /// Handles the models creation event and pre-process the models
        /// </summary>
        public static void ModelsCreated(ActiveRecordModelCollection models, IConfigurationSource source)
        {
            foreach (ActiveRecordModel model in models)
            {
                if (IsCacheable(model.Type) == false)
                {
                    model.ActiveRecordAtt.Cache = CacheEnum.ReadWrite;
                }
                foreach (HasManyModel hasManyModel in model.HasMany)
                {
                    if (IsCacheable(hasManyModel.HasManyAtt.MapType))
                    {
                        hasManyModel.HasManyAtt.Cache = CacheEnum.ReadWrite;
                    }
                }
            }
        }

        private static bool IsCacheable(Type type)
        {
            return type.GetCustomAttributes(typeof(CacheableAttribute), true).Length != 0;
        }
    }
}
