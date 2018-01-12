using System;
using System.Collections.Generic;
using System.Text;

namespace CommLib.ShareFun
{
    /// <summary>
    /// 环形缓冲队列
    /// </summary>
    public class RingBufferManager
    {
        /// <summary>
        /// 内部缓冲区
        /// </summary>
        public byte[] Buffer { get; set; } 

        /// <summary>
        /// 现存数据量 -- 请勿修改
        /// </summary>
        public int DataCount { get; set; } 

        /// <summary>
        /// 起始索引 -- 请勿修改
        /// </summary>
        public int DataStart { get; set; } 

        /// <summary>
        /// 结束索引 -- 请勿修改
        /// </summary>
        public int DataEnd { get; set; }   

        /// <summary>
        /// 初始化，参数为缓冲区大小
        /// </summary>
        /// <param name="bufferSize">内部缓冲区大小</param>
        public RingBufferManager(int bufferSize)
        {
            DataCount = 0; DataStart = 0; DataEnd = 0;
            Buffer = new byte[bufferSize];
        }

        /// <summary>
        /// 获取当前缓冲区内的第n个数据（有效数据）
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public byte this[int index]
        {
            get
            {
                if (index >= DataCount) throw new Exception("环形缓冲区异常，索引溢出");
                if (DataStart + index < Buffer.Length)
                {
                    return Buffer[DataStart + index];
                }
                else
                {
                    return Buffer[(DataStart + index) - Buffer.Length];
                }
            }
        }

        /// <summary>
        /// 获得当前写入的字节数
        /// </summary>
        /// <returns></returns>
        public int GetDataCount()
        {
            return DataCount;
        }

        /// <summary>
        /// 获得剩余的字节数
        /// </summary>
        /// <returns></returns>
        public int GetReserveCount() 
        {
            return Buffer.Length - DataCount;
        }

        /// <summary>
        /// 清空整个队列
        /// </summary>
        public void Clear()
        {
            DataCount = 0;
            DataStart = 0;
            DataEnd = 0;
        }


        /// <summary>
        /// 清空指定大小的数据
        /// </summary>
        /// <param name="count">指定数量，如果超过现有数量，则全部清除</param>
        public void Clear(int count) // 
        {
            if (count >= DataCount) // 如果需要清理的数据大于现有数据大小，则全部清理
            {
                DataCount = 0;
                DataStart = 0;
                DataEnd = 0;
            }
            else
            {
                if (DataStart + count >= Buffer.Length)
                {
                    DataStart = (DataStart + count) - Buffer.Length;
                }
                else
                {
                    DataStart += count;
                }
                DataCount -= count;
            }
        }

        /// <summary>
        /// 写入缓冲区
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        public void WriteBuffer(byte[] buffer, int offset, int count)
        {
            Int32 reserveCount = Buffer.Length - DataCount;
            if (reserveCount >= count)                          // 可用空间够使用
            {
                if (DataEnd + count < Buffer.Length)            // 数据没到结尾
                {
                    Array.Copy(buffer, offset, Buffer, DataEnd, count);
                    DataEnd += count;
                    DataCount += count;
                }
                else           //  数据结束索引超出结尾 循环到开始
                {
                    System.Diagnostics.Debug.WriteLine("缓存重新开始....");
                    Int32 overflowIndexLength = (DataEnd + count) - Buffer.Length;      // 超出索引长度
                    Int32 endPushIndexLength = count - overflowIndexLength;             // 填充在末尾的数据长度
                    Array.Copy(buffer, offset, Buffer, DataEnd, endPushIndexLength);
                    DataEnd = 0;
                    offset += endPushIndexLength;
                    DataCount += endPushIndexLength;
                    if (overflowIndexLength != 0)
                    {
                        Array.Copy(buffer, offset, Buffer, DataEnd, overflowIndexLength);
                    }
                    DataEnd += overflowIndexLength;                                     // 结束索引
                    DataCount += overflowIndexLength;                                   // 缓存大小
                }
            }
            else
            {
                // 缓存溢出，不处理
            }
        }

        /// <summary>
        /// 读取数据到指定buf（不清空已读出数据）
        /// </summary>
        /// <param name="targetBytes">要存入的byte数组</param>
        /// <param name="offset">该byte数组的大小</param>
        /// <param name="count">数量</param>
        public void ReadBuffer(byte[] targetBytes, Int32 offset, Int32 count)
        {
            if (count > DataCount) throw new Exception("环形缓冲区异常，读取长度大于数据长度");
            Int32 tempDataStart = DataStart;
            if (DataStart + count < Buffer.Length)
            {
                Array.Copy(Buffer, DataStart, targetBytes, offset, count);
            }
            else
            {
                Int32 overflowIndexLength = (DataStart + count) - Buffer.Length;    // 超出索引长度
                Int32 endPushIndexLength = count - overflowIndexLength;             // 填充在末尾的数据长度
                Array.Copy(Buffer, DataStart, targetBytes, offset, endPushIndexLength);
                offset += endPushIndexLength;
                if (overflowIndexLength != 0)
                {
                    Array.Copy(Buffer, 0, targetBytes, offset, overflowIndexLength);
                }
            }
        }

        /// <summary>
        /// 弹出数据到指定buf（删除已读出数据）
        /// </summary>
        /// <param name="targetBytes">要存入的byte数组</param>
        /// <param name="offset">该byte数组的大小</param>
        /// <param name="count">数量</param>
        public void PopBuffer(byte[] targetBytes, Int32 offset, Int32 count)
        {
            ReadBuffer(targetBytes,offset,count);
            Clear(count);
        }

        /// <summary>
        /// 写入缓冲区
        /// </summary>
        /// <param name="buffer"></param>
        public void WriteBuffer(byte[] buffer)
        {
            WriteBuffer(buffer, 0, buffer.Length);
        }

    }
}
