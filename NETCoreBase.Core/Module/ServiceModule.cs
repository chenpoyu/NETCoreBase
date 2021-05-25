using System.Linq;
using Autofac;

namespace NETCoreBase.Core
{
    public class ServiceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            builder
                .RegisterAssemblyTypes(assembly)
                .Where(c => c.IsPublic && c.IsClass && !c.IsAbstract && c.Name.EndsWith("Service"))
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();
        }
    }

}