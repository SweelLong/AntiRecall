using System;
using System.Threading.Tasks;
using AntiRecall.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using PluginCore.IPlugins;
using Konata.Core;
using Konata.Core.Events.Model;
using Konata.Core.Interfaces.Api;
using KonataPlugin;
using Konata.Core.Message;
using System.Text.RegularExpressions;
using Konata.Core.Message.Model;
using System.Reflection;

namespace AntiRecall
{
    public class AntiRecall : BasePlugin, IStartupXPlugin, IWidgetPlugin, IQQBotPlugin
    {
        public void OnBotOnline((Bot s, BotOnlineEvent e) obj, string botName, uint botUin) { }

        public void OnBotOffline((Bot s, BotOfflineEvent e) obj, string botName, uint botUin) { }

        public void ConfigureServices(IServiceCollection services) { }

        public void Configure(IApplicationBuilder app) => app.UseMiddleware<SayHelloMiddleware>();
        
        public int ConfigureOrder => 2;

        public int ConfigureServicesOrder => 2;

        public async Task<string> Widget(string widgetKey, params string[] extraPars)
        {
            string rtnStr = null;
            if (widgetKey == "PluginCore.Admin.Footer")
            {
                if (extraPars != null)
                {
                    Console.WriteLine(string.Join(",", extraPars));
                }
                rtnStr = @"<div style=""border:1px solid green;width:300px;"">
                                <h3>AntiRecall 注入</h3>
                                <div>AntiRecall 挂件</div>
                           </div>";// 这是HTML标签吗，思考.jpg
            }
            return await Task.FromResult(rtnStr);
        }

        public override (bool IsSuccess, string Message) BeforeDisable()
        {
            Console.WriteLine($"{nameof(AntiRecall)}: {nameof(BeforeDisable)}");
            return base.BeforeDisable();
        }

        public override (bool IsSuccess, string Message) AfterEnable()
        {
            AntiRecallCore.Read();
            KonataBotStore.Bot.OnGroupMessageRecall += (Bot sender, GroupMessageRecallEvent args) =>
            {
                MessageBuilder mb = new();
                mb.At(args.AffectedUin);
                mb.Text("试图撤回一条消息：\n");
                try
                {
                    string coreText = AntiRecallCore.HistoryCall[args.Sequence];
                    GetCore(); void GetCore()
                    {
                        char[] str = coreText.ToCharArray();// 获取消息
                        for (int i = 0; i < str.Length; i++)
                        {
                            try
                            {
                                if (str[i] == '[')
                                {
                                    // 试图使用反射获取图片等信息，可能会影响性能
                                    if (new string(str[(i + 1)..(i + 9)]) == "KQ:image")
                                    {
                                        // 正则匹配提取关键代码
                                        string temp = new Regex("\\[KQ:image,(.+?)]").Match(coreText).Groups[0].Value;
                                        coreText = coreText.Replace(temp, "");
                                        mb.Add(typeof(ImageChain).GetMethod("Parse", BindingFlags.NonPublic | BindingFlags.Static).Invoke(this, new object[] { temp }) as ImageChain);
                                        GetCore(); break;
                                    }
                                    if (new string(str[(i + 1)..(i + 8)]) == "KQ:face")
                                    {
                                        string temp = new Regex("\\[KQ:face,(.+?)]").Match(coreText).Groups[0].Value;
                                        coreText = coreText.Replace(temp, "");
                                        mb.Add(typeof(QFaceChain).GetMethod("Parse", BindingFlags.NonPublic | BindingFlags.Static).Invoke(this, new object[] { temp }) as QFaceChain);
                                        GetCore(); break;
                                    }
                                    if (new string(str[(i + 1)..(i + 6)]) == "KQ:at")
                                    {
                                        string temp = new Regex("\\[KQ:at,(.+?)]").Match(coreText).Groups[0].Value;
                                        coreText = coreText.Replace(temp, "");
                                        mb.Add(typeof(AtChain).GetMethod("Parse", BindingFlags.NonPublic | BindingFlags.Static).Invoke(this, new object[] { temp }) as AtChain);
                                        GetCore(); break;
                                    }
                                    if (new string(str[(i + 1)..(i + 9)]) == "KQ:flash")
                                    {
                                        string temp = new Regex("\\[KQ:flash,(.+?)]").Match(coreText).Groups[0].Value;
                                        coreText = coreText.Replace(temp, "");
                                        mb.Add(typeof(FlashImageChain).GetMethod("Parse", BindingFlags.NonPublic | BindingFlags.Static).Invoke(this, new object[] { temp }) as FlashImageChain);
                                        GetCore(); break;
                                    }
                                    if (new string(str[(i + 1)..(i + 10)]) == "KQ:record")
                                    {
                                        string temp = new Regex("\\[KQ:record,(.+?)]").Match(coreText).Groups[0].Value;
                                        coreText = coreText.Replace(temp, "");
                                        mb.Add(typeof(RecordChain).GetMethod("Parse", BindingFlags.NonPublic | BindingFlags.Static).Invoke(this, new object[] { temp }) as RecordChain);
                                        GetCore(); break;
                                    }
                                }
                            }
                            catch
                            {
                                continue;
                            }
                        }
                    }
                    mb.Text(coreText);
                }
                catch
                {
                    mb.Text("该部分已隐藏");
                }
                sender.SendGroupMessage(args.GroupUin, mb);
            };
            Console.WriteLine($"{nameof(AntiRecall)}: {nameof(AfterEnable)}");
            return base.AfterEnable();
        }

        public void OnGroupMessage((Bot s, GroupMessageEvent e) obj, string message, string groupName, uint groupUin, uint memberUin)
        {
            string str = "";
            foreach (BaseChain c in obj.e.Chain)// 通过数据链转字符串的方式保留图片等代码
            {
                str += c.ToString();
            }
            AntiRecallCore.HistoryCall.Add(obj.e.Message.Sequence, str);
            AntiRecallCore.Write();
        }

        public void OnFriendMessage((Bot s, FriendMessageEvent e) obj, string message, uint friendUin)
        {
            string str = "";
            foreach (BaseChain c in obj.e.Chain)
            {
                str += c.ToString();
            }
            AntiRecallCore.HistoryCall.Add(obj.e.Message.Sequence, str);
            AntiRecallCore.Write();
        }
    }
}