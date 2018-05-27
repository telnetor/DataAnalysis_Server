using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAnalysis.Component.Tools.Log
{
    public class LogManage
    {

        static ILogger _Error = LogManager.GetLogger("Error");

        public static ILogger Error
        {
            get
            {
                return _Error;
            }
        }

        static ILogger _Sql = LogManager.GetLogger("Sql");

        public static ILogger Sql
        {
            get
            {
                return _Sql;
            }
        }

        static ILogger _WebSocket = LogManager.GetLogger("WebSocket");

        public static ILogger WebSocket
        {
            get
            {
                return _WebSocket;
            }
        }

        static ILogger _Job = LogManager.GetLogger("Job");

        public static ILogger Job
        {
            get
            {
                return _Job;
            }
        }
    }
}
