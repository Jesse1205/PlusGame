using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using PlusGame.Message;
using Newtonsoft.Json;
using PlusGame.Common;
using PlusGame.Common.Log;
using PlusGame.Framework.Config;
using PlusGame.Framework.Session;

namespace PlusGame.Framework.Contract
{
    public class SessionManager
    {
        private static ConcurrentDictionary<Guid, GameSession> _globalSession;
        /// <summary>
        /// UserId与session映射关系
        /// </summary>
        private static ConcurrentDictionary<int, Guid> _userHash;
        /// <summary>
        /// handler与session映射关系
        /// </summary>
        private static ConcurrentDictionary<IChannelHandlerContext, Guid> _handlerHash;

        private static Timer clearTime;
        private static int isClearWorking;

        /// <summary>
        /// Heartbeat timeout(sec), default 60s
        /// </summary>
        public static int HeartbeatTimeout { get; set; }

        //超时事件
        public static event Action<GameSession> HeartbeatTimeoutHandle;
        private static void DoHeartbeatTimeout(GameSession session)
        {
            try
            {
                Action<GameSession> handler = HeartbeatTimeoutHandle;
                if (handler != null) handler(session);
            }
            catch (Exception)
            {
            }
        }

        static SessionManager()
        {
            HeartbeatTimeout = 60;//60s
            clearTime = new Timer(OnClearSession, null, 6000, HeartbeatTimeout * 1000);

            _globalSession = new ConcurrentDictionary<Guid, GameSession>();
            _userHash = new ConcurrentDictionary<int, Guid>();
            _handlerHash = new ConcurrentDictionary<IChannelHandlerContext, Guid>();
        }

        /// <summary>
        /// 清除过期Session
        /// </summary>
        /// <param name="state"></param>
        private static void OnClearSession(object state)
        {
            if (Interlocked.CompareExchange(ref isClearWorking, 1, 0) == 0)
            {
                try
                {
                    var sessions = new List<GameSession>();
                    foreach (var pair in _globalSession)
                    {
                        var session = pair.Value;
                        if (session == null) continue;

                        if (HeartbeatTimeout > 0 &&
                            session.LastActivityTime < MathUtils.Now.AddSeconds(-HeartbeatTimeout))
                        {
                            DoHeartbeatTimeout(session);
                            if (_globalSession.TryRemove(pair.Key, out session))
                            {
                                Guid guid;
                                _handlerHash.TryRemove(session.Context, out guid);
                            }
                        }
                    }
                }
                catch (Exception er)
                {
                    TraceLog.WriteError("ClearSession error:{0}", er);
                }
                finally
                {
                    Interlocked.Exchange(ref isClearWorking, 0);
                }
            }
        }

        #region 绑定Session 与 session校验

        public static bool Bind(GameSession session, int userId)
        {
            _userHash[userId] = session.KeyCode;
            session.Bind(userId);
            return true;
        }

        /// <summary>
        /// 验证是否登录
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static bool CheckLogin(GameSession session, int userId)
        {
            Guid guid;
            if (_userHash.TryGetValue(userId, out guid) && Guid.Equals(guid, session.KeyCode))
                return true;
            return false;
        }

        #endregion

        public static string GetSessionId(IChannelHandlerContext context)
        {
            string keyCode = context.GetHashCode().ToNotNullString();
            return string.Format("s_{0}_{1}_{2}", keyCode, GameEnvironment.AppConfig.GameId, GameEnvironment.AppConfig.ServerId);
        }

        #region 创建Session

        /// <summary>
        /// Add session to cache
        /// </summary>
        /// <param name="keyCode"></param>
        /// <param name="socket"></param>
        /// <param name="appServer"></param>
        public static GameSession CreateNew(IChannelHandlerContext context)
        {
            Guid guid = Guid.NewGuid();
            GameSession session = new GameSession(guid, context);
            _handlerHash[context] = guid;
            _globalSession[guid] = session;
            return session;
        }

        #endregion

        #region 获取Session

        public static int GetCount()
        {
            return _globalSession.Values.Count;
        }

        public static GameSession Get(int userId)
        {
            GameSession session;
            Guid guid;
            if (_userHash.TryGetValue(userId, out guid) && _globalSession.TryGetValue(guid, out session))
            {
                return session;
            }
            return null;
        }

        public static GameSession Get(IChannelHandlerContext context)
        {
            GameSession session;
            Guid guid;
            if (_handlerHash.TryGetValue(context, out guid) && _globalSession.TryGetValue(guid, out session))
            {
                return session;
            }
            return null;
        }

        /// <summary>
        /// Get session by sessionid.
        /// </summary>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public static GameSession Get(string sessionId)
        {
            GameSession session = null;
            Guid guid;
            if (TryParseSessionId(sessionId, out guid) && 
                _globalSession.TryGetValue(guid, out session))
            {
                return session;
            }
            return null;
        }

        public static bool TryParseSessionId(string sessionId, out Guid sid)
        {
            sid = Guid.Empty;
            string[] arr = (sessionId ?? "").Split('_', '|');
            if (arr.Length > 1)
            {
                Guid hashCode;
                if (Guid.TryParse(arr[1], out hashCode))
                {
                    sid = hashCode;
                    return true;
                }
            }
            return false;
        }

        #endregion

        //public static void PostSend(int userId, IByteBuffer data)
        //{
        //    var session = Get(userId);
        //    if (session != null)
        //    {
        //        try
        //        {
        //            session.PostSend(data);
        //        }
        //        catch (Exception ex)
        //        {
        //            TraceLog.WriteError("PostSend Error {0},{1},{2},{3}", userId, JsonConvert.SerializeObject(data), ex.Message, ex.StackTrace);
        //        }
        //    }
        //}

        //public static void PostSend(string sessionId, IByteBuffer data)
        //{
        //    var session = Get(sessionId);
        //    if (session != null)
        //    {
        //        try
        //        {
        //            session.PostSend(data);
        //        }
        //        catch (Exception ex)
        //        {
        //            TraceLog.WriteError("PostSend Error {0},{1},{2},{3}", sessionId, JsonConvert.SerializeObject(data), ex.Message, ex.StackTrace);
        //        }
        //    }
        //}
    }
}
