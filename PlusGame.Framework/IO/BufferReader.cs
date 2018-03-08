﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace PlusGame.Framework.IO
{
    /// <summary>
    /// 缓冲区读取器
    /// </summary>
    public class BufferReader
    {
        private readonly byte[] _buffer;
        private int _offset;
        private int _length;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        public BufferReader(Stream stream)
        {
            using (var reader = new MemoryStream())
            {
                byte[] buffer = new byte[1024];
                int count;
                while (true)
                {
                    try
                    {
                        count = stream.Read(buffer, 0, buffer.Length);
                    }
                    catch (Exception)
                    {
                        break;
                    }
                    if (count == 0)
                    {
                        break;
                    }
                    reader.Write(buffer, 0, count);
                }
                _buffer = reader.ToArray();
            }
            _offset = 0;
            _length = _buffer.Length;
        }

        /// <summary>
        /// 
        /// </summary>
        public byte[] Data { get { return _buffer; } }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        public BufferReader(byte[] buffer)
        {
            _offset = 0;
            _buffer = buffer;
            _length = buffer.Length;
        }

        /// <summary>
        /// 缓冲区大小
        /// </summary>
        public int Length
        {
            get { return _length; }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Reset()
        {
            _offset = 0;
        }

        /// <summary>
        /// 是否还有数据
        /// </summary>
        /// <returns></returns>
        public bool HasByte(out byte[] values)
        {
            values = new byte[0];
            if (_offset < Length)
            {
                values = new byte[Length - _offset];
                Buffer.BlockCopy(_buffer, _offset, values, 0, values.Length);
                return true;
            }
            return false;
        }
        /// <summary>
        /// 读取内存流中的头4位并转换成整型
        /// </summary>
        /// <returns></returns>
        public long ReadInt64()
        {
            var value = BitConverter.ToInt64(_buffer, _offset);
            _offset = Interlocked.Add(ref _offset, 8);
            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int ReadInt32()
        {
            var value = BitConverter.ToInt32(_buffer, _offset);
            _offset = Interlocked.Add(ref _offset, 4);
            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int ReadInt16()
        {
            var value = BitConverter.ToInt16(_buffer, _offset);
            _offset = Interlocked.Add(ref _offset, 2);
            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool ReadBool()
        {
            var value = BitConverter.ToBoolean(_buffer, _offset);
            _offset = Interlocked.Increment(ref _offset);
            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public byte ReadByte()
        {
            var value = _buffer[_offset];
            _offset = Interlocked.Increment(ref _offset);
            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string ReadString()
        {
            int len = _length - _offset;
            string value = BitConverter.ToString(_buffer, _offset, len);
            _offset = Interlocked.Add(ref _offset, len);
            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public byte[] ReadToEnd()
        {
            int len = _length - _offset;
            var value = new byte[len];
            Buffer.BlockCopy(_buffer, _offset, value, 0, len);
            _offset = Interlocked.Add(ref _offset, len);
            return value;
        }

        /// <summary>
        /// 读取分包字节流，头部前4位是分包的长度
        /// </summary>
        /// <returns></returns>
        public byte[] ReadPacket()
        {
            int length = ReadInt32();
            var values = new Byte[length];
            Buffer.BlockCopy(_buffer, _offset, values, 0, values.Length);
            _offset = Interlocked.Add(ref _offset, length);
            return values;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string ReadPacketString()
        {
            return ReadPacketString(Encoding.UTF8);
        }

        /// <summary>
        /// 读取内存流中一段字符串
        /// </summary>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public string ReadPacketString(Encoding encoding)
        {
            byte[] values = ReadPacket();
            return encoding.GetString(values, 0, values.Length);
        }

    }
}