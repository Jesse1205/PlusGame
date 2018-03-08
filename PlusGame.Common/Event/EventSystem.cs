using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PlusGame.Common.Event
{
    public interface IDispatcher
    {
        void Schedule(Func<Task> runner);
    }

    public static class Dispatchers
    {
        public static ThreadPoolDispatcher DefaultDispatcher { get; } = new ThreadPoolDispatcher();
        public static SynchronousDispatcher SynchronousDispatcher { get; } = new SynchronousDispatcher();
    }

    public class ThreadPoolDispatcher : IDispatcher
    {
        public void Schedule(Func<Task> runner)
        {
            Task.Factory.StartNew(runner, TaskCreationOptions.None);
        }
    }

    public class SynchronousDispatcher : IDispatcher
    {
        public void Schedule(Func<Task> runner)
        {
            runner().Wait();
        }
    }

    public class EventSystem : EventSystem<object>
    {
        public static readonly EventSystem Instance = new EventSystem();

        internal EventSystem()
        {
            //Subscribe(msg =>
            //{
            //    if (msg is DeadLetterEvent letter)
            //    {
            //        _logger.LogInformation("[DeadLetter] '{0}' got '{1}:{2}' from '{3}'", letter.Pid.ToShortString(),
            //            letter.Message.GetType().Name, letter.Message, letter.Sender?.ToShortString());
            //    }
            //});
        }
    }

    public class EventSystem<T>
    {
        private readonly ConcurrentDictionary<string, Subscription<T>> _subscriptions =
            new ConcurrentDictionary<string, Subscription<T>>();

        internal EventSystem()
        {
        }

        public Subscription<T> Subscribe(Action<T> action, IDispatcher dispatcher)
        {
            var sub = new Subscription<T>(this, dispatcher, x =>
            {
                action(x);
                return Task.FromResult(0);
            });
            _subscriptions.TryAdd(sub.Id, sub);
            return sub;
        }

        public Subscription<T> Subscribe(Func<T, Task> action, IDispatcher dispatcher)
        {
            var sub = new Subscription<T>(this, dispatcher, action);
            _subscriptions.TryAdd(sub.Id, sub);
            return sub;
        }

        public Subscription<T> Subscribe<TMsg>(Action<TMsg> action, IDispatcher dispatcher) where TMsg : T
        {
            var sub = new Subscription<T>(this, dispatcher, msg =>
            {
                if (msg is TMsg typed)
                {
                    action(typed);
                }
                return Task.FromResult(0);
            });

            _subscriptions.TryAdd(sub.Id, sub);
            return sub;
        }

        public void Publish(string eventId, T msg)
        {
            Subscription<T> sub;
            if (_subscriptions.TryGetValue(eventId, out sub))
            {
                sub.Dispatcher.Schedule(() =>
                {
                    try
                    {
                        sub.Action(msg);
                    }
                    catch (Exception ex)
                    {

                    }
                    return Task.FromResult(0);
                });
            }
        }

        public void Publish(T msg)
        {
            foreach (var sub in _subscriptions)
            {
                sub.Value.Dispatcher.Schedule(() =>
                {
                    try
                    {
                        sub.Value.Action(msg);
                    }
                    catch (Exception ex)
                    {

                    }
                    return Task.FromResult(0);
                });
            }
        }

        public void Unsubscribe(string id)
        {
            _subscriptions.TryRemove(id, out var _);
        }
    }

    public class Subscription<T>
    {
        private readonly EventSystem<T> _EventSystem;
        private static long index;

        public Subscription(EventSystem<T> EventSystem, IDispatcher dispatcher, Func<T, Task> action)
        {
            Id = Guid.NewGuid().ToString("N");
            _EventSystem = EventSystem;
            Dispatcher = dispatcher;
            Action = action;
        }

        public string Id { get; }
        public IDispatcher Dispatcher { get; }
        public Func<T, Task> Action { get; }

        public void Unsubscribe()
        {
            _EventSystem.Unsubscribe(Id);
        }
    }
}
