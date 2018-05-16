
using Autofac;
using Autofac.Extensions.DependencyInjection;
using DataAnalysis.Core.Data.IRepositories.IUnitRepositories;
using DataAnalysis.Core.Data.Repository.Repositories.UnitRepository;
using DataAnalysis.Manipulation.DapperExtension;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAnalysisFrame
{
    public static class ServiceCollectionExtensions
    {
        public static AutofacServiceProvider AddInjection(this IServiceCollection services)
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<TestRepository>().As<ITestRepository>();
            builder.RegisterType<DbProviderConfig>().As<IDbProviderConfig>().SingleInstance();
            builder.RegisterType<UnitOfWork>().As<IUnitOfWork>().InstancePerLifetimeScope();
            builder.Populate(services);
            Autofac.IContainer ApplicationContainer = builder.Build();
            return new AutofacServiceProvider(ApplicationContainer);
        }
    }
}
