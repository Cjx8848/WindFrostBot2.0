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
                    break;
                case 1:
                    break;
                default:
                    break;
            }
            seq++;
        }
        public void SendTextMessage(string message)
        {
            switch(Type)
            {
                case 0:
                    MainSDK.QQClient.SendGroupMessage("\n" + message, eventArgs, seq);
                    break;
                case 1:
                    MainSDK.QQClient.SendMessage(message, eventArgs, seq);
                    break;
                case 2:
                    MainSDK.QQClient.SendGroupMessage(message, eventArgs, eventArgs.EventId, seq);
                    break;
                default:
                    break;
            }
            seq++;
        }
        public void SendTextWithImage(byte[] data,string text, string name = "upload")
        {
            var date = DateTime.Now;
            switch (Type)
            {
                case 0:
                    MainSDK.QQClient.SendGroupMedia(eventArgs, data, text, seq, $"{date.ToString().Replace("/", "-").Replace(" ", "_").Replace(":", "-")}.jpg");
                    break;
                case 1:
                    MainSDK.QQClient.SendMessageMedia(eventArgs, data, text, seq, $"{date.ToString().Replace("/", "-").Replace(" ", "_").Replace(":", "-")}.jpg");
                    break;
                case 2:
                    MainSDK.QQClient.SendGroupMedia(eventArgs, data, text, eventArgs.EventId, seq, $"{date.ToString().Replace("/", "-").Replace(" ", "_").Replace(":", "-")}.jpg");
                    break;
            }
            seq++;
        }
        public void SendImage(byte[] data,string name = "upload")
        {
            var date = DateTime.Now;
            switch (Type)
            {

                case 0:
                    MainSDK.QQClient.SendGroupMedia(eventArgs, data, seq, $"{date.ToString().Replace("/", "-").Replace(" ", "_").Replace(":","-")}.jpg");
                    seq++;
                    break;
                case 1:
                    MainSDK.QQClient.SendMessageMedia(eventArgs, data, seq, $"{date.ToString().Replace("/", "-").Replace(" ", "_").Replace(":", "-")}.jpg");
                    break;
                case 2:
                    MainSDK.QQClient.SendGroupMedia(eventArgs, data, "", eventArgs.EventId, seq, $"{date.ToString().Replace("/", "-").Replace(" ", "_").Replace(":", "-")}.jpg");
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
                    MainSDK.QQClient.SendMessageMedia(eventArgs, url, seq);
                    break;
                case 2:
                    MainSDK.QQClient.SendGroupMedia(eventArgs, url, eventArgs.EventId, seq);
                    break;
            }
        }
    }
}
