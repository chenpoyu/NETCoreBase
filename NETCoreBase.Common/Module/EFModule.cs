using Autofac;
using NETCoreBase.Common.Interfaces;
using NETCoreBase.Common.Services;
using NETCoreBase.Database;
using Microsoft.EntityFrameworkCore;
using NETCoreBase.Database.Models;

namespace NETCoreBase.Common
{
    public class EFModule : Module
    {
        private readonly string _connStr;
        public EFModule(string connectionString)
        {
            _connStr = connectionString;
        }
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(c =>
            {
                var optionsBuilder = new DbContextOptionsBuilder<NETCoreBaseContext>();
                optionsBuilder.UseSqlServer(_connStr);
                return optionsBuilder.Options;
            }).InstancePerLifetimeScope();

            builder.RegisterType<NETCoreBaseContext>()
                .As<NETCoreBaseContext>()
                .InstancePerLifetimeScope();

            builder.RegisterGeneric(typeof(GenericRepository<>))
                .As(typeof(IGenericRepository<>))
                .InstancePerLifetimeScope();
        }
    }
}