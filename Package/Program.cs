using DotNetty.Buffers;
using Google.Protobuf;
using Google.Protobuf.Reflection;
using PlusGame.Common.Serialization;
using PlusGame.Framework.Contract;
using PlusGame.Message;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Web;

namespace Package
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
            1000000解包
            scut协议
            总耗时:34483,平均耗时:0.034483(ms)
            protobuf协议
            总耗时:5104,平均耗时:0.005104(ms)
            protobuf3协议
            总耗时:3374,平均耗时:0.003374(ms)
            */
            //suct
            //string data = "MsgId%3d0%26ActionId%3d1002%26Sid%3d%26Uid%3d0%26St%3dst%26GameType%3d1%26ClientAppVersion%3d1.0.0%26MobileType%3d1%26DeviceId%3da%26HeadIcon%3d1%26UnionId%3dabc%26OpenId%3dabc%26NickName%3d%25e8%25b6%2585%25e7%25ba%25a7%25e5%2590%2588%25e4%25bc%2599%25e4%25ba%25ba%26Sex%3dFalse%26Location%3d%26sign%3dd3418b5383ab375ee7c8bcf0cf0477a8";
            //string data = "MsgId=0&ActionId=1002&Sid=&Uid=0&St=st&GameType=1&ClientAppVersion=1.0.0&MobileType=1&DeviceId=a&HeadIcon=1&UnionId=abc&OpenId=abc&NickName=%e8%b6%85%e7%ba%a7%e5%90%88%e4%bc%99%e4%ba%ba&Sex=False&Location=&sign=d3418b5383ab375ee7c8bcf0cf0477a8";
            //byte[] sendHeader = BitConverter.GetBytes(data.Length);
            //byte[] sendData = Encoding.UTF8.GetBytes(data);
            //byte[] sendTotal = new byte[sendHeader.Length + sendData.Length];
            //Buffer.BlockCopy(sendHeader, 0, sendTotal, 0, sendHeader.Length);
            //Buffer.BlockCopy(sendData, 0, sendTotal, sendHeader.Length, sendData.Length);

            //var buffer = Unpooled.WrappedBuffer(sendData);

            //var dispatcher = new PlusDispatcher();

            //Stopwatch watch = new Stopwatch();
            //watch.Start();
            //RequestPackage package;
            //int totalCount = 1000000;
            //for (int i = 0; i < totalCount; i++)
            //{
            //    dispatcher.TryDecodePackage(buffer, out package);
            //}
            //watch.Stop();
            //Console.WriteLine("总耗时:{0},平均耗时:{1}(ms)", watch.ElapsedMilliseconds, watch.ElapsedMilliseconds * 1.0 / totalCount);

            //proto
            //Request request = new Request();
            //request.MsgId = 0;
            //request.ActionId = 1002;
            //request.SessionId = "";
            //request.UserId = 0;
            //request.St = "st";
            //request.DictData = new Dictionary<string, string>();
            //request.DictData["ClientAppVersion"] = "1.0.0";
            //request.DictData["MobileType"] = "1";
            //request.DictData["DeviceId"] = "a";
            //request.DictData["HeadIcon"] = "a";
            //request.DictData["UnionId"] = "a";
            //request.DictData["OpenId"] = "a";
            //request.DictData["NickName"] = "a";
            //request.DictData["Sex"] = "a";
            //request.DictData["Location"] = "a";
            //request.DictData["sign"] = "d3418b5383ab375ee7c8bcf0cf0477a8";
            //var sendData = ProtoBufUtils.Serialize(request, false);
            //var buffer = Unpooled.WrappedBuffer(sendData);


            //var dispatcher = new PlusDispatcher();

            //Stopwatch watch = new Stopwatch();
            //watch.Start();
            //Request package;
            //int totalCount = 1000000;
            //for (int i = 0; i < totalCount; i++)
            //{
            //    //dispatcher.TryDecodePackage(buffer, out package);
            //    package = ProtoBufUtils.Deserialize<Request>(sendData);
            //}
            //watch.Stop();
            //Console.WriteLine("总耗时:{0},平均耗时:{1}(ms)", watch.ElapsedMilliseconds, watch.ElapsedMilliseconds * 1.0 / totalCount);

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

            var buffer = Unpooled.WrappedBuffer(sendData);

            Stopwatch watch = new Stopwatch();
            watch.Start();
            RequestPackage package;
            int totalCount = 1000000;
            for (int i = 0; i < totalCount; i++)
            {
                //dispatcher.TryDecodePackage(buffer, out package);
                package = RequestPackage.Parser.ParseFrom(sendData);
            }
            watch.Stop();
            Console.WriteLine("总耗时:{0},平均耗时:{1}(ms)", watch.ElapsedMilliseconds, watch.ElapsedMilliseconds * 1.0 / totalCount);


            Console.Read();
        }
    }

    [ProtoContract]
    public class Request
    {
        [ProtoMember(1)]
        public int MsgId { get; set; }
        [ProtoMember(2)]
        public int ActionId { get; set; }
        [ProtoMember(3)]
        public string SessionId { get; set; }
        [ProtoMember(4)]
        public int UserId { get; set; }
        [ProtoMember(5)]
        public string St { get; set; }
        [ProtoMember(6)]
        public Dictionary<string, string> DictData { get; set; }
    }

}
