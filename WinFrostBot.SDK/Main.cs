using System;
using System.Data;

namespace WindFrostBot.SDK
{
    public class MainSDK
    {
        public static FunctionManager<CommandArgs> OnAtEvent= new FunctionManager<CommandArgs>();
        public static FunctionManager<CommandArgs> OnCommand = new FunctionManager<CommandArgs>();
        public static FunctionManager<GroupMessageEvent> OnGroupAdd = new FunctionManager<GroupMessageEvent>();
        public static FunctionManager<GroupMessageEvent> OnGroupRemove = new FunctionManager<GroupMessageEvent>();
        public static Config BotConfig { get; set; }
        public static IDbConnection Db { get; set; }
        public static BotClient QQClient { get; set; }
    }
    public abstract class Plugin
    {
        public List<Command> Commands = new List<Command>();
        public List<Command> PrivateCommands = new List<Command>();
        public abstract string PluginName();
        public abstract string Version();
        public abstract string Author();
        public abstract string Description();
        public abstract void OnLoad();
        public virtual string OnReload()
        {
            return "";
        }
        public virtual void OnDispose()
        {
            var copylist = new List<Command>(Commands);
            foreach (var command in copylist)
            {
                CommandManager.Coms.Remove(command);
            }
            if (MainSDK.OnCommand.functions.ContainsKey(PluginName()))
            {
                MainSDK.OnCommand.functions.Remove(PluginName());
            }
        }
    }
    public class FunctionManager<T>
    {
        public Dictionary<string, Action<T>> functions = new Dictionary<string, Action<T>>();
        public void AddFunction(Plugin plugin, Action<T> func)
        {
            if (!functions.ContainsKey(plugin.PluginName()))
            {
                functions.Add(plugin.PluginName(), func);
            }
        }
        public void ExecuteAll(T args)
        {
            foreach (var func in functions)
            {
                func.Value(args);
            }
        }
    }
}