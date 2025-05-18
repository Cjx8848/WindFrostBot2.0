using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindFrostBot.SDK
{
    public class GroupMessageEvent : EventArgs
    {
        public string GroupOpenID = "";
        public string UserOpenID = "";
        public GroupMessageEvent(string groupOpenID, string userOpenID)
        {
            GroupOpenID = groupOpenID;
            UserOpenID = userOpenID;
        }
    }
    public class MessageEvent : EventArgs
    {
        public string UserOpenID = "";
    }
}
