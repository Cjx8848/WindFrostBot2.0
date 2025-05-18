using WindFrostBot.SDK;
using OpenQA.Selenium.Chrome;
using System.Drawing;
using TerraWiki;
using JsonTool;

namespace TerraWikiPluin
{
    public class WikiPlugin : Plugin
    {
        public override string PluginName()
        {
            return "WikiPlugin";
        }

        public override string Version()
        {
            return "2.0";
        }

        public override string Author()
        {
            return "Cjx";
        }

        public override string Description()
        {
            return "ExamplePlugin";
        }
        public override void OnLoad()
        {
            var path = Path.Combine(AppContext.BaseDirectory, "TerrariaID");
            var itemjson = new JsonRw<List<ItemInfo>>(path + "/item_id.json");
            ItemInfos = itemjson.ConfigObj;
            var buffjson = new JsonRw<List<BuffInfo>>(path + "/buff_id.json");
            BuffInfos = buffjson.ConfigObj;
            var npcjson = new JsonRw<List<NpcInfo>>(path + "/npc_id.json");
            NpcInfos = npcjson.ConfigObj;
            var projectjson = new JsonRw<List<ProjectInfo>>(path + "/project_id.json");
            ProjectInfos = projectjson.ConfigObj;
            var prefixjson = new JsonRw<List<PrefixInfo>>(path + "/prefix_id.json");
            PrefixInfos = prefixjson.ConfigObj;
            CommandManager.InitGroupCommand(this, WikiCommand, "wiki指令", "wiki");
            CommandManager.InitGroupCommand(this, SearchItems, "搜物品", "搜物品", "物品", "si");
            CommandManager.InitGroupCommand(this, SearchBuffs, "搜增益", "搜增益", "增益", "sb");
            CommandManager.InitGroupCommand(this, SearchProjects, "搜弹幕", "搜弹幕", "弹幕", "sp");
            CommandManager.InitGroupCommand(this, SearchPrefix, "搜前缀", "搜前缀", "前缀", "spr");
            CommandManager.InitGroupCommand(this, SearchNpcs, "搜生物", "搜生物", "生物", "sn");
            //CommandManager.InitGroupCommand(this, About, "关于", "about");
        }
        public override string OnReload()
        {
            var path = Path.Combine(AppContext.BaseDirectory, "TerrariaID");
            var itemjson = new JsonRw<List<ItemInfo>>(path + "/item_id.json");
            ItemInfos = itemjson.ConfigObj;
            var buffjson = new JsonRw<List<BuffInfo>>(path + "/buff_id.json");
            BuffInfos = buffjson.ConfigObj;
            var npcjson = new JsonRw<List<NpcInfo>>(path + "/npc_id.json");
            NpcInfos = npcjson.ConfigObj;
            var projectjson = new JsonRw<List<ProjectInfo>>(path + "/project_id.json");
            ProjectInfos = projectjson.ConfigObj;
            var prefixjson = new JsonRw<List<PrefixInfo>>(path + "/prefix_id.json");
            PrefixInfos = prefixjson.ConfigObj;
            return "[TerraWiki]重读成功!";
        }
        public static List<ItemInfo> ItemInfos;
        public static List<BuffInfo> BuffInfos;
        public static List<NpcInfo> NpcInfos;
        public static List<ProjectInfo> ProjectInfos;
        public static List<PrefixInfo> PrefixInfos;
        static string HelpText = "使用'si <内容>'搜索物品\n使用'sb <内容>'搜索buff\n使用'sp <内容>'搜索弹幕\n使用'sn <内容>'搜索NPC\n使用'spr <内容>'搜索前缀\n使用'wiki <内容>'获取泰拉瑞亚wiki页面";
        public static void SearchPrefix(CommandArgs args)
        {
            if (args.Parameters.Count < 1)
            {
                args.Api.SendTextMessage($"参数不足!\n正确用法:/spr(搜前缀) <内容>");
                return;
            }
            string value = args.Parameters[0];
            if (int.TryParse(value, out var id))
            {
                var prefix = PrefixInfos.Find(i => i.PrefixId == id);
                if (prefix == null)
                {
                    args.Api.SendTextMessage($"☾搜前缀☽\n未找到关于[{value}]的相关前缀.\n{HelpText}");
                    return;
                }
                var message = new List<string>
                {
                    $"🆔前缀ID:{prefix.PrefixId}",
                    $"📕名称:{prefix.Name}"
                };
                args.Api.SendTextMessage("☾搜前缀☽\n" + string.Join("\n", message));
                return;
            }
            var list = PrefixInfos.FindAll(i => i.Name.Contains(value) || i.Alias.Contains(value));
            if (list == null || list.Count < 1)
            {
                args.Api.SendTextMessage($"☾搜前缀☽\n未找到关于[{value}]的相关前缀.\n{HelpText}");
                return;
            }
            else if (list.Count == 1)
            {
                var prefix = list[0];
                var message = new List<string>
                {
                    $"🆔前缀ID:{prefix.PrefixId}",
                    $"📕名称:{prefix.Name}"
                };
                args.Api.SendTextMessage("☾搜前缀☽\n" + string.Join("\n", message));
                return;
            }
            else
            {
                var prefixnamelist = new List<string>();
                list.ForEach(prefix => prefixnamelist.Add($"{prefix.Name}({prefix.PrefixId})"));
                args.Api.SendTextMessage($"☾搜前缀☽\n存在多个符合[{value}]的前缀!\n{string.Join(",", prefixnamelist)}");
            }
        }
        public static void SearchProjects(CommandArgs args)
        {
            if (args.Parameters.Count < 1)
            {
                args.Api.SendTextMessage($"参数不足!\n正确用法:/sp(搜弹幕) <内容>");
                return;
            }
            string value = args.Parameters[0];
            if (int.TryParse(value, out var id))
            {
                var proj = ProjectInfos.Find(i => i.ProjId == id);
                if (proj == null)
                {
                    args.Api.SendTextMessage($"☾搜弹幕☽\n未找到关于[{value}]的相关弹幕.\n{HelpText}");
                    return;
                }
                var message = new List<string>
                {
                    $"🆔弹幕ID:{proj.ProjId}",
                    $"📕名称:{proj.Name}",
                    $"⚡AiStyle:{proj.AiStyle}",
                    $"🕊友好:{(proj.Friendly ? "是":"否")}"
                };
                args.Api.SendTextMessage("☾搜弹幕☽\n" + string.Join("\n", message));
                return;
            }
            var list = ProjectInfos.FindAll(i => i.Name.Contains(value) || i.Alias.Contains(value));
            if (list == null || list.Count < 1)
            {
                args.Api.SendTextMessage($"☾搜弹幕☽\n未找到关于[{value}]的相关弹幕.\n{HelpText}");
                return;
            }
            else if (list.Count == 1)
            {
                var proj = list[0];
                var message = new List<string>
                {
                    $"🆔弹幕ID:{proj.ProjId}",
                    $"📕名称:{proj.Name}",
                    $"⚡AiStyle:{proj.AiStyle}",
                    $"🕊友好:{(proj.Friendly ? "是":"否")}"
                };
                args.Api.SendTextMessage("☾搜弹幕☽\n" + string.Join("\n", message));
                return;
            }
            else
            {
                var projnamelist = new List<string>();
                list.ForEach(proj => projnamelist.Add($"{proj.Name}({proj.ProjId})"));
                args.Api.SendTextMessage($"☾搜弹幕☽\n存在多个符合[{value}]的弹幕!\n{string.Join(",", projnamelist)}");
            }
        }
        public static void SearchItems(CommandArgs args)
        {
            if (args.Parameters.Count < 1)
            {
                args.Api.SendTextMessage($"参数不足!\n正确用法:/si(搜物品) <内容>");
                return;
            }
            string value = args.Parameters[0];
            if(int.TryParse(value,out var id))
            {
                var item = ItemInfos.Find(i=>i.ItemId == id);
                if (item == null)
                {
                    args.Api.SendTextMessage($"☾搜物品☽\n未找到关于[{value}]的相关物品.\n{HelpText}");
                    return;
                }
                var itempath = Path.Combine(Directory.GetCurrentDirectory() + "/Pictures", $"物品ID/Item_{item.ItemId}.png");
                var message = new List<string>
                {
                    $"🆔物品ID:{item.ItemId}",
                    $"📕名称:{item.Name}",
                    $"🔗堆叠:{item.MaxStack}",
                    (item.MonetaryValue.HasValue() ? ($"💰价值:" + ((item.MonetaryValue.Copper > 0) ? $"{item.MonetaryValue.Copper}铜 " : "") + ((item.MonetaryValue.Silver > 0) ? $"{item.MonetaryValue.Silver}银 " : "") + ((item.MonetaryValue.Gold > 0) ? $"{item.MonetaryValue.Gold}金 " : "") + ((item.MonetaryValue.Platinum > 0) ? $"{item.MonetaryValue.Platinum}铂" : "")) : "💰无价值")
                };
                if (item.Damage > 0)
                {
                    message.Add($"⚔️伤害:{item.Damage}");
                }
                if (item.Shoot > 0)
                {
                    message.Add($"💥射弹:{ProjectInfos.Find(p => p.ProjId == item.Shoot).Name}({item.Shoot})");
                }
                if (!string.IsNullOrEmpty(item.Description))
                {
                    message.Add("\u0023\uFE0F\u20E3" + item.Description.Replace("\n", "\n\u0023\uFE0F\u20E3"));
                }
                args.Api.SendTextWithImage(File.ReadAllBytes(itempath), "\n☾搜物品☽\n" + string.Join("\n", message));
                return;
            }
            var list = ItemInfos.FindAll(i => i.Name.Contains(value) || i.Alias.Contains(value));
            if(list == null || list.Count < 1)
            {
                args.Api.SendTextMessage($"☾搜物品☽\n未找到关于[{value}]的相关物品.\n{HelpText}");
                return;
            }
            else if(list.Count == 1)
            {
                var item = list[0];
                var itempath = Path.Combine(Directory.GetCurrentDirectory() + "/Pictures", $"物品ID/Item_{item.ItemId}.png");
                var message = new List<string>
                {
                    $"🆔物品ID:{item.ItemId}",
                    $"📕名称:{item.Name}",
                    $"🔗堆叠:{item.MaxStack}",
                    (item.MonetaryValue.HasValue() ? ($"💰价值:" + ((item.MonetaryValue.Copper > 0) ? $"{item.MonetaryValue.Copper}铜 " : "") + ((item.MonetaryValue.Silver > 0) ? $"{item.MonetaryValue.Silver}银 " : "") + ((item.MonetaryValue.Gold > 0) ? $"{item.MonetaryValue.Gold}金 " : "") + ((item.MonetaryValue.Platinum > 0) ? $"{item.MonetaryValue.Platinum}铂" : "")) : "💰无价值")
                };
                if (item.Damage > 0)
                {
                    message.Add($"⚔️伤害:{item.Damage}");
                }
                if (item.Shoot > 0)
                {
                    message.Add($"💥射弹:{ProjectInfos.Find(p=>p.ProjId == item.Shoot).Name}({item.Shoot})");
                }
                if (!string.IsNullOrEmpty(item.Description))
                {
                    message.Add("\u0023\uFE0F\u20E3" + item.Description.Replace("\n", "\n\u0023\uFE0F\u20E3"));
                }
                args.Api.SendTextWithImage(File.ReadAllBytes(itempath), "\n☾搜物品☽\n" + string.Join("\n", message));
            }
            else
            {
                var itemnamelist = new List<string>();
                list.ForEach(item => itemnamelist.Add($"{item.Name}({item.ItemId})"));
                args.Api.SendTextMessage($"☾搜物品☽\n存在多个符合[{value}]的物品!\n{string.Join(",",itemnamelist)}");
            }
        }
        public static void SearchNpcs(CommandArgs args)
        {
            if (args.Parameters.Count < 1)
            {
                args.Api.SendTextMessage($"参数不足!\n正确用法:/sn(搜生物) <内容>");
                return;
            }
            string value = args.Parameters[0];
            if (int.TryParse(value, out var id))
            {
                var npc= NpcInfos.Find(i => i.NpcId == id);
                if (npc == null)
                {
                    args.Api.SendTextMessage($"☾搜生物☽\n未找到关于[{value}]的相关生物.\n{HelpText}");
                    return;
                }
                var message = new List<string>
                {
                    $"🆔NPCID:{npc.NpcId}",
                    $"📕名称:{npc.Name}",
                    $"❤血量:{npc.LifeMax}",
                    (npc.MonetaryValue.HasValue() ? ($"💰价值:" + ((npc.MonetaryValue.Copper > 0) ? $"{npc.MonetaryValue.Copper}铜 " : "") + ((npc.MonetaryValue.Silver > 0) ? $"{npc.MonetaryValue.Silver}银 " : "") + ((npc.MonetaryValue.Gold > 0) ? $"{npc.MonetaryValue.Gold}金 " : "") + ((npc.MonetaryValue.Platinum > 0) ? $"{npc.MonetaryValue.Platinum}铂" : "")) : "💰无价值")
                };
                if (npc.Damage > 0)
                {
                    message.Add($"⚔️伤害:{npc.Damage}");
                }
                if (!string.IsNullOrEmpty(npc.Description))
                {
                    message.Add("\u0023\uFE0F\u20E3" + npc.Description.Replace("\n", "\n\u0023\uFE0F\u20E3"));
                }
                args.Api.SendTextMessage("☾搜生物☽\n" + string.Join("\n", message));
                return;
            }
            var list = NpcInfos.FindAll(i => i.Name.Contains(value) || i.Alias.Contains(value));
            if (list == null || list.Count < 1)
            {
                args.Api.SendTextMessage($"☾搜生物☽\n未找到关于[{value}]的相关生物.\n{HelpText}");
                return;
            }
            else if (list.Count == 1)
            {
                var npc = list[0];
                var message = new List<string>
                {
                    $"🆔NPCID:{npc.NpcId}",
                    $"📕名称:{npc.Name}",
                    $"❤血量:{npc.LifeMax}",
                    (npc.MonetaryValue.HasValue() ? ($"💰价值:" + ((npc.MonetaryValue.Copper > 0) ? $"{npc.MonetaryValue.Copper}铜 " : "") + ((npc.MonetaryValue.Silver > 0) ? $"{npc.MonetaryValue.Silver}银 " : "") + ((npc.MonetaryValue.Gold > 0) ? $"{npc.MonetaryValue.Gold}金 " : "") + ((npc.MonetaryValue.Platinum > 0) ? $"{npc.MonetaryValue.Platinum}铂" : "")) : "💰无价值")
                };
                if(npc.Damage > 0)
                {
                    message.Add($"⚔️伤害:{npc.Damage}");
                }
                if (!string.IsNullOrEmpty(npc.Description))
                {
                    message.Add("\u0023\uFE0F\u20E3" + npc.Description.Replace("\n", "\n\u0023\uFE0F\u20E3"));
                }
                args.Api.SendTextMessage("☾搜生物☽\n" + string.Join("\n", message));
            }
            else
            {
                var npcnamelist = new List<string>();
                list.ForEach(npc => npcnamelist.Add($"{npc.Name}({npc.NpcId})"));
                args.Api.SendTextMessage($"☾搜生物☽\n存在多个符合[{value}]的生物!\n{string.Join(",", npcnamelist)}");
            }
        }
        public static void SearchBuffs(CommandArgs args)
        {
            if (args.Parameters.Count < 1)
            {
                args.Api.SendTextMessage($"参数不足!\n正确用法:/sb(搜增益) <内容>");
                return;
            }
            string value = args.Parameters[0];
            if (int.TryParse(value, out var id))
            {
                var buff = BuffInfos.Find(i => i.BuffId== id);
                if (buff == null)
                {
                    args.Api.SendTextMessage($"☾搜增益☽\n未找到关于[{value}]的相关Buff.\n{HelpText}");
                    return;
                }
                //var itempath = Path.Combine(Directory.GetCurrentDirectory() + "/Pictures", $"物品ID/Item_{item.ItemId}.png");
                var message = new List<string>
                {
                    $"🆔BuffID:{buff.BuffId}",
                    $"📕名称:{buff.Name}"
                };
                if (!string.IsNullOrEmpty(buff.Description))
                {
                    message.Add("\u0023\uFE0F\u20E3" + buff.Description.Replace("\n", "\n\u0023\uFE0F\u20E3"));
                }
                args.Api.SendTextMessage("☾搜增益☽\n" + string.Join("\n", message));
                return;
            }
            var list = BuffInfos.FindAll(i => i.Name.Contains(value) || i.Alias.Contains(value));
            if (list == null || list.Count < 1)
            {
                args.Api.SendTextMessage($"☾搜增益☽\n未找到关于[{value}]的相关Buff.\n{HelpText}");
                return;
            }
            else if (list.Count == 1)
            {
                var buff = list[0];
                var message = new List<string>
                {
                    $"🆔BuffID:{buff.BuffId}",
                    $"📕名称:{buff.Name}"
                };
                if (!string.IsNullOrEmpty(buff.Description))
                {
                    message.Add("\u0023\uFE0F\u20E3" + buff.Description.Replace("\n", "\n\u0023\uFE0F\u20E3"));
                }
                args.Api.SendTextMessage("☾搜增益☽\n" + string.Join("\n", message));
                return;
            }
            else
            {
                var buffnamelist = new List<string>();
                list.ForEach(buff => buffnamelist.Add($"{buff.Name}({buff.BuffId})"));
                args.Api.SendTextMessage($"☾搜增益☽\n存在多个符合[{value}]的增益!\n{string.Join(",", buffnamelist)}");
            }
        }
        public static void WikiCommand(CommandArgs args)
        {
            if (args.Parameters.Count < 1)
            {
                args.Api.SendTextMessage($"参数不足!\n正确用法:wiki <内容>");
                return;
            }
            string item = args.Parameters[0];
            args.Api.SendTextMessage($"正在生成泰拉WIKI上关于[{item}]的页面！\n有延迟请耐心等待!");
            var op = new ChromeOptions();
            op.AddArguments("headless", "disable-gpu");
            using (var dir = new ChromeDriver(op))
            {
                var t2 = Task<byte[]>.Run(() =>
                {
                    dir.Navigate().GoToUrl($"https://terraria.wiki.gg/zh/wiki/Special:%E6%90%9C%E7%B4%A2?search={System.Web.HttpUtility.UrlEncode(item)}");
                    string w = dir.ExecuteScript("return document.body.scrollWidth").ToString();
                    string h = dir.ExecuteScript("return document.body.scrollHeight").ToString();
                    dir.Manage().Window.Size = new System.Drawing.Size(int.Parse(w), int.Parse(h));
                    return new MemoryStream(dir.GetScreenshot().AsByteArray);
                });
                args.Api.SendImage(t2.Result.ToArray());
            }
        }
    }
}