using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using PlusGame.Message;
using PlusGame.Common;
using PlusGame.Common.Log;
using PlusGame.Common.Security;
using PlusGame.Framework.Config;
using PlusGame.Framework.IO;
using PlusGame.Framework.Session;
using Google.Protobuf;

namespace PlusGame.Framework.Contract
{
    /// <summary>
    /// Action分发器接口
    /// </summary>
    public interface IActionDispatcher
    {
        /// <summary>
        /// decode package for socket
        /// </summary>
        /// <param name="e"></param>
        /// <param name="package"></param>
        /// <returns></returns>
        bool TryDecodePackage(IByteBuffer data, out RequestPackage package);

        /// <summary>
        /// encode package for socket
        /// </summary>
        /// <param name="package"></param>
        /// <param name="buffer"></param>
        /// <returns></returns>
        bool TryEncodePackage(ResponsePackage package, out IByteBuffer buffer);
    }

    /// <summary>
    /// Action分发器
    /// </summary>
    //public class PlusDispatcher : IActionDispatcher
    //{
    //    private static readonly string PrefixParamChar = "?d=";
    //    private static readonly string SignParamChar = "&sign=";
    //    private static readonly Encoding encoding = Encoding.UTF8;

    //    /// <summary>
    //    /// Decode request package
    //    /// </summary>
    //    /// <param name="e"></param>
    //    /// <param name="package"></param>
    //    /// <returns></returns>
    //    public bool TryDecodePackage(IByteBuffer data, out RequestPackage package)
    //    {
    //        package = new RequestPackage();
    //        package.ReceiveTime = (long)MathUtils.GetUnixEpochTimeSpan(DateTime.Now).TotalMilliseconds;

    //        string paramString = data.ToString(encoding);
    //        string str = paramString;

    //        int index = paramString.IndexOf(PrefixParamChar, StringComparison.OrdinalIgnoreCase);
    //        if (index != -1)
    //        {
    //            str = paramString.Substring(index + PrefixParamChar.Length);
    //            str = HttpUtility.UrlDecode(str) ?? "";
    //        }

    //        if (!str.Contains("="))
    //        {
    //            TraceLog.ReleaseWriteDebug("Parse request error:{0}", paramString);
    //            return false;
    //        }

    //        var nvc = HttpUtility.ParseQueryString(str);

    //        Dictionary<string, string> dict = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
    //        foreach (var key in nvc.AllKeys)
    //        {
    //            if (string.IsNullOrEmpty(key))
    //                continue;
    //            var val = nvc[key];
    //            dict[key] = val;
    //        }

    //        #region 签名校验

    //        string signKey = GameEnvironment.AppConfig != null ? GameEnvironment.AppConfig.SignKey : "";
    //        if (!string.IsNullOrEmpty(signKey))
    //        {
    //            if (!dict.Keys.Contains("sign"))
    //            {
    //                return false;
    //            }
    //            string sign = dict["sign"];
    //            dict.Remove("sign");

    //            int signIndex = str.LastIndexOf(SignParamChar, StringComparison.OrdinalIgnoreCase);
    //            if (signIndex == -1)
    //            {
    //                return false;
    //            }
    //            str = str.Substring(0, signIndex);

    //            string attachParam = str + signKey;
    //            string key = CryptoHelper.MD5_Encrypt(attachParam, Encoding.UTF8);
    //            if (!string.Equals(key.ToLower(), sign))
    //            {
    //                return false;
    //            }
    //        }

    //        #endregion

    //        #region 读取协议头

    //        if (!dict.Keys.Contains("msgid"))
    //        {
    //            return false;
    //        }
    //        package.MsgId = dict["msgid"].ToInt();
    //        dict.Remove("msgid");

    //        if (!dict.Keys.Contains("actionid"))
    //        {
    //            return false;
    //        }
    //        package.ActionId = dict["actionId"].ToInt();
    //        dict.Remove("actionId");

    //        if (!dict.Keys.Contains("sid"))
    //        {
    //            return false;
    //        }
    //        package.SessionId = dict["sid"];
    //        dict.Remove("sid");

