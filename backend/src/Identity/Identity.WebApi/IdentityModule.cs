using Autofac;
using Identity.Infrastructure.Databases;

namespace Identity.WebApi;

public class IdentityModule:Module
{
    protected override void Load(ContainerBuilder builder)
    {
        // var contextOptions = new DbContextOptionsBuilder<AppDbContext>().Options;
        // builder.RegisterInstance(contextOptions).As<DbContextOptions>();
        builder.RegisterType<ApplicationDbContext>().InstancePerLifetimeScope();
        base.Load(builder);
    }
}