using WindFrostBot;
using WindFrostBot.SDK;

namespace InitPlugin
{
    public class InitPlugin : Plugin
    {
        public override string PluginName()
        {
            return "InitPlugin";
        }

        public override string Version()
        {
            return "1.0";
        }

        public override string Author()
        {
            return "Cjx";
        }

        public override string Description()
        {
            return "InitPlugin";
        }
        public override void OnLoad()
        {
            CommandManager.InitGroupCommand(this, Reload, "重读指令", "reload", "重读");
            CommandManager.InitPrivateCommand(this, Reload, "重读指令", "reload", "重读");
        }
        public static void Reload(CommandArgs args)
        {
            if (!args.IsOwner())
            {
                return;
            }
            try
            {
                int number = 0;
                string reloadtext = "";
                foreach (var plugin in PluginLoader.Plugins)
                {
                    string result = plugin.OnReload();
                    number++;
                    if (!string.IsNullOrEmpty(result))
                    {
                        reloadtext += $"\n[{plugin.PluginName()}]{result}";
                    }
                }
                args.Api.SendTextMessage($"[{ConfigWriter.GetConfig().BotName}]成功执行了 {number} 个插件的重读函数!{reloadtext}");
            }
            catch (Exception ex)
            {
                args.Api.SendTextMessage($"[{ConfigWriter.GetConfig().BotName}]重读出错!");
            }
        }
    }
}