    //        if (!dict.Keys.Contains("uid"))
    //        {
    //            return false;
    //        }
    //        package.UserId = dict["uid"].ToInt();
    //        dict.Remove("uid");

    //        if (!dict.Keys.Contains("st"))
    //        {
    //            return false;
    //        }
    //        package.St = dict["st"];
    //        dict.Remove("st");

    //        #endregion

    //        foreach (var key in dict.Keys)
    //        {
    //            if (string.IsNullOrEmpty(key))
    //                continue;

    //            var val = dict[key];
    //            package.DictData[key] = val;
    //        }

    //        return true;
    //    }

    //    public bool TryEncodePackage(ResponsePackage package, out IByteBuffer buffer)
    //    {
    //        MessageHead head = new MessageHead(package.MsgId, package.ActionId, package.St, package.ErrorCode, package.ErrorInfo);
    //        MessageStructure sb = new MessageStructure();
    //        sb.WriteBuffer(head);
    //        if (data != null && data.Length != 0)
    //            sb.WriteByte(data);
    //        return Unpooled.WrappedBuffer(sb.PopBuffer());
    //    }

    //    public bool CheckSpecialPackge(RequestPackage package, GameSession session)
    //    {
    //        ////处理特殊包
    //        //if (package.ActionId == (int)ActionEnum.Interrupt)
    //        //{
    //        //    if (session != null)
    //        //    {
    //        //        session.Close();
    //        //    }
    //        //    return true;
    //        //}
    //        if (package.ActionId == (int)ActionEnum.Heartbeat)
    //        {
    //            if (session != null)
    //            {
    //                // 客户端tcp心跳包
    //                session.Refresh();
    //            }
    //            return true;
    //        }
    //        return false;
    //    }

    //    public bool TryBuildPackage(IChannelHandlerContext context, IByteBuffer buffer, out string message, out RequestPackage package)
    //    {
    //        message = "";
    //        package = null;
    //        try
    //        {
    //            if (!GameEnvironment.ActionDispatcher.TryDecodePackage(buffer, out package))
    //            {
    //                message = "解包失败或签名错误";
    //                return false;
    //            }

    //            var session = SessionManager.Get(context);
    //            if (CheckSpecialPackge(package, session))
    //            {
    //                return false;
    //            }

    //            package.SessionId = session.SessionId;

    //            return true;
    //        }
    //        catch (Exception ex)
    //        {
    //            message = ex.Message;
    //            TraceLog.WriteError("接受数据:{0},{1}", ex.Message, ex.StackTrace);
    //        }
    //        return false;
    //    }

    //}

    /// <summary>
    /// 
    /// </summary>
    public class ProtoDispatcher: IActionDispatcher
    {
        /// <summary>
        /// decode package for socket
        /// </summary>
        /// <param name="e"></param>
        /// <param name="package"></param>
        /// <returns></returns>
        public bool TryDecodePackage(IByteBuffer data, out RequestPackage package)
        {
            package = null;
            try
            {
                package = RequestPackage.Parser.ParseFrom(data.Array);
                //统计时间
                package.ReceiveTime = DateTime.Now.Ticks;
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("decode package error :{0},{1}", ex.Message, ex.StackTrace);
                return false;
            }
            return true;
        }

        /// <summary>
        /// encode package for socket
        /// </summary>
        /// <param name="package"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool TryEncodePackage(ResponsePackage package, out IByteBuffer buffer)
        {
            buffer = null;
            try
            {
                var message = package as IMessage;
                var sendData = message.ToByteArray();
                buffer = Unpooled.WrappedBuffer(sendData);
                //统计时间
                long ticks = DateTime.Now.Ticks - package.ReceiveTime;
                if (ticks > 10)
                {
                    TraceLog.WriteDebug("执行{0}超时:{1}", package.ActionId, ticks);
                }
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("encode package error :{0},{1}", ex.Message, ex.StackTrace);
                return false;
            }
            return true;
        }
    }
}
