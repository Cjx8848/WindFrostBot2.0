using System;
using System.Diagnostics.Tracing;

namespace WindFrostBot.SDK
{
    public delegate void ComDelegate(CommandArgs args);
    public class CommandArgs : EventArgs
    {
        public string Message { get; private set; }
        public List<string> Parameters { get; private set; }
        public MessageEventArgs eventArgs { get; private set; }
        public QCommand Api { get; private set; }
        public bool Handled = false;
        public List<Attachment> Attachments = new List<Attachment>();
        public CommandArgs(string msg,List<string> args, QCommand cmd)
        {
            Parameters = args;
            Message = msg;
            eventArgs = cmd.eventArgs;
            Api = cmd;
            //EventArgs = eventarg;
        }
        public CommandArgs(string msg, List<string> args, QCommand cmd , List<Attachment> atts)
        {
            Parameters = args;
            Message = msg;
            eventArgs = cmd.eventArgs;
            Api = cmd;
            Attachments = atts;
            //EventArgs = eventarg;
        }
        public bool IsOwner()
        {
            if(MainSDK.BotConfig.OwnerOpenID == eventArgs.Author)
            {
                return true;
            }
            return false;
        }
    }
    public class CommandManager
    {
        public static List<Command> Coms = new List<Command>();
        public static List<Command> PrivateComs = new List<Command>();
        public static void InitCommandToBot()
        {
            var client = MainSDK.QQClient;
            client.OnMessageReceived += (sender, e) => //私聊消息部分
            {
                if (!string.IsNullOrEmpty(e.Content))
                {
                    string text = e.Content;//接收的所有消息
                    string msg = text.Split(" ")[0].ToLower().Replace("/", "");//指令消息
                    List<string> arg = text.Split(" ").ToList();
                    arg.Remove(text.Split(" ")[0]);//除去指令消息的其他段消息
                    var cmd = PrivateComs.Find(c => c.Names.Contains(msg));
                    if (cmd != null)
                    {
                        if (cmd.Type == 1)
                        {
                            try
                            {
                                var handler = new CommandArgs(msg, arg, new QCommand(e, 1), e.Attachments);
                                MainSDK.OnCommand.ExecuteAll(handler);
                                if (!handler.Handled)
                                {
                                    cmd.Run(msg, arg, handler.Api, e.Attachments);
                                }
                            }
                            catch (Exception ex)
                            {
                                Message.LogErro(ex.Message);
                            }
                        }
                    }
                    else
                    {
                        var qcmd = new QCommand(e, 1);
                        qcmd.SendTextMessage("不存在该指令~\n没准在幻想乡?");
                    }
                }
            };
            client.OnGroupMessageReceived += (sender, e) => //群聊消息部分
            {
                string text = e.Content.Substring(1);//接收的所有消息
                string msg = text.Split(" ")[0].ToLower().Replace("/","");//指令消息
                List<string> arg = text.Split(" ").ToList();
                arg.Remove(text.Split(" ")[0]);//除去指令消息的其他段消息
                var cmd = Coms.Find(c => c.Names.Contains(msg));
                var handler = new CommandArgs(msg, arg, new QCommand(e), e.Attachments);
                if (cmd != null)
                {
                    if (cmd.Type == 0)
                    {
                        try
                        {
                            MainSDK.OnCommand.ExecuteAll(handler);
                            if (!handler.Handled)
                            {
                                cmd.Run(msg, arg, handler.Api, e.Attachments);
                            }
                        }
                        catch (Exception ex)
                        {
                            Message.LogErro(ex.Message);
                        }
                    }
                }
                else
                {
                    MainSDK.OnAtEvent.ExecuteAll(handler);
                    if (!handler.Handled)
                    {
                        var qcmd = new QCommand(e, 0);
                        qcmd.SendTextMessage("不存在该指令~\n没准在幻想乡?");
                    }
                }
            };
        }
        public static void InitGroupCommand(Plugin plugin,ComDelegate cmd,string cmdinfo,params string[] cmdnames)
        {
            Command com = new Command(cmd, cmdinfo, 0, cmdnames);
            plugin.Commands.Add(com);
            Coms.Add(com);
        }
        public static void InitPrivateCommand(Plugin plugin, ComDelegate cmd, string cmdinfo, params string[] cmdnames)
        {
            Command com = new Command(cmd, cmdinfo, 1, cmdnames);
            plugin.PrivateCommands.Add(com);
            PrivateComs.Add(com);
        }
    }
    public class Command
    {
        private ComDelegate cd;
        public int Type;
        public ComDelegate Cd
        {
            get
            {
                return cd;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException();
                }

                cd = value;
            }
        }
        public Command(ComDelegate cmd, int type,params string[] names)
        {
            if (cmd == null)
            {
                throw new ArgumentNullException("cmd");
            }

            if (names == null || names.Length < 1)
            {
                throw new ArgumentException("names");
            }
            Names = new List<string>(names);
            cd = cmd;
            HelpText = "此指令没有帮助.";
            Type = type;
        }
        public Command(ComDelegate cmd, string help, int type ,params string[] names)
        {
            if (cmd == null)
            {
                throw new ArgumentNullException("cmd");
            }

            if (names == null || names.Length < 1)
            {
                throw new ArgumentException("names");
            }
            Names = new List<string>(names);
            cd = cmd;
            HelpText = help;
            Type = type;
        }
        public List<string> Names = new List<string>();
        public string HelpText = "";
        public bool Run(string msg, List<string> parms, QCommand cmd ,List<Attachment> atts)
        {
            try
            {
                cd(new CommandArgs(msg, parms, cmd, atts));
            }
            catch (Exception ex)
            {
                Message.Erro("指令出错!:" + ex.ToString());
            }
            return true;
        }
        public bool Run(string msg,List<string> parms,QCommand cmd)
        {
            try
            {
                cd(new CommandArgs(msg, parms, cmd));
            }
            catch(Exception ex)
            {
                Message.Erro("指令出错!:" + ex.ToString());
            }
            return true;
        }
    }
}
