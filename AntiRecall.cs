using System;
using PluginCore.IPlugins;
using QQBotHub.Sdk.IPlugins;
using Konata.Core;
using Konata.Core.Events.Model;
using Konata.Core.Interfaces.Api;
using Konata.Core.Message;
using AntiRecall.Methods;
using QQBotHub.Sdk;
using Konata.Core.Message.Model;
using System.Reflection;
using System.Text.RegularExpressions;

namespace AntiRecall
{
    public class AntiRecall : BasePlugin, IQQBotPlugin
    {
        public override (bool IsSuccess, string Message) AfterEnable()
        {
            AntiRecallCore.Read();
            QQBotStore.Bot.OnGroupMessageRecall += (Bot sender, GroupMessageRecallEvent args) =>
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

        public override (bool IsSuccess, string Message) BeforeDisable()
        {
            Console.WriteLine($"{nameof(AntiRecall)}: {nameof(BeforeDisable)}");
            return base.BeforeDisable();
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

        public void OnBotOnline((Bot s, BotOnlineEvent e) obj, string botName, uint botUin)
        {
        }

        public void OnBotOffline((Bot s, BotOfflineEvent e) obj, string botName, uint botUin)
        {
        }
    }
}