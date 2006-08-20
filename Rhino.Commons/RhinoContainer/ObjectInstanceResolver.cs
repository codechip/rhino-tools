using System;
using System.Collections.Generic;
using System.Text;
using Castle.MicroKernel;
using Castle.Model;

namespace Rhino.Commons
{
    public class ObjectInstanceResolver : ISubDependencyResolver
    {
        Dictionary<DependencyModel, object> dependencies = new Dictionary<DependencyModel, object>();
        ///<summary>
        ///
        ///            Should return an instance of a service or property values as
        ///            specified by the dependency model instance. 
        ///            It is also the responsability of <see cref="T:Castle.MicroKernel.IDependencyResolver" />
        ///            to throw an exception in the case a non-optional dependency 
        ///            could not be resolved.
        ///            
        ///</summary>
        ///
        ///<param name="model"></param>
        ///<param name="dependency"></param>
        ///<returns>
        ///
        ///</returns>
        ///
        public object Resolve(CreationContext context, ComponentModel model, DependencyModel dependency)
        {
            return dependencies[dependency];
        }

        ///<summary>
        ///
        ///            Returns true if the resolver is able to satisfy this dependency.
        ///            
        ///</summary>
        ///
        ///<param name="model"></param>
        ///<param name="dependency"></param>
        ///<returns>
        ///
        ///</returns>
        ///
        public bool CanResolve(CreationContext context, ComponentModel model, DependencyModel dependency)
        {
            return dependencies.ContainsKey(dependency);
        }
        
        public void AddDependencyInstance(DependencyModel dependencyModel, object instance)
        {
            dependencies.Add(dependencyModel, instance);
        }
    }
}
