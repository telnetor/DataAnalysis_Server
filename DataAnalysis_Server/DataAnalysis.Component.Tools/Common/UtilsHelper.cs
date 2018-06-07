using System;
using System.Collections.Generic;
using System.Text;

namespace DataAnalysis.Component.Tools.Common
{
    public class UtilsHelper
    {
        /// <summary>    
        /// Unix时间戳转为C#格式时间    
        /// </summary>    
        /// <param name="timeStamp">Unix时间戳格式,例如1482115779</param>    
        /// <returns>C#格式时间</returns>    
        public static DateTime GetTime(long unixTimeStamp)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); // 当地时区
            DateTime dt = startTime.AddSeconds(unixTimeStamp);
            return dt;
        }


        /// <summary>    
        /// DateTime时间格式转换为Unix时间戳格式    
        /// </summary>    
        /// <param name="time"> DateTime时间格式</param>    
        /// <returns>Unix时间戳格式</returns>    
        public static int ConvertDateTimeInt(System.DateTime time)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            return (int)(time - startTime).TotalSeconds;
        }
    }
}
