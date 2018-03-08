using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using Newtonsoft.Json;
using PlusGame.Common.Log;
using PlusGame.Framework.Config;

namespace PlusGame.Framework.Session
{
    public class GameSession
    {
        #region property

        /// <summary>
        /// Remote end address
        /// </summary>
        public string RemoteAddress { get; private set; }

        /// <summary>
        /// SessionId
        /// </summary>
        public string SessionId { get; private set; }

        /// <summary>
        /// 最后活动时间
        /// </summary>
        public DateTime LastActivityTime { get; private set; }

        public Guid KeyCode { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public IChannelHandlerContext Context { get; private set; }

        public int UserId { get; private set; }

        #endregion

        public GameSession(Guid guid, IChannelHandlerContext context)
        {
            Context = context;
            RemoteAddress = context.Channel.RemoteAddress.ToString();
            KeyCode = guid;
            SessionId = string.Format("s_{0}|{1}|{2}", guid.ToString("N"), GameEnvironment.AppConfig.GameId, GameEnvironment.AppConfig.ServerId);
            Refresh();
        }

        public void Bind(int userId)
        {
            UserId = userId;
        }

        /// <summary>
        /// 刷新心跳时间
        /// </summary>
        internal void Refresh()
        {
            LastActivityTime = DateTime.Now;
        }

        public void PostSend(IByteBuffer data)
        {
            Context.WriteAsync(data);
        }
    }
}
