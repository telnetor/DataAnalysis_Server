using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAnalysis.Component.Tools.Config
{



    public class AppSettings
    {

        private static IConfiguration appConfig = null;

        private static IConfigurationSection webConfig = null;

        private static IConfigurationSection webChat = null;


        /// <summary>
        ///  站点配置
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string WebSetting(string key)
        {
            string str = "";
            if (webConfig.GetSection(key) != null)
            {
                str = webConfig.GetSection(key).Value;
            }
            return str;
        }



        public static void SetSetting(IConfiguration config)
        {
            appConfig = config;
        }


    }
}
