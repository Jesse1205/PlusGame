using System;
using System.Linq;
using Proto.Remote;
using PlusGame.Message;
using Proto;
using System.Threading.Tasks;
using System.Collections.Generic;
using PlusGame.Common.Event;
using System.Threading;

namespace Node2
{
    class Program
    {
        static Dictionary<int, int> dict = new Dictionary<int, int>();

        static void Main(string[] args)
        {
            Serialization.RegisterFileDescriptor(PlusGame.Message.ProtosReflection.Descriptor);
            Remote.Start("127.0.0.1", 6002);
            var localPid = Actor.SpawnNamed(
                   Actor.FromProducer(() => new NodeActor()),
                   "Node2");

            var pid = new PID(string.Format("127.0.0.1:6001"), "Node1");
            pid.RequestAsync<Done>(new RegisterPid() { Sender = localPid }).Wait();

            for (int i = 1; i < 1000; i++)
            {
                LoadData(pid, i);
            }

            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    Task.Delay(1000).Wait();
                    Console.WriteLine("calc:{0}", count);
                }
            });

            Console.WriteLine("Node2 Started!");
            Console.Read();
        }

        static int count;

        static void LoadData(PID pid,int actionId)
        {
            var subscribe = EventSystem.Instance.Subscribe(Callback, Dispatchers.SynchronousDispatcher);
            pid.Tell(new RequestData() { EventId = subscribe.Id.ToString(), MessageData = actionId.ToString() });
            //var package = await pid.RequestAsync<ResponsePackage>(new RequestPackage() { ActionId = actionId, ReceiveTime = DateTime.Now.Ticks });
            //dict.Add(package.ActionId, (int)(package.ResponseTime - package.ReceiveTime));
            //Console.WriteLine("ResponsePackage:{0}", package.ActionId);
        }

        static void Callback(object data)
        {
            Console.WriteLine(data);
        }

        public class NodeActor : IActor
        {
            public Task ReceiveAsync(IContext context)
            {
                switch (context.Message)
                {
                    case ResponseData response:
                        EventSystem.Instance.Publish(response.EventId, response.Data);
                        EventSystem.Instance.Unsubscribe(response.EventId);
                        break;
                }
                return Actor.Done;
            }
        }
    }
}
