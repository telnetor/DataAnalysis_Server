using System;
using System.Collections.Generic;

using System.Text;
using WebSocket4Net;

namespace DataAnalysis.Manipulation.WebSocketExtension
{
    public static class WebSocketFactories
    {
        public static WebSocket webSocket = new WebSocket(SocketProviderConfig.HUOBI_WEBSOCKET_API);
        public static WebSocket GetFactory()
        {
            return webSocket;
        }
    }
}
