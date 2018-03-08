using System;
using System.Collections.Generic;
using System.Text;
using PlusGame.Message;

namespace PlusGame.Framework.Contract
{
    public static class PackageExtension
    {
        public static void InitPackage(this ResponsePackage response, RequestPackage request)
        {
            response.MsgId = request.MsgId;
            response.ActionId = request.ActionId;
            response.SessionId = request.SessionId;
            response.UserId = request.UserId;
            response.St = request.St;
            response.ReceiveTime = request.ReceiveTime;
        }

        public static void IntiPackage(this ResponseData response, RequestData request)
        {
            response.MessageType = request.MessageType;
        }
    }
}
