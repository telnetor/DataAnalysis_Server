using DataAnalysis.Application.IService;
using DataAnalysis.Core.Data.IRepositories.IUnitRepositories;
using DataAnalysis.Manipulation.WebSocketExtension;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAnalysis.Application.Service
{
    public class AmplitudeService : IAmplitudeService
    {
        public void CollectionAreaData()
        {
            HtmlWeb webClient = new HtmlWeb();
            HtmlDocument doc = webClient.Load("https://www.huobi.br.com/zh-cn/coin_coin/exchange/#s=eos_usdt");
            HtmlNodeCollection hrefList = doc.DocumentNode.SelectNodes(".//div[@class='coin_table']//div[@class='coin_list']");
        }

        public void StartWebSocket()
        {

         
        }
    }
}
