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
                if (!IsCacheable(model.Type))
                {
                    model.ActiveRecordAtt.Cache = CacheEnum.ReadWrite;
                }

                foreach (HasManyModel hasManyModel in model.HasMany)
                {
                    if (!IsCacheable(hasManyModel.HasManyAtt.MapType)) continue;
                    hasManyModel.HasManyAtt.Cache = CacheEnum.ReadWrite;
                }
            }
        }

        private static bool IsCacheable(Type type)
        {
            return type.GetCustomAttributes(typeof(CacheableAttribute), true).Length != 0;
        }
    }
}
