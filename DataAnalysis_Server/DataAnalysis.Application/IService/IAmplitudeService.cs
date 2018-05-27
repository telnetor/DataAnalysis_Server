using System;
using System.Collections.Generic;
using System.Text;

namespace DataAnalysis.Application.IService
{
    public interface IAmplitudeService
    {
        /// <summary>
        /// 采集主区数据
        /// </summary>
        void CollectionAreaData();

        void StartWebSocket();
    }
}
