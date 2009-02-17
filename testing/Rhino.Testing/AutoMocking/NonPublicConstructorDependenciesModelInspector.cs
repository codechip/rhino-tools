using System.Reflection;
using Castle.MicroKernel;
using Castle.MicroKernel.ModelBuilder.Inspectors;

namespace Rhino.Testing.AutoMocking
{
    internal class NonPublicConstructorDependenciesModelInspector : ConstructorDependenciesModelInspector
    {
        public override void ProcessModel(IKernel kernel, Castle.Core.ComponentModel model)
        {
            base.ProcessModel(kernel, model);

            var constructors = model.Implementation.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic);
            foreach (var constructorInfo in constructors)
                model.Constructors.Add(CreateConstructorCandidate(model, constructorInfo));
        }
    }
}
