using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace CommLib.ShareFun
{
    /// <summary>
    /// 异步TCP服务
    /// <para>通过异步socket建立基本服务端，所有接入的连接都可以在ClientList找到</para>
    /// <para>客户端连接时，会通过ClientArrived回调上报信息，用法如下</para>
    /// <para>1. 通过构造函数，输入ip，端口，各种回调信息。</para>
    /// <para>2. 有客户端接入时，会通过回调通知上层，并将此连接插入ClientList</para>
    /// <para>3. 上层通过ClientList与客户端通信。</para>
    /// <para>4. 发送、删除都走client那边就好了。</para>
    /// </summary>
    public class AsyncSocketServer
    {
        /// <summary>
        /// 同步信号量
        /// </summary>
        private ManualResetEvent allDone = new ManualResetEvent(false);

        /// <summary>
        /// 写日志委托，如果赋值了，那么本对象中的所有日志均通过此输出
        /// </summary>
        private MyDelegateString makeLog;

        /// <summary>
        /// 客户端连接回调-如果一个新客户端连接上来，将通过此回调通知上层应用
        /// </summary>
        private ReceiveTcpClientHandler ClientArrived;

        /// <summary>
        /// 客户端删除回调--某个客户端去掉了，通过此回调通知上层应用
        /// <para>一般是上层主动删掉的话，就不用监控这个了。。</para>
        /// </summary>
        private ReceiveTcpClientHandler ClientDeleted;

        /// <summary>
        /// 客户端列表
        /// </summary>
        public List<ClientObject> ClientList = new List<ClientObject>();

        /// <summary>
        /// IP地址
        /// </summary>
        public string ServerIp;

        /// <summary>
        /// 端口
        /// </summary>
        public int ServerPort;

        /// <summary>
        /// 使用IP端口来初始化，传入的IP或者端口有问题的话，会抛异常--初始化正常的话，TCP服务器就启动了。
        /// </summary>
        /// <param name="ip">服务器IP</param>
        /// <param name="port">服务器端口</param>
        /// <param name="clientEvnet">客户端已经连上回调，所有客户端连接时会触发此回调</param>
        /// <param name="log">日志回调，对象中所有日志将通过此回调输出，不写的话就不输出日志了（客户端也用这个回调回显日志）</param>
        public AsyncSocketServer(string ip, int port,
            ReceiveTcpClientHandler clientAdd = null,
            ReceiveTcpClientHandler clientDel = null,
            MyDelegateString log = null)
        {
            ClientArrived += clientAdd;
            ClientDeleted += clientDel;
            makeLog += log;

            StartListening(ip, port);
        }

        /// <summary>
        /// 默认构造函数，调用这个完毕后需要手动设置IP端口、各种回调，最后调用StartListening启动服务。
        /// </summary>
        public AsyncSocketServer()
        {

        }

        /// <summary>
        /// 设置客户端接入回调
        /// </summary>
        /// <param name="arrive"></param>
        public void SetClientArrivedCallBack(ReceiveTcpClientHandler arrive)
        {
            ClientArrived += arrive;
        }

        /// <summary>
        /// 设置日志回调--客户端日志也将用这个回调
        /// </summary>
        /// <param name="log"></param>
        public void SetLogCallBack(MyDelegateString log)
        {
            makeLog += log;
        }
        /// <summary>
        /// 设置客户端掉线回调
        /// </summary>
        /// <param name="del"></param>
        public void SetClientDeletedCallBack(ReceiveTcpClientHandler del)
        {
            ClientDeleted += del;
        }

        /// <summary>
        /// 使用IP，端口启动监听
        /// </summary>
        /// <param name="ip">服务器IP</param>
        /// <param name="port">服务器端口</param>
        public void StartListening(string ip, int port)
        {
            ServerIp = ip;
            ServerPort = port;

            IPEndPoint ep = new IPEndPoint(IPAddress.Parse(ip), port);
            StartListening(ep);
        }

        /// <summary>
        /// 使用IPEndPoint启动服务
        /// </summary>
        /// <param name="ep">服务端IPEndPoint对象</param>
        public void StartListening(IPEndPoint ep)
        {
            #region 抄来代码中的自动获取IP地址的代码，这个先去掉了，后期如果需要自动获取IP，就找这个。。
            //// Establish the local endpoint for the socket.  
            //// The DNS name of the computer  
            //// running the listener is "host.contoso.com".  
            //IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            //IPAddress ipAddress = ipHostInfo.AddressList[1];
            //IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);
            #endregion

            makeLog($"start listening...IP:[{ep.Address.ToString()}],port[{ep.Port}]");

            try
            {
                // Create a TCP/IP socket.  
                Socket listener = new Socket(ep.AddressFamily,
                    SocketType.Stream, ProtocolType.Tcp);

                // Bind the socket to the local endpoint and listen for incoming connections.  
                listener.Bind(ep);
                listener.Listen(100);

                while (true)
                {
                    // Set the event to nonsignaled state.  
                    allDone.Reset();

                    // Start an asynchronous socket to listen for connections.  
                    makeLog("Waiting for a connection...");
                    listener.BeginAccept(
                        new AsyncCallback(AcceptCallback),
                        listener);

                    // Wait until a connection is made before continuing.  
                    allDone.WaitOne();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            makeLog("stop listening...");

        }

        /// <summary>
        /// 客户端接入的回调。。
        /// </summary>
        /// <param name="ar"></param>
        private void AcceptCallback(IAsyncResult ar)
        {
            // Signal the main thread to continue.  
            allDone.Set();

            // Get the socket that handles the client request.  
            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            // Create the state object.  
            ClientObject state = new ClientObject(this, makeLog);
            state.workSocket = handler;

            //把这个链接对象塞入到队列中。。
            ClientList.Add(state);
            //通知上层，客户端已经收到
            ClientArrived(state);
            makeLog("Accept client:" + state.remoteInfo);
        }

        /// <summary>
        /// 从客户端列表中删掉指定客户端
        /// </summary>
        /// <param name="s"></param>
        public void DeleteClientObj(ClientObject s)
        {
            try
            {
                if (ClientList.Contains(s))
                {
                    ClientList.Remove(s);
                    //然后通知一下上层应用，，避免他处理抛异常，导致服务器删不掉客户端了。。
                    ClientDeleted(s);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }

        }

    }

    /// <summary>
    /// TCP客户端信息
    /// <para>本客户端由异步服务创建并维护，在创建后，需要上层程序对“接收”回调进行赋值，否则只能通过查询的方式判断数据是否已经到来</para>
    /// <para>1. 中断方式：</para>
    /// <para>1.1 上位程序收到服务的“客户端接入”事件后，调用SetReceiveCallBack()为此对象的ReceiveMessage赋值，此后只要收到消息，均会通过此回调向上汇报</para>
    /// <para>1.2 收到消息后，上位程序检测ReceivedData，判断是否有数据，是否长度满足需要，取走就好</para>
    /// <para>2. 查询方式</para>
    /// <para>2.1 上位程序没有调用SetReceiveCallBack()为此对象的ReceiveMessage赋值，不知道是否已经有数据过来</para>
    /// <para>2.2 上位程序使用定时器周期查询ReceivedData是否有数</para>
    /// </summary>
    public class ClientObject
    {
        private static int ObjCount = 0;
        private static object lockObj = new object();

        /// <summary>
        /// 写日志委托，如果赋值了，那么本对象中的所有日志均通过此输出
        /// </summary>
        private MyDelegateString makeLog;


        /// <summary>
        /// 唯一ID号，每个程序启动的时候从0开始分派--只读
        /// </summary>
        public int ObjId
        {
            get { return objId; }
        }

        /// <summary>
        /// 唯一ID号，每个程序启动的时候从0开始分派-
        /// </summary>
        private int objId;

        /// <summary>
        /// 自己所属的服务程序（记录一下）
        /// </summary>
        public AsyncSocketServer Parent;

        /// <summary>
        /// 获取客户端基本信息
        /// </summary>
        public string remoteInfo
        {
            get
            {
                try
                {
                    if (workSocket != null)
                    {
                        string info = workSocket.RemoteEndPoint.ToString();
                        return info;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    //throw;
                }

                return "unknown...";
            }
        }

        /// <summary>
        /// 客户端通信socket
        /// </summary>
        public Socket workSocket = null;

        /// <summary>
        /// 缓冲区大小
        /// </summary>
        public const int BufferSize = 10240;

        /// <summary>
        /// 临时缓冲区
        /// </summary>
        private byte[] buffer = new byte[BufferSize];

        /// <summary>
        /// 数据队列
        /// </summary>
        public RingBufferManager ReceivedData = new RingBufferManager(BufferSize);

        /// <summary>
        /// 收到信息回调，只要收到一次报文，就会触发此回调，本对象创建后，需要上层应用手动对此赋值，
        /// 若不赋值则需要通过查询的方式判断是否有数据到来。
        /// </summary>
        private MyDelegateVoid ReceiveMessage;


        /// <summary>
        /// 创建对象，并赋予一个全局ID，初始化完毕就立即开始监听数据
        /// </summary>
        /// <param name="server">所隶属的服务对象</param>
        /// <param name="log">日志代理</param>
        public ClientObject(AsyncSocketServer server, MyDelegateString log)
        {
            Parent = server;
            makeLog = log;
            lock (lockObj)
            {
                ObjCount++;
                objId = ObjCount;
            }

            //异步读取消息
            workSocket.BeginReceive(buffer, 0, ClientObject.BufferSize, 0,
                new AsyncCallback(ReadCallback), workSocket);

            makeLog(remoteInfo + " 已接入，已启动监听...")
        }

        /// <summary>
        /// 设置接受消息的回调
        /// </summary>
        /// <param name="cb"></param>
        public void SetReceiveCallBack(MyDelegateVoid cb)
        {
            ReceiveMessage = cb;
        }

        /// <summary>
        /// 异步收取信息
        /// </summary>
        /// <param name="ar"></param>
        private void ReadCallback(IAsyncResult ar)
        {
            try
            {

                // Read data from the client socket.   
                int bytesRead = workSocket.EndReceive(ar);

                if (bytesRead > 0)
                {
                    // 收到了bytesRead个字节，放到了临时缓冲区buffer里，现在只需要把buffer的内容放到循环队列里ReceivedData就好了。。
                    ReceivedData.WriteBuffer(buffer, 0, bytesRead);

                    //通知上层，说已经收到了数据。。。然后让他自己查询吧。。
                    ReceiveMessage();

                    // 然后继续接收不用停
                    workSocket.BeginReceive(buffer, 0, ClientObject.BufferSize, 0,
                         new AsyncCallback(ReadCallback), workSocket);

                }
            }
            catch (Exception e)
            {
                makeLog(e);
            }
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="buf"></param>
        /// <param name="startIndex"></param>
        /// <param name="count"></param>
        public void Send(byte[] buf, int startIndex, int count)
        {
            try
            {
                // Begin sending the data to the remote device.  
                workSocket.BeginSend(buf, startIndex, count, 0,
                    new AsyncCallback(SendCallback), workSocket);
            }
            catch (Exception e)
            {
                makeLog(e.ToString());
            }

        }

        /// <summary>
        /// 发送回调--主要记录一下发送结果，，不需要的话把这里的注释去掉就好了。。
        /// </summary>
        /// <param name="ar"></param>
        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                Socket handler = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.  
                int bytesSent = handler.EndSend(ar);
                makeLog($"Sent {bytesSent} bytes to client.{remoteInfo}");

            }
            catch (Exception e)
            {
                makeLog(e.ToString());
            }
        }

        /// <summary>
        /// 关闭连接--同时在服务端注销自己
        /// </summary>
        /// <param name="s"></param>
        public void Shutdown()
        {
            try
            {
                workSocket.Shutdown(SocketShutdown.Both);
                workSocket.Close();

                //然后通知服务器从队列里删掉
                Parent.DeleteClientObj(this);
            }
            catch (Exception e)
            {
                makeLog(e.Message);
            }
        }
    }

    /// <summary>
    /// TCP服务器接收到连接的处理回调对象
    /// </summary>
    public delegate void ReceiveTcpClientHandler(ClientObject co);

}
