using PlusGame.Common.Event;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace GameServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var subscribe = EventSystem.Instance.Subscribe((m) => Console.WriteLine(m), Dispatchers.DefaultDispatcher);

            for (int i = 1; i < 100; i++)
            {
                EventSystem.Instance.Publish(i);
                EventSystem.Instance.Unsubscribe(subscribe.Id);
            }

            Console.WriteLine("Hello World!");
            Console.Read();
        }
    }
}
