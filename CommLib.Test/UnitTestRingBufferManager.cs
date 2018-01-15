using CommLib.ShareFun;
using System;
using System.Diagnostics;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace CommLib.Test
{
    [TestClass]
    public class UnitTestRingBufferManager
    {
        private RingBufferManager rbm;

        /// <summary>
        /// 开个线程不停的写，然后这边不停的读，判断数据是否正常。。
        /// 1. 测出来不加锁的话，就跑飞了。。 
        /// </summary>
        [TestMethod]
        public void DataValadTest()
        {

            rbm = new RingBufferManager(999);
            Thread th1 = new Thread(InsertQueueThread);
            th1.IsBackground = true;
            th1.Start();


            byte[] rb = new byte[100];

            int cycleCount = 0;
            while (th1.IsAlive)
            {
                if (rbm.GetDataCount() < 100)
                {
                    Thread.Sleep(10);
                    continue;
                }

                rbm.PopBuffer(rb, 0, 100);

                for (int i = 0; i < 100; i++)
                {
                    Assert.AreEqual(i, rb[i]);
                }

                cycleCount++;

                if (cycleCount % 100 == 0)
                {
                    Debug.WriteLine("read complete count:" + cycleCount);
                }
            }

            while (rbm.GetDataCount() > 100)
            {
                rbm.PopBuffer(rb, 0, 100);

                for (int i = 0; i < 100; i++)
                {
                    Assert.AreEqual(i, rb[i]);
                }

                cycleCount++;


                Debug.WriteLine("read complete count:" + cycleCount);

            }
        }

        private void InsertQueueThread()
        {
            Debug.WriteLine("insert begin...");
            byte[] buf = new byte[100];
            for (int i = 0; i < 100; i++)
            {
                buf[i] = (byte)i;
            }

            for (int i = 0; i < 10000; i++)
            {
                //容量不足了，稍微等会
                while (rbm.GetReserveCount() < 300)
                {
                    Thread.Sleep(10);
                }

                rbm.WriteBuffer(buf);

            }

            Debug.WriteLine("insert end...");
        }

        /// <summary>
        /// 这个主要是上面测试结果，加了个锁，然后这个看下，，锁里面抛异常了会不会还继续被锁。。
        /// 结论：应该没问题，锁里面抛异常，出来了就解锁了。。
        /// </summary>
        [TestMethod]
        public void LockTest()
        {
            byte[] buf = new byte[100];
            for (int i = 0; i < 100; i++)
            {
                buf[i] = (byte)i;
            }

            rbm = new RingBufferManager(999);


            try
            {
                byte[] rb1 = new byte[100];
                rbm.PopBuffer(rb1, 0, 100);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }


            try
            {
                //尝试插入11次，就锁住了。。看看一会能不能读取
                for (int i = 0; i < 11; i++)
                {
                    rbm.WriteBuffer(buf);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }


            byte[] rb = new byte[100];
            rbm.PopBuffer(rb, 0, 100);

            for (int i = 0; i < 100; i++)
            {
                Assert.AreEqual(i, rb[i]);
            }
        }

        /// <summary>
        /// 测一下性能把。。
        /// 2018年1月12日 结果：1000w次300字节读写，11秒
        /// </summary>
        [TestMethod]
        public void PerformanceTest()
        {
            byte[] buf = new byte[100];
            for (int i = 0; i < 100; i++)
            {
                buf[i] = (byte)i;
            }

            rbm = new RingBufferManager(999);

            //插个底数，别让他每次清空
            rbm.WriteBuffer(buf);

            DateTime dt1 = DateTime.Now;

            for (int i = 0; i < 10000000; i++)
            {
                rbm.WriteBuffer(buf);
                rbm.WriteBuffer(buf);
                rbm.WriteBuffer(buf);
                byte[] rb = new byte[100];
                rbm.PopBuffer(rb, 0, 100);
                rbm.PopBuffer(rb, 0, 100);
                rbm.PopBuffer(rb, 0, 100);
            }

            DateTime dt2 = DateTime.Now;

            Debug.WriteLine("1000w次读写300字节，耗时（s）" + (dt2 - dt1).TotalSeconds);
        }
    }
}
