using PluginCore;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AntiRecall
{
    internal class AntiRecallCore
    {
        internal static string path = Path.Combine(PluginPathProvider.PluginsRootPath(), nameof(AntiRecall), "AntiRecall.txt");

        internal static Dictionary<uint, string> HistoryCall = new();

        internal static void Read()
        {
            StreamReader sr = new(path, Encoding.UTF8);
            while (sr.ReadLine() != null)
            {
                var li = sr.ReadLine().Split(",");
                HistoryCall.Add(uint.Parse(li[0]), li[1]);
            }
        }

        internal static void Write()
        {
            FileStream fs = new(path, FileMode.Create);
            StreamWriter sw = new(fs);
            foreach (var d in HistoryCall)
            {
                sw.WriteLine(d.Key + "," + d.Value);
            }
            sw.Flush();
            sw.Close();
            fs.Dispose();
            if (HistoryCall.Count > PluginSettingsModelFactory.Create<SettingsModel>(nameof(AntiRecallCore)).ClearHistoryByNum)
            {
                HistoryCall.Clear();
            }
        }
    }
}