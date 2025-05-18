using System;
using System.Drawing;

namespace WindFrostBot.SDK
{
    public class QCommand
    {
        public MessageEventArgs eventArgs { get; private set; }
        public int Type = 0;
        public int seq = 1;
        public QCommand(MessageEventArgs eventArgs , int type = 0)
        {
            this.eventArgs = eventArgs;
            this.Type = type;
        }
        public void SendKeyBoard(string keyid)
        {
            switch (Type)
            {
                case 0:
                    MainSDK.QQClient.SendKeyboard(keyid, eventArgs, seq);
                    seq++;
                    break;
                case 1:
                    seq++;
                    break;
                default:
                    break;
            }
        }
        public void SendTextMessage(string message)
        {
            switch(Type)
            {
                case 0:
                    MainSDK.QQClient.SendGroupMessage("\n" + message, eventArgs, seq);
                    seq++;
                    break;
                case 1:
                    MainSDK.QQClient.SendMessage(message, eventArgs, seq);
                    seq++;
                    break;
                default:
                    break;
            }
        }
        public void SendTextWithImage(byte[] data,string text, string name = "upload")
        {
            switch (Type)
            {
                case 0:
                    var date = DateTime.Now;
                    MainSDK.QQClient.SendGroupMedia(eventArgs, data, text, seq, $"{date.ToString().Replace("/", "-").Replace(" ", "_").Replace(":", "-")}.jpg");
                    seq++;
                    break;
                case 1:
                    break;
            }
        }
        public void SendImage(byte[] data,string name = "upload")
        {
            switch (Type)
            {
                case 0:
                    var date = DateTime.Now;
                    MainSDK.QQClient.SendGroupMedia(eventArgs, data, seq, $"{date.ToString().Replace("/", "-").Replace(" ", "_").Replace(":","-")}.jpg");
                    seq++;
                    break;
                case 1:
                    break;
            }
        }
        public void SendImage(string url)
        {
            switch (Type)
            {
                case 0:
                    MainSDK.QQClient.SendGroupMedia(eventArgs, url, seq);
                    seq++;
                    break;
                case 1:
                    break;
            }
        }
    }
}
