using DataAnalysis.Component.Tools.Common;
using DataAnalysis.Component.Tools.Constant;
using DataAnalysis.Component.Tools.Constant.ResponseEntity;
using DataAnalysis.Component.Tools.Log;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using WebSocket4Net;

namespace DataAnalysis.Application.WebSocketExtension
{
    public class WebSocketBehavior
    {
        private static WebSocket websocket;
        private static bool isOpened;
        private static bool isFirst = true;

        public static event EventHandler<HuoBiMessageReceivedEventArgs> OnMessage;

        static WebSocketBehavior()
        {
            Init();
        }

        public static void Init()
        {
            try
            {
                websocket = WebSocketFactories.GetFactory();
                websocket.DataReceived+= ReceviedMsg;
                websocket.Opened += OnOpened;
                websocket.Error += (sender, e) =>
                {
                    System.Diagnostics.Trace.WriteLine("老子断线了");
                    LogManage.WebSocket.Error(e.Exception.Message);
                };
                //断线重连
                websocket.Closed += (sender, e) =>
                {
                    if (websocket.State != WebSocketState.Connecting && 
                    websocket.State != WebSocketState.Open)
                    {
                        System.Diagnostics.Trace.WriteLine("老子又重联了");
                        Init();
                    }
                };
                websocket.Open();
                OnMessage += (sender, e) => { };
            }
            catch (Exception ex)
            {
                LogManage.WebSocket.Error(ex.Message);
            }
        }
        #region Opened&心跳响应&触发消息事件
        private static void OnOpened(object sender, EventArgs e)
        {
            isOpened = true;
            foreach (var item in HuoBiContract.topicDic)
            {
                SendSubscribeTopic(item.Value);
            }
        }

        private static void ReceviedMsg(object sender, DataReceivedEventArgs e)
        {
            var msg = GZipHelper.GZipDecompressString(e.Data);
            if (msg.IndexOf("ping") != -1) //响应心跳包
            {
                var reponseData = msg.Replace("ping", "pong");
                websocket.Send(reponseData);
            }
            else//接收消息
            {
                if (msg.IndexOf("status")<0)
                {
                    if (msg.IndexOf("depth") >= 0)
                    {
                        //System.Diagnostics.Trace.WriteLine(msg);
                        OnMessage?.Invoke(null, new HuoBiMessageReceivedEventArgs(msg));
                    }
                    else if (msg.IndexOf("kline") >= 0)
                    {

                    }
                    else if (msg.IndexOf("trade") >= 0)
                    {
                        OnMessage?.Invoke(null, new HuoBiMessageReceivedEventArgs(msg));
                    }

                }
            }
        }
        #endregion

        #region 订阅相关
        public static void SendSubscribeTopic(string msg)
        {
            websocket.Send(msg);
        }

        public static void Subscribe(string topic, string id)
        {
            if (HuoBiContract.topicDic.ContainsKey(topic))
                return;
            var msg = new { sub = topic, id = id };
            string json = JsonConvert.SerializeObject(msg);
            HuoBiContract.topicDic.Add(topic, json);
            if (isOpened)
            {
                SendSubscribeTopic(json);
            }
        }

        public static void UnSubscribe(string topic, string id)
        {
            if (!HuoBiContract.topicDic.ContainsKey(topic) || !isOpened)
                return;
            var msg = new { unsub = topic, id = id };
            HuoBiContract.topicDic.Remove(topic);
            SendSubscribeTopic(JsonConvert.SerializeObject(msg));
        }

        #endregion
    }
}
