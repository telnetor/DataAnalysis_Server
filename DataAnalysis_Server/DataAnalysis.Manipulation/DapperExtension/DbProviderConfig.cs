using DataAnalysis.Component.Tools.Settings;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAnalysis.Manipulation.DapperExtension
{
    public class DbProviderConfig : IDbProviderConfig
    {
        protected static IConfigurationRoot Configuration { get; set; }
        protected static string connection { get; set; }


        public string ProviderFactoryString {
            get {
                return AppSetting.GetConnection("MySql", "ProviderName");
            }

         }

        public string DbConnectionString {
            get
            {
                return AppSetting.GetConnection("MySql", "DataAnalysisDB");
            }
        }

    }
}
