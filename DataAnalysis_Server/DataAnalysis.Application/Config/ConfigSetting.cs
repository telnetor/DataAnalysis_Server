
using DataAnalysis.Component.Tools.Config;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAnalysisFrame
{
    public static class ConfigSetting
    {



        public static void AddConfig(this IServiceCollection services, IConfiguration configuration)
        {        
            AppSettings.SetSetting(configuration);
        }
    }
}
