using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using CommLib.ShareFun;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommLib.Test
{
    [TestClass]
    public class UnitTestAsyncSocketServer
    {

        public void mLog(string val)
        {
            Debug.WriteLine(val);
        }

        //普通测试--客户端连上，发消息。。
        [TestMethod]
        public void GeneralTest()
        {
            //启动一个服务器，在启动一堆线程不停的连接断开。。
            AsyncSocketServer server = new AsyncSocketServer("127.0.0.1", 12345, AcceptClient, null, mLog);

            Thread th11 = null;
            for (int i = 0; i < 100; i++)
            {
                Thread th = new Thread(ConnectAndDisconnect);
                th.IsBackground = true;
                th.Start(i);
                Thread.Sleep(100);

                th11 = th;
            }

            th11?.Join();

            Thread.Sleep(5000);
        }

        public void ConnectAndDisconnect(object o)
        {
            int count = (int)o;
            try
            {
                IPAddress ip = IPAddress.Parse("127.0.0.1");
                Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                try
                {
                    clientSocket.Connect(new IPEndPoint(ip, 12345)); //配置服务器IP与端口  
                }
                catch
                {
                    Assert.Fail("连接服务器失败");
                }
                //通过 clientSocket 发送数据  
                //for (int i = 0; i < 10; i++)
                //{
                try
                {
                    Thread.Sleep(1000);    //等待1秒钟  
                    string sendMessage = count + "-client send Message Hellp" + DateTime.Now;
                    clientSocket.Send(Encoding.ASCII.GetBytes(sendMessage));
                    Debug.WriteLine($"{count},向服务器发送消息：{sendMessage}");

                    clientSocket.Shutdown(SocketShutdown.Both);
                    clientSocket.Close();
                }
                catch (Exception e)
                {
                    //clientSocket.Shutdown(SocketShutdown.Both);
                    //clientSocket.Close();
                    //break;

                    Assert.Fail("断开了：" + count + "\r\n" + e.Message);
                }
                //}
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        public void AcceptClient(ClientObject co)
        {
            Debug.WriteLine("收到连接：" + co.remoteInfo);
            //Thread th = new Thread(ReadClient);
            //th.IsBackground = true;
            //th.Start(co);

            co.SetReceiveCallBack(ReadCb);
        }

        private static int ClientCount = 0;
        public void ReadClient(object o)
        {
            ClientObject co = (ClientObject)o;
            int count = ClientCount++;

            string read = "";
            int cycCnt = 0;
            while (cycCnt++ < 100)
            {
                Thread.Sleep(50);
                if (co.ReceivedData.DataCount > 0)
                {
                    Thread.Sleep(100);
                    byte[] readBuf = new byte[co.ReceivedData.DataCount];
                    co.ReceivedData.PopBuffer(readBuf, 0, readBuf.Length);
                    read = Encoding.ASCII.GetString(readBuf);
                }
            }

            if (read.Length > 0)
            {
                Debug.WriteLine(count + "收到数据了" + read);
            }
            else
            {
                Assert.Fail("没收到数据" + count);
            }
        }

        public void ReadCb(ClientObject co)
        {
            int count = ClientCount++;
            string read="";
            Thread.Sleep(100);
            if (co.ReceivedData.DataCount > 0)
            {
                byte[] readBuf = new byte[co.ReceivedData.DataCount];
                co.ReceivedData.PopBuffer(readBuf, 0, readBuf.Length);
                read = Encoding.ASCII.GetString(readBuf);
            }

            if (read.Length > 0)
            {
                Debug.WriteLine(count + "收到数据了" + read);
            }
            else
            {
                Assert.Fail("没收到数据" + count);
            }
        }



        [TestMethod]
        public void ReadAndSendTest()
        {
            
        }
    }
}
