using Fleck;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace MARS_Web.Helper
{
    public class WebSocketHelper
    {
        private static WebSocketHelper webSocketInstance = null;
        private static object lockObj = new object();

        List<IWebSocketConnection> allSockets;
        Dictionary<string, Dictionary<long, WebSocketInfo>> socketDic;
        WebSocketServer server;
        
        private WebSocketHelper()
        {
            string webSocketUrl = System.Web.Configuration.WebConfigurationManager.AppSettings["WebSocketUrl"];
            allSockets = new List<IWebSocketConnection>();
            server = new WebSocketServer(webSocketUrl);
            socketDic =new Dictionary<string, Dictionary<long, WebSocketInfo>>();// new Dictionary<long, WebSocketInfo>();
        }
        public static WebSocketHelper WebSocketInstance
        {
            get 
            {
                if (webSocketInstance == null) 
                {
                    lock (lockObj)
                    {
                        if (webSocketInstance == null)
                        {
                            webSocketInstance = new WebSocketHelper();
                        }
                    }
                }

                return webSocketInstance;
            }
        }

        public void StartServer() 
        {
            server.Start(socket =>
            {
                socket.OnOpen = () =>
                {
                    allSockets.Add(socket);
                };
                socket.OnClose = () =>
                {
                    allSockets.Remove(socket);
                    foreach (var info in socketDic)
                    {
                        if (info.Value.Values.ToList().Exists(r => r.SocketInfo.Equals(socket)))//remove close and  unsaved
                        {
                            foreach (var socketInfo in socketDic[info.Key])
                            {
                                if (socketInfo.Value.SocketInfo.Equals(socket))
                                {
                                    socketDic[info.Key].Remove(socketInfo.Key);
                                    break;
                                }
                            }
                        }
                    }
                };
                socket.OnMessage = message =>
                {
                    //socket.ConnectionInfo.Headers["Sec-WebSocket-Key"]
                    WebSocketMessage info = Newtonsoft.Json.JsonConvert.DeserializeObject<WebSocketMessage>(message);
                    if (info != null && info.SocketType == "0")
                    {
                        if (!socketDic.ContainsKey(info.OpendViewName))
                            socketDic.Add(info.OpendViewName, new Dictionary<long, WebSocketInfo>());

                        if (!socketDic[info.OpendViewName].ContainsKey(info.TestCaseid))
                        {
                            socketDic[info.OpendViewName].Add(info.TestCaseid, new WebSocketInfo(socket,info));
                            allSockets.ToList().ForEach(s =>
                            {
                                if (!socket.Equals(s))//no need to send to yourself
                                    s.Send(message);
                            });
                        }
                    }
                    else if(info != null && info.SocketType == "1")
                    {
                        if (socketDic.ContainsKey(info.OpendViewName))
                        {
                            if (socketDic[info.OpendViewName].ContainsKey(info.TestCaseid))
                            {
                                var exsitsInfo = socketDic[info.OpendViewName][info.TestCaseid];
                                if (!exsitsInfo.SocketInfo.Equals(socket))
                                    socket.Send(Newtonsoft.Json.JsonConvert.SerializeObject(exsitsInfo.Message));
                            }
                        }
                    }
                    else if (info != null && info.SocketType == "2")
                    {
                        if (socketDic.ContainsKey(info.OpendViewName))
                        {
                            if (socketDic[info.OpendViewName].ContainsKey(info.TestCaseid))
                            {
                                allSockets.ToList().ForEach(s =>
                                {
                                    if (!socket.Equals(s))
                                        s.Send(message);
                                });
                                if (socket.Equals(socketDic[info.OpendViewName][info.TestCaseid].SocketInfo))
                                {
                                    socketDic[info.OpendViewName].Remove(info.TestCaseid);
                                }
                            }
                        }
                    }
                };
            });
        }
    }

    public class WebSocketMessage
    {
        public string UserName { get; set; }
        public string TestCaseName { get; set; }
        public long TestCaseid { get; set; }
        /// <summary>
        /// type:0 some one editing the test case,server send message to other users
        /// type:1 some one open the test case,ask server 
        /// type:2 some one save the test case,send message to other users
        /// </summary>
        public string SocketType { get; set; }

        public string OpendViewName { get; set; }
    }

    public class WebSocketInfo
    {
        public IWebSocketConnection SocketInfo { get; set; }
        public WebSocketMessage Message { get;set;}

        public WebSocketInfo(IWebSocketConnection socket, WebSocketMessage message)
        {
            this.Message = message;
            this.SocketInfo = socket;
        }
    }
}