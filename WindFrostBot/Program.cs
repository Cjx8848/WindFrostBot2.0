using System;
using WindFrostBot.SDK;
using Spectre.Console;
using System.Reflection;
using System.Runtime.Loader;
using System.Timers;

namespace WindFrostBot
{
    public class Program
    {
        static void Init()
        {
            AppDomain.CurrentDomain.AssemblyResolve += (sender, eventArgs) =>
            {
                string assemblyName = new AssemblyName(eventArgs.Name).Name;
                string path = Path.Combine(AppContext.BaseDirectory, "bin", $"{assemblyName}.dll");

                if (File.Exists(path))
                {
                    return AssemblyLoadContext.Default.LoadFromAssemblyPath(path);
                }

                return null;
            };
        }
        static void Main(string[] arg)
        {
            Init();
            AnsiConsole.Write(new FigletText("WindFrostBot").Color(Color.Aqua));
            ConfigWriter.InitConfig();
            Message.LogWriter.StartLog();
            DataBase.Init();
            Message.BlueText("WindFrostBot2.0 正在启动...");
            if (MainSDK.BotConfig.EnableLog)
            {
                Message.BlueText("日志功能已开启.");
            }
            if (!File.Exists(PluginLoader.PluginsDirectory))
            {
                Directory.CreateDirectory(PluginLoader.PluginsDirectory);
            }
            PluginLoader.LoadPlugins();
            StartBot();
            Message.BlueText("WindFrostBot2.0 启动成功!");
            for(; ;)
                Console.ReadLine();
        }
        public static bool UnLock = false;//锁
        public static void OnUpdate(object? Sender, EventArgs e)
        {
            if (UnLock) return;
            if (!MainSDK.QQClient.Connected)
            {
                try
                {
                    UnLock = true;
                    Message.Info("重新连接中...");
                    MainSDK.QQClient.Dispose();
                    MainSDK.QQClient = new BotClient(MainSDK.BotConfig.AppID, MainSDK.BotConfig.Secret);
                    CommandManager.InitCommandToBot();
                }
                catch
                {
                    UnLock = false;
                }
            }
            UnLock = false;
        }
        static readonly System.Timers.Timer Update = new System.Timers.Timer(1000);//秒时钟
        public static  void StartBot()
        {
            MainSDK.QQClient = new BotClient(MainSDK.BotConfig.AppID, MainSDK.BotConfig.Secret);
            CommandManager.InitCommandToBot();
            Update.Elapsed += OnUpdate;
            Update.Start();
        }
    }
}