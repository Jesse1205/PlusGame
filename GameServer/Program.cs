using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using PlusGame.Common.Event;
using PlusGame.Common.Log;
using PlusGame.Framework.Config;
using PlusGame.Framework.Contract;
using PlusGame.Framework.Session;
using PlusGame.Message;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace GameServer
{
    class Program
    {
        static void Main(string[] args)
        {
            ServerHost host = new ServerHost();
            try
            {
                Console.WriteLine("服务器{0},{1}启动成功", GameEnvironment.SocketConfig.IPAddress, GameEnvironment.SocketConfig.Port);
                host.RunServerAsync().Wait();
                Console.Read();
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("服务器{0},{1}启动失败,{2},{3}", GameEnvironment.SocketConfig.IPAddress, GameEnvironment.SocketConfig.Port, ex.Message, ex.StackTrace);
            }
            finally
            {
                host.CloseServerAsync().Wait();
            }
        }
    }

    public class ServerHost : GameSocketHost
    {
        protected override void OnConnected(GameSession session)
        {

        }

        protected override void OnDisconnected(GameSession session)
        {

        }

        protected override void OnDataReceived(IChannelHandlerContext context, IByteBuffer message)
        {
            RequestPackage package;
            if (GameEnvironment.ActionDispatcher.TryDecodePackage(message, out package))
            {
                context.WriteAsync(message);
            }

            //Program.ThreadIds.Add(Thread.CurrentThread.ManagedThreadId);
            //context.WriteAndFlushAsync(message);
            //Task.Run(() =>
            //{
            //var session = SessionManager.Get(context);
            //session.PostSend(message);
            //});


            //Task.Run(() =>
            //{
            //    Stopwatch watch = new Stopwatch();
            //    watch.Start();
            //    try
            //    {
            //        string result;
            //        RequestPackage package;
            //        if (TryBuildPackage(context, message, out result, out package))
            //        {
            //            var loginPid = ActorManagement.GetLocalServerPid<LoginActor>();
            //            loginPid.Tell(package);
            //            //ResponsePackage repPackage = new ResponsePackage();
            //            //repPackage.InitPackage(package);
            //            //IByteBuffer data = GameEnvironment.ActionDispatcher.TryEncodePackage(repPackage);
            //            //var session = SessionManager.Get(repPackage.SessionId);
            //            //session.PostSend(data);
            //        }
            //        else
            //        {
            //            throw new Exception(result);
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        ResponsePackage response = new ResponsePackage();
            //        response.ErrorCode = 10000;
            //        response.ErrorInfo = ex.Message;
            //        var data = GameEnvironment.ActionDispatcher.TryEncodePackage(response);
            //        context.WriteAndFlushAsync(data);
            //        TraceLog.WriteError("接受数据异常:{0},{1},{2}", ex.Message, ex.StackTrace, message.ToString());
            //    }
            //    watch.Stop();
            //    if (watch.ElapsedMilliseconds > 5)
            //        TraceLog.WriteError("解包超时:{0},{1}", message.ToString(), watch.ElapsedMilliseconds);
            //});
        }

        protected override void OnHeartTimeout(GameSession session)
        {

        }
    }
}
