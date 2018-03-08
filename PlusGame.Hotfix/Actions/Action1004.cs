using PlusGame.Message;
using System;
using System.Collections.Generic;

namespace PlusGame.Hotfix.Actions
{
    public class Action1004 : ActionBase
    {
        public Action1004(RequestPackage request, ResponsePackage response)
            : base(1004, request, response)
        {

        }

        public override bool GetUrlElement()
        {
            return true;
        }

        public override bool TakeAction()
        {
            return true;
        }

        public override void BuildPackage()
        {

        }
    }
}
