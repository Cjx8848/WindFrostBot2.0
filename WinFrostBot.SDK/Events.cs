using System;

namespace WindFrostBot.SDK
{
    public class GroupMessageEvent : EventArgs
    {
        public string GroupOpenID = "";
        public string UserOpenID = "";
        public QCommand Api;
        public GroupMessageEvent(string groupOpenID, string userOpenID ,string eventid)
        {
            GroupOpenID = groupOpenID;
            UserOpenID = userOpenID;
            var eventArg = new MessageEventArgs(GroupOpenID, eventid, userOpenID);
            Api = new QCommand(eventArg, 2);
        }
    }
    public class MessageEvent : EventArgs
    {
        public string UserOpenID = "";
    }
}
