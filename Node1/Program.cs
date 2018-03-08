using System;
using Proto.Remote;
using Proto;
using System.Threading.Tasks;
using PlusGame.Message;
using System.Threading;

namespace Node1
{
    class Program
    {
        static void Main(string[] args)
        {
            Serialization.RegisterFileDescriptor(PlusGame.Message.ProtosReflection.Descriptor);
            Remote.Start("127.0.0.1", 6001);

            var pid = Actor.SpawnNamed(
                   Actor.FromProducer(() => new NodeActor()),
                   "Node1");

            Console.WriteLine("Node1 Started!");
            Console.Read();
        }

        public class NodeActor : IActor
        {
            PID client;
            long count;
            public Task ReceiveAsync(IContext context)
            {
                switch (context.Message)
                {
                    case RegisterPid register:
                        client = register.Sender;
                        context.Respond(new Done());
                        break;
                    case RequestData request:
                        Task.Factory.StartNew(() =>
                        {
                            if (Int32.Parse(request.MessageData) % 500 == 0)
                                Task.Delay(10000).Wait();
                            client.Tell(new ResponseData() { EventId = request.EventId, Data = request.MessageData });
                        });
                        break;
                }
                return Actor.Done;
            }
        }
    }
}
