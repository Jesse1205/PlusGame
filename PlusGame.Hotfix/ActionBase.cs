using PlusGame.Message;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlusGame.Hotfix
{
    public abstract class ActionBase
    {
        int _actionId;
        RequestPackage _request;
        ResponsePackage _response;

        public ActionBase(int actionId, RequestPackage request, ResponsePackage response)
        {
            _actionId = actionId;
            _request = request;
            _response = response;
        }

        public virtual bool GetUrlElement()
        {
            return true;
        }

        public virtual bool TakeAction()
        {
            return true;
        }

        public virtual void BuildPackage()
        {

        }
    }
}
