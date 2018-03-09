using Google.Protobuf;
using PlusGame.Message;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Protocol
{
    class Program
    {
        static void Main(string[] args)
        {
            Socket socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9001));

            //string data = "";
            //for (int i = 0; i < 100; i++)
            //{
            //    data += "a";
            //}
            //byte[] sendHeader = BitConverter.GetBytes(data.Length);
            //byte[] sendData = Encoding.UTF8.GetBytes(data);
            //byte[] sendTotal = new byte[sendHeader.Length + sendData.Length];
            //Buffer.BlockCopy(sendHeader, 0, sendTotal, 0, sendHeader.Length);
            //Buffer.BlockCopy(sendData, 0, sendTotal, sendHeader.Length, sendData.Length);

            //proto3
            RequestPackage request = new RequestPackage();
            request.MsgId = 0;
            request.ActionId = 1002;
            request.SessionId = "";
            request.UserId = 0;
            request.St = "st";
            request.DictData["ClientAppVersion"] = "1.0.0";
            request.DictData["MobileType"] = "1";
            request.DictData["DeviceId"] = "a";
            request.DictData["HeadIcon"] = "a";
            request.DictData["UnionId"] = "a";
            request.DictData["OpenId"] = "a";
            request.DictData["NickName"] = "a";
            request.DictData["Sex"] = "a";
            request.DictData["Location"] = "a";
            request.DictData["sign"] = "d3418b5383ab375ee7c8bcf0cf0477a8";
            var message = request as IMessage;

            var sendData = message.ToByteArray();

            byte[] sendHeader = BitConverter.GetBytes(sendData.Length);
            byte[] sendTotal = new byte[sendHeader.Length + sendData.Length];
            Buffer.BlockCopy(sendHeader, 0, sendTotal, 0, sendHeader.Length);
            Buffer.BlockCopy(sendData, 0, sendTotal, sendHeader.Length, sendData.Length);

            int totalCount = 1000000;

            int sendCount = 0;
            Task.Run(() =>
            {
                while (sendCount < totalCount)
                {
                    socket.Send(sendTotal);
                    Interlocked.Increment(ref sendCount);
                }
            });

            AutoResetEvent reset = new AutoResetEvent(false);
            int receiveCount = 1;
            long receiveLen = 0;
            Stopwatch watch = new Stopwatch();
            watch.Start();
            Task.Run(() =>
            {
                byte[] buffer = new byte[2048];
                while (true)
                {
                    receiveLen += socket.Receive(buffer);
                    //while (socket.Receive(buffer) > 0)
                    //{
                    //    Interlocked.Increment(ref receiveCount);
                    //    if (receiveCount >= sendCount)
                    //    {
                    //        reset.Set();
                    //    }
                    //}
                    if (receiveLen >= totalCount * 100)
                    {
                        reset.Set();
                    }
                }
            });

            Task.Run(() =>
            {
                while (true)
                {
                    Thread.Sleep(1000);
                    Console.WriteLine("{0},{1},{2}", sendCount, receiveCount, receiveLen);
                }
            });

            reset.WaitOne();
            watch.Stop();
            Console.WriteLine("时间:{0},{1},{2},{3},ping pong{4}(s)", watch.ElapsedMilliseconds, sendCount, receiveCount, receiveLen, totalCount * 1000 / watch.ElapsedMilliseconds);

            Console.WriteLine("Hello World!");
            Console.Read();
        }
    }
}
