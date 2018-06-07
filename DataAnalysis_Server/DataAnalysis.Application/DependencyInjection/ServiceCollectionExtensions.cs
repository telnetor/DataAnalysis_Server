
using Autofac;
using Autofac.Extensions.DependencyInjection;
using DataAnalysis.Application.IService;
using DataAnalysis.Application.IService.ICallService;
using DataAnalysis.Application.IService.IJobService;
using DataAnalysis.Application.IService.IUserService;
using DataAnalysis.Application.Service;
using DataAnalysis.Application.Service.BitService;
using DataAnalysis.Application.Service.CallService;
using DataAnalysis.Application.Service.JobService;
using DataAnalysis.Application.Service.UserService;
using DataAnalysis.Manipulation.DapperExtension;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace DataAnalysisFrame
{
    public static class ServiceCollectionExtensions
    {
        public static AutofacServiceProvider AddInjection(this IServiceCollection services)
        {
            var builder = new ContainerBuilder();
            SetupResolveRules(builder);

            #region service 层注入
            builder.RegisterType<AmplitudeService>().As<IAmplitudeService>();
            builder.RegisterType<ExecuteSocketService>().As<IExecuteSocketService>();
            builder.RegisterType<ExecuteRedisService>().As<IExecuteRedisService>();
            builder.RegisterType<ExecuteCallService>().As<IExecuteCallService>();
            builder.RegisterType<ObtainService>().As<IObtainService>();
            builder.RegisterType<UserAssetsService>().As<IUserAssetsService>();
            builder.RegisterType<BitBaseService>();
            #endregion

            builder.RegisterType<DbProviderConfig>().As<IDbProviderConfig>().SingleInstance();
            builder.RegisterType<UnitOfWork>().As<IUnitOfWork>().InstancePerLifetimeScope();
            builder.Populate(services);
            Autofac.IContainer ApplicationContainer = builder.Build();
            ServerLocation.SetServerLocation(ApplicationContainer);

            return new AutofacServiceProvider(ApplicationContainer);
        }
        private static void SetupResolveRules(ContainerBuilder builder)
        {
            var Inter = new List<Type>();
            Assembly assembly = Assembly.Load("DataAnalysis.Core.Data");
            foreach (Type aaa in assembly.GetTypes())
            {
                if (aaa.Name.StartsWith("I") && aaa.Name.EndsWith("Repository"))
                {
                    Inter.Add(aaa);
                }
            }

            var entity = new List<Type>();
            Assembly entassembly = Assembly.Load("DataAnalysis.Core.Data.Repository");
            foreach (Type aaa in entassembly.GetTypes())
            {
                if (aaa.Name.EndsWith("Repository"))
                {
                    entity.Add(aaa);
                }
            }

            foreach (var item in entity)
            {
                Inter.ForEach(a =>
                {
                    if (a.Name.Contains(item.Name))
                    {
                        builder.RegisterType(item).As(a);
                    }
                });
            }

        }
    }
}
