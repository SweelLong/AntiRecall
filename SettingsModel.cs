using Konata.Core.Message;
using PluginCore.Models;
using System.Collections.Generic;

namespace AntiRecall
{
    public class SettingsModel : PluginSettingsModel
    {
        public int ClearHistoryByNum { get; set; }
    }
}