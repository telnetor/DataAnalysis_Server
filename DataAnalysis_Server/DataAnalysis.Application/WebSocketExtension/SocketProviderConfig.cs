using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text;
using WebSocket4Net;

namespace DataAnalysis.Application.WebSocketExtension
{
    public class SocketProviderConfig 
    {
        protected static string connection { get; set; }
        protected static IConfigurationRoot Configuration { get; set; }

        public static string HUOBI_WEBSOCKET_API
        {
            get {
                var builder = new ConfigurationBuilder()
                         .SetBasePath(Directory.GetCurrentDirectory())
                         .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                Configuration = builder.Build();
                connection = Configuration.GetSection("HuoBi")["WebSocket_API"];
                return connection;
            }
        }
    }
}
