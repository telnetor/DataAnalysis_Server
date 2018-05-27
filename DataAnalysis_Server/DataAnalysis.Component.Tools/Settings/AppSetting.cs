using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DataAnalysis.Component.Tools.Settings
{
    public class AppSetting
    {
        protected static IConfigurationRoot Configuration { get; set; }
        protected static string connection { get; set; }

        public static string GetConnection(string key, string key1)
        {
            var builder = new ConfigurationBuilder()
                         .SetBasePath(Directory.GetCurrentDirectory())
                         .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            Configuration = builder.Build();
            connection = Configuration.GetSection(key)[key1];
            return connection;
        }
    }
}
