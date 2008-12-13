using Castle.Core;
using Castle.MicroKernel;
using Castle.Windsor;

namespace Advance.IoC.ComponentSelectors
{
    public class NotificationResolver : ISubDependencyResolver
    {
        private readonly IWindsorContainer container;
        private int level = 0;

        public NotificationResolver(IWindsorContainer container)
        {
            this.container = container;
        }

        public object Resolve(CreationContext context, ISubDependencyResolver parentResolver, ComponentModel model,
                              DependencyModel dependency)
        {
            try
            {
                string notification = Context.CurrentUser.Preferences.Notification;
                string split = notification.Split('.')[level];
                level +=1;
                return container.Resolve<INotifications>(split);
            }
            finally
            {
                level -= 1;
            }
        }

        public bool CanResolve(CreationContext context, ISubDependencyResolver parentResolver, ComponentModel model,
                               DependencyModel dependency)
        {

            return Context.CurrentUser != null &&
                   dependency.TargetType == typeof (INotifications);
        }
    }
